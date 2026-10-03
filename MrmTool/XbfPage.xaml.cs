using MrmTool.Common;
using MrmTool.Scintilla;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinRT;
using WinUIEditor;
using XbfKit;
using XbfKit.IO;

namespace MrmTool;

public sealed partial class XbfPage : Page, INotifyPropertyChanged
{
    private StorageFile? _currentFile;
    private XbfVersion? _version;
    private XbfDialect _dialect = XbfDialect.WUX;
    private bool _isBusy;
    private bool _isDirty;
    private bool _includeConnectionIds = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public StorageFile? CurrentFile
    {
        get => _currentFile;
        private set
        {
            if (SetProperty(ref _currentFile, value))
            {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public XbfVersion? Version
    {
        get => _version;
        private set
        {
            if (SetProperty(ref _version, value))
            {
                OnPropertyChanged(nameof(IsXbf2));
                OnPropertyChanged(nameof(CanSave));
                OnPropertyChanged(nameof(CanSaveAs));
                OnPropertyChanged(nameof(CanSwitchDialect));
            }
        }
    }

    public XbfDialect Dialect
    {
        get => _dialect;
        private set => SetProperty(ref _dialect, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanSave));
                OnPropertyChanged(nameof(CanSaveAs));
                OnPropertyChanged(nameof(CanSwitchDialect));
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    public bool IncludeConnectionIds
    {
        get => _includeConnectionIds;
        set => SetProperty(ref _includeConnectionIds, value);
    }

    public bool CanSave => CurrentFile is not null && Version is not null && !IsBusy;
    public bool CanSaveAs => Version is not null && !IsBusy;
    public bool IsXbf2 => Version?.Major == 2;
    public bool CanSwitchDialect => IsXbf2 && !IsBusy;

    public XbfPage()
    {
        InitializeComponent();

        xamlEditor.Editor.SavePointLeft += Editor_SavePointLeft;
        xamlEditor.Editor.SavePointReached += Editor_SavePointReached;
        PropertyChanged += Page_PropertyChanged;
    }

    /// <inheritdoc/>
    [DynamicWindowsRuntimeCast(typeof(StorageFile))]
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
        if (e.Parameter is StorageFile file)
        {
            await LoadXbf(file);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
        base.OnNavigatedFrom(e);
    }

    private async Task<string> LoadAsync(StorageFile file, XbfDialect xbf2Dialect)
    {
        ArgumentNullException.ThrowIfNull(file);
        bool includeConnectionIds = IncludeConnectionIds;
        IsBusy = true;
        try
        {
            byte[] bytes;
            using (IRandomAccessStream stream = await file.OpenAsync(
                FileAccessMode.Read,
                StorageOpenOptions.AllowReadersAndWriters))
            using (Stream input = stream.AsStreamForRead())
            using (MemoryStream copy = new())
            {
                await input.CopyToAsync(copy);
                bytes = copy.ToArray();
            }

            var result = await Task.Run(() =>
            {
                using MemoryStream input = new(bytes, writable: false);
                XbfDocument document = XbfDocumentReader.Read(input);
                XbfDialect dialect = document.Version.Major == 1
                    ? XbfDialect.WUX
                    : xbf2Dialect;
                XbfDecompilationOptions options = new()
                {
                    IncludeConnectionIds = includeConnectionIds,
                };
                return (Xaml: XbfDecompiler.Decompile(document, dialect, options), document.Version, Dialect: dialect);
            });

            CurrentFile = file;
            Version = result.Version;
            Dialect = result.Dialect;
            IsDirty = false;
            return result.Xaml;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync(StorageFile file, string xaml)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (IsBusy || Version is not XbfVersion version)
        {
            return;
        }

        IsBusy = true;
        try
        {
            XbfDialect dialect = version.Major == 1 ? XbfDialect.WUX : Dialect;
            using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            stream.Size = 0;
            using Stream output = stream.AsStream();
            await Task.Run(() => XbfCompiler.Compile(xaml, output, version, dialect));
            await output.FlushAsync();

            CurrentFile = file;
            IsDirty = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task LoadXbf(StorageFile file, XbfDialect? dialect = null)
    {
        try
        {
            string xaml = await LoadAsync(file, dialect ?? Dialect);

            var editor = xamlEditor.Editor;
            editor.ReadOnly = false;
            editor.WrapMode = Wrap.Word;
            editor.CaretStyle = CaretStyle.Line;
            editor.SetText(xaml);
            xamlEditor.ApplyDefaultsToDocument();
            xamlEditor.HighlightingLanguage = "xml";
            editor.SetSavePoint();
            SyncDialectMenu();
            SyncDecompilationMenu();
        }
        catch (Exception ex)
        {
            SyncDialectMenu();
            SyncDecompilationMenu();
            await ShowLoadError(file, ex);
        }
    }

    [DynamicWindowsRuntimeCast(typeof(ToggleMenuFlyoutItem))]
    private async void IncludeConnectionIds_Click(object sender, RoutedEventArgs e)
    {
        var item = (ToggleMenuFlyoutItem)sender;
        bool requestedValue = item.IsChecked;
        bool previousValue = IncludeConnectionIds;
        if (requestedValue == previousValue)
        {
            return;
        }

        if (!await ConfirmDiscardChanges())
        {
            item.IsChecked = previousValue;
            return;
        }

        IncludeConnectionIds = requestedValue;
        if (CurrentFile is StorageFile file)
        {
            await LoadXbf(file, Dialect);
        }
    }

    private async void SystemXaml_Click(object sender, RoutedEventArgs e)
    {
        if (Dialect == XbfDialect.WUX)
        {
            return;
        }

        if (!await ConfirmDiscardChanges())
        {
            WinUiXamlMenuItem.IsChecked = true;
            return;
        }

        if (CurrentFile is StorageFile file)
        {
            await LoadXbf(file, XbfDialect.WUX);
        }
    }

    private async void WinUIXaml_Click(object sender, RoutedEventArgs e)
    {
        if (Dialect == XbfDialect.MUX)
        {
            return;
        }

        if (!await ConfirmDiscardChanges())
        {
            SystemXamlMenuItem.IsChecked = true;
            return;
        }

        if (CurrentFile is StorageFile file)
        {
            await LoadXbf(file, XbfDialect.MUX);
        }
    }

    private async void Open_Click(object sender, RoutedEventArgs e)
    {
        if (await OpenFileHelper.PickAsync() is not StorageFile file)
        {
            return;
        }

        if (!await ConfirmDiscardChanges())
        {
            return;
        }

        if (OpenFileHelper.IsPri(file.Name))
        {
            Frame.Navigate(typeof(PriPage), file);
        }
        else
        {
            await LoadXbf(file);
        }
    }

    private void Grid_DragOver(object sender, DragEventArgs e)
    {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            return;
        }

        string? path = e.DataView.GetFirstStorageItemPathUnsafe();
        if (path is null || OpenFileHelper.IsSupported(path))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Drop to open the PRI or XBF file";
            e.Handled = true;
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
        }
    }

    [DynamicWindowsRuntimeCast(typeof(StorageFile))]
    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            return;
        }

        IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
        if (items.Count == 0 || items[0] is not StorageFile file ||
            !OpenFileHelper.IsSupported(file.Name))
        {
            return;
        }

        if (!await ConfirmDiscardChanges())
        {
            return;
        }

        if (OpenFileHelper.IsPri(file.Name))
        {
            Frame.Navigate(typeof(PriPage), file);
        }
        else
        {
            await LoadXbf(file);
        }

        e.Handled = true;
    }

    private async Task SaveXbf(StorageFile file)
    {
        if (!CanSaveAs)
        {
            return;
        }

        try
        {
            await SaveAsync(file, xamlEditor.Text);
            xamlEditor.Editor.SetSavePoint();
        }
        catch (Exception ex)
        {
            await ShowSaveError(file, ex);
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentFile is StorageFile file)
        {
            await SaveXbf(file);
        }
    }

    private async void SaveAs_Click(object sender, RoutedEventArgs e)
    {
        await SaveAsXbf();
    }

    private async Task SaveAsXbf()
    {
        if (!CanSaveAs)
        {
            return;
        }

        FileSavePicker picker = new();
        picker.FileTypeChoices.Add("XBF File", new List<string> { ".xbf" });
        picker.SuggestedFileName = CurrentFile?.Name ?? "document.xbf";
        picker.Initialize();

        if (await picker.PickSaveFileAsync() is StorageFile file)
        {
            await SaveXbf(file);
        }
    }

    private void Page_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsBusy))
        {
            xamlEditor.Editor.ReadOnly = IsBusy;
        }

        if (e.PropertyName is nameof(CurrentFile) or nameof(IsDirty))
        {
            UpdateWindowTitle();
        }
    }

    private async void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
    {
        if (!CanSaveAs || args.VirtualKey != VirtualKey.S ||
            args.EventType is not (CoreAcceleratorKeyEventType.KeyDown or CoreAcceleratorKeyEventType.SystemKeyDown) ||
            !IsKeyDown(VirtualKey.Control))
        {
            return;
        }

        args.Handled = true;
        if (IsKeyDown(VirtualKey.Shift))
        {
            await SaveAsXbf();
        }
        else if (CurrentFile is StorageFile file)
        {
            await SaveXbf(file);
        }
    }

    private static bool IsKeyDown(VirtualKey key)
    {
        return (Window.Current.CoreWindow.GetKeyState(key) & CoreVirtualKeyStates.Down) != 0;
    }

    private void Editor_SavePointLeft(Editor sender, SavePointLeftEventArgs args)
    {
        IsDirty = true;
    }

    private void Editor_SavePointReached(Editor sender, SavePointReachedEventArgs args)
    {
        IsDirty = false;
    }

    private void UpdateWindowTitle()
    {
        if (CurrentFile is StorageFile file)
        {
            Program.SetWindowTitle(file.Path, IsDirty);
        }
    }

    private void SyncDialectMenu()
    {
        SystemXamlMenuItem.IsChecked = Dialect == XbfDialect.WUX;
        WinUiXamlMenuItem.IsChecked = Dialect == XbfDialect.MUX;
    }

    private void SyncDecompilationMenu()
    {
        ConnectionIdsMenuItem.IsChecked = IncludeConnectionIds;
    }

    private async Task<bool> ConfirmDiscardChanges()
    {
        if (!IsDirty)
        {
            return true;
        }

        ContentDialog dialog = new()
        {
            Title = "Unsaved changes",
            Content = $"Discard changes to '{CurrentFile?.Name}'?",
            PrimaryButtonText = "Discard",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
        };
        return await dialog.ShowAsync() == ContentDialogResult.Primary;
    }

    [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
    private static async Task ShowLoadError(StorageFile file, Exception ex)
    {
        ContentDialog dialog = new()
        {
            Title = "Error",
            Content = $"Failed to open '{file.Name}'.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}",
            CloseButtonText = "OK",
            DefaultButton = ContentDialogButton.Close,
            Template = (ControlTemplate)Program.Application.Resources["ScrollableContentDialogTemplate"],
        };
        await dialog.ShowAsync();
    }

    [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
    private static async Task ShowSaveError(StorageFile file, Exception ex)
    {
        ContentDialog dialog = new()
        {
            Title = "Error",
            Content = $"Failed to save '{file.Name}'.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}",
            CloseButtonText = "OK",
            DefaultButton = ContentDialogButton.Close,
            Template = (ControlTemplate)Program.Application.Resources["ScrollableContentDialogTemplate"],
        };
        await dialog.ShowAsync();
    }

    private unsafe void SyntaxHighlightingApplied(object sender, ElementTheme e)
    {
        xamlEditor.HandleSyntaxHighlightingApplied(e);
    }

    private async void Exit_Click(object sender, RoutedEventArgs e)
    {
        if (await ConfirmDiscardChanges())
        {
            Program.Exit();
        }
    }
}
