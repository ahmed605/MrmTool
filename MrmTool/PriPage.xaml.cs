using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using WinRT;
using MrmLib;
using MrmTool.Models;
using MrmTool.Common;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static MrmTool.Common.ErrorHelpers;

namespace MrmTool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PriPage : Page
    {
        private PriFile? _pri;
        private StorageFile? _currentFile;
        private StorageFolder? _rootFolder;
        private ResourceItem? _selectedResource;

        public ObservableCollection<ResourceItem> ResourceItems { get; } = [];

        public PriPage()
        {
            InitializeComponent();
        }

        [DynamicWindowsRuntimeCast(typeof(PriFile))]
        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is (PriFile pri, StorageFile file))
            {
                LoadPri(pri);
                _currentFile = file;
            }
        }

        private ResourceItem GetOrAddResourceItem(string name)
        {
            string[] split = name.Split('/');
            ResourceItem? currentParent = null;
            foreach (var item in split)
            {
                ObservableCollection<ResourceItem> currentList = currentParent?.Children ?? ResourceItems;
                currentParent = currentList.FirstOrDefault(i => i.Name == item);
                if (currentParent is null)
                {
                    currentParent = new ResourceItem(item);
                    currentList.Add(currentParent);
                }
            }

            return currentParent!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear()
        {
            ResourceItems.Clear();
            // TODO: do we need to do any other cleanup here?
        }

        private void LoadPri(PriFile pri)
        {
            Clear();

            _pri = pri;

            foreach (var candidate in pri.ResourceCandidates)
            {
                var item = GetOrAddResourceItem(candidate.ResourceName);
                item.Candidates.Add(candidate);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async Task TryLoadPri(StorageFile file)
        {
            try
            {
                LoadPri(await PriFile.LoadAsync(file));
                _currentFile = file;
                _rootFolder = null;
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new()
                {
                    Title = "Error",
                    Content = $"Failed to load the selected PRI file.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}\r\nStacktrace:\r\n\r\n{ex.StackTrace}",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    Template = (ControlTemplate)Program.Application.Resources["ScrollableContentDialogTemplate"]
                };

                await dialog.ShowAsync();
            }
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".pri");
            picker.CommitButtonText = "Load";
            picker.Initialize();

            if (await picker.PickSingleFileAsync() is { } file)
            {
                await TryLoadPri(file);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async Task SavePri(StorageFile file)
        {
            try
            {
                using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                stream.Size = 0;

                await _pri!.WriteAsync(stream);
                _currentFile = file;
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new()
                {
                    Title = "Error",
                    Content = $"Failed to save the PRI file.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}\r\nStacktrace:\r\n\r\n{ex.StackTrace}",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    Template = (ControlTemplate)Program.Application.Resources["ScrollableContentDialogTemplate"]
                };

                await dialog.ShowAsync();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await SavePri(_currentFile!);
        }

        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new();
            picker.FileTypeChoices.Add("PRI File", new List<string>() { ".pri" });
            picker.Initialize();

            if (await picker.PickSaveFileAsync() is { } file)
            {
                await SavePri(file);
            }
        }

        private void AddResource_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void RemoveResources_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private async Task PickRootFolder()
        {
            FolderPicker picker = new();
            picker.FileTypeFilter.Add("*");
            picker.CommitButtonText = "Select PRI Root Folder";
            picker.Initialize();

            if (await picker.PickSingleFolderAsync() is { } folder)
            {
                _rootFolder = folder;
            }
        }

        private async void SetRootFolder_Click(object sender, RoutedEventArgs e)
        {
            await PickRootFolder();

            if (_rootFolder is not null && candidatesList.SelectedItem is CandidateItem item && item.Candidate.ValueType is ResourceValueType.Path)
            {
                await DisplayCandidate(item);
            }
        }

        private async void EmbedPathResources_Click(object sender, RoutedEventArgs e)
        {
            if (_rootFolder is null)
            {
                await PickRootFolder();

                if (_rootFolder is null)
                {
                    ContentDialog dialog = new()
                    {
                        Title = "Error",
                        Content = "Please select PRI root folder first in order to embed path resources into the PRI file.",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                    };

                    await dialog.ShowAsync();
                    return;
                }
            }

            await _pri!.ReplacePathCandidatesWithEmbeddedDataAsync(_rootFolder);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SendMessageW(Program.WindowHandle, WM.WM_CLOSE, 0, 0);
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            var view = e.DataView;
            if (view.Contains(StandardDataFormats.StorageItems))
            {
                var path = view.GetFirstStorageItemPathUnsafe();
                if (path is null || Path.GetExtension(path).ToLowerInvariant() is ".pri")
                {
                    e.AcceptedOperation = DataPackageOperation.Copy;
                    e.DragUIOverride.Caption = "Drop to load the PRI file";
                    e.Handled = true;
                }
                else
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                }
            }
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0 && items[0] is StorageFile file && file.Name.ToLowerInvariant().EndsWith(".pri"))
                {
                    await TryLoadPri(file);
                    e.Handled = true;
                }
            }
        }

        private void treeView_SelectionChanged(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewSelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count is 1 && args.AddedItems[0] is ResourceItem item && item.Candidates.Count > 0)
            {
                _selectedResource = item;
                candidatesList.ItemsSource = item.Candidates;
            }
        }

        private void UnloadOtherPreviewElements(CandidateItem item)
        {
            UnloadObject(invalidRootPathContainer);
            UnloadObject(failedToOpenFileContainer);

            if (_selectedResource?.Type.IsText is not true)
                UnloadObject(valueTextEditor);

            if (_selectedResource?.Type is not ResourceType.Image)
                UnloadObject(imagePreviewerContainer);

            if (!(item.ValueType is ResourceValueType.EmbeddedData && _selectedResource?.Type.IsPreviewable is not true))
                UnloadObject(exportContainer);

            if (!(item.ValueType is ResourceValueType.Path && _selectedResource?.Type.IsPreviewable is not true))
                UnloadObject(openFolderContainer);
        }

        private void UnloadAllPreviewElements()
        {
            UnloadObject(invalidRootPathContainer);
            UnloadObject(failedToOpenFileContainer);
            UnloadObject(valueTextEditor);
            UnloadObject(imagePreviewerContainer);
            UnloadObject(exportContainer);
            UnloadObject(openFolderContainer);
        }

        private void UnloadNonErrorPreviewElements()
        {
            UnloadObject(valueTextEditor);
            UnloadObject(imagePreviewerContainer);
            UnloadObject(exportContainer);
            UnloadObject(openFolderContainer);
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        private async Task DisplayCandidate(CandidateItem item)
        {
            UnloadOtherPreviewElements(item);

            var candidate = item.Candidate;
            if (candidate.ValueType is ResourceValueType.Path)
            {
                if (await TryResolvePathCandidate(candidate.StringValue) is StorageFile file)
                {
                    await DisplayPathCandidate(file);
                    return;
                }

                UnloadNonErrorPreviewElements();
                FindName(nameof(invalidRootPathContainer));
            }
            else if (candidate.ValueType is ResourceValueType.EmbeddedData)
            {
                using (MemoryStream stream = new(candidate.DataValue))
                {
                    if (await DisplayBinaryCandidate(stream.AsRandomAccessStream(), _selectedResource!.Type))
                        return;
                }

                FindName(nameof(exportContainer));
                fileSizeLabel.Text = $"File Size: {candidate.DataValue.Length} bytes";
            }
            else
            {
                DisplayStringCandidate(candidate.StringValue);
            }
        }

        private void DisplayStringCandidate(string str)
        {
            FindName(nameof(valueTextEditor));

            var editor = valueTextEditor.Editor;
            editor.ReadOnly = false;
            editor.WrapMode = WinUIEditor.Wrap.Word;
            editor.CaretStyle = WinUIEditor.CaretStyle.Invisible;
            editor.SetText(str);
            editor.ReadOnly = true;

            valueTextEditor.ApplyDefaultsToDocument();

            if (_selectedResource is not null)
                valueTextEditor.HighlightingLanguage = _selectedResource.Type is ResourceType.Xaml ? "xml" : _selectedResource.DisplayName.GetExtensionAfterPeriod();
        }

        private async Task<bool> DisplayBinaryCandidate(IRandomAccessStream stream, ResourceType type)
        {
            try
            {
                if (type == ResourceType.Image)
                {
                    BitmapImage image = new();
                    await image.SetSourceAsync(stream);
                    FindName(nameof(imagePreviewerContainer));
                    imagePreviewer.Source = image;

                    imagePreviewer.Stretch = Stretch.None;
                    imagePreviewer.MaxWidth = double.PositiveInfinity;
                    imagePreviewer.MaxHeight = double.PositiveInfinity;

                    imagePreviewerContainer.UpdateLayout();

                    var imageWidth = imagePreviewer.ActualWidth;
                    var imageHeight = imagePreviewer.ActualHeight;
                    var containerWidth = imagePreviewerContainer.ActualWidth;
                    var containerHeight = imagePreviewerContainer.ActualHeight;

                    if (imageWidth > containerWidth ||
                        imageHeight > containerHeight)
                    {
                        var ratio = Math.Min(containerWidth / imageWidth, containerHeight / imageHeight);
                        if (ratio < 0.1d)
                        {
                            imagePreviewer.MaxWidth = containerWidth / 0.1d;
                            imagePreviewer.MaxHeight = containerHeight / 0.1d;
                            imagePreviewer.Stretch = Stretch.Uniform;

                            ratio = 0.1d;
                        }

                        imagePreviewerContainer.ChangeView(null, null, (float)ratio);
                    }

                    return true;
                }
                else if (type.IsText)
                {
                    var size = (uint)stream.Size;
                    var buffer = WindowsRuntimeBuffer.Create((int)size);
                    await stream.ReadAsync(buffer, size, InputStreamOptions.None);

                    if (WindowsRuntimeMarshal.TryGetDataUnsafe(buffer, out var ptr)) unsafe
                    {
                        DisplayStringCandidate(Encoding.UTF8.GetString((byte*)ptr, (int)size));
                        return true;
                    }
                }
            } catch { }

            return false;
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        [DynamicWindowsRuntimeCast(typeof(StorageFolder))]
        private async Task<StorageFile?> TryResolvePathCandidate(string fileName)
        {
            StorageFile? file = null;

            var root = _rootFolder ?? await _currentFile?.GetParentAsync();
            if (await root.TryGetItemAsync(fileName) is StorageFile cFile)
            {
                file = cFile;
            }
            else
            {
                if (await root.TryGetItemAsync(_currentFile?.DisplayName) is StorageFolder folder)
                {
                    file = await folder.TryGetItemAsync(fileName) as StorageFile;
                }
            }

            return file;
        }

        private async Task DisplayPathCandidate(StorageFile file)
        {
            bool result = false;
            IRandomAccessStream stream;

            if (_selectedResource?.Type.IsPreviewable is true)
            {
                try
                {
                    stream = await file.OpenAsync(FileAccessMode.Read, StorageOpenOptions.AllowReadersAndWriters);
                }
                catch (Exception ex)
                {
                    UnloadNonErrorPreviewElements();
                    FindName(nameof(failedToOpenFileContainer));

                    failedFileNameRun.Text = file.Path;
                    failedExceptionMessageRun.Text = $"{ex.GetType().Name} (0x{ex.HResult:X8}) -> {ex.Message}";
                    failedToOpenFileContainer.Visibility = Visibility.Visible;
                    return;
                }

                result = await DisplayBinaryCandidate(stream, _selectedResource.Type);
                stream.Dispose();
            }

            if (!result)
            {
                FindName(nameof(openFolderContainer));
                openFolderContainer.Tag = file.Path;
            }
        }

        private async void candidatesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count is 1 && e.AddedItems[0] is CandidateItem item)
            {
                await DisplayCandidate(item);
            }
            else
            {
                UnloadAllPreviewElements();
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceCandidate candidate = ((CandidateItem)candidatesList.SelectedItem).Candidate;
            byte[] data = candidate.ValueType is ResourceValueType.EmbeddedData ? candidate.DataValue : Encoding.UTF8.GetBytes(candidate.StringValue);

            string fileName = candidate.ResourceName.GetDisplayName();
            string extension = Path.GetExtension(fileName);

            FileSavePicker picker = new();
            picker.Initialize();
            picker.SuggestedFileName = fileName;

            if (!string.IsNullOrEmpty(extension))
            {
                picker.FileTypeChoices.Add($"{extension[1..].ToUpperInvariant()} file", new string[] { extension });
            }

            picker.FileTypeChoices.Add("All files", new string[] { "." });

            if (await picker.PickSaveFileAsync() is { } file)
                await FileIO.WriteBytesAsync(file, data);
        }

        private void SystemTheme_Click(object sender, RoutedEventArgs e)
        {
            PreviewContainer.RequestedTheme = ElementTheme.Default;
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            PreviewContainer.RequestedTheme = ElementTheme.Light;
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            PreviewContainer.RequestedTheme = ElementTheme.Dark;
        }

        private async void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            if (candidatesList.SelectedItem is CandidateItem item)
            {
                await DisplayCandidate(item);
            }
        }

        private unsafe void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (openFolderContainer.Tag is string path)
            {
                fixed (char* pPath = path)
                {
                    ITEMIDLIST* pList = default;
                    if (SUCCEEDED_LOG(SHParseDisplayName(pPath, null, &pList, 0, null)))
                    {
                        LOG_IF_FAILED(SHOpenFolderAndSelectItems(pList, 0, null, 0));
                        ILFree(pList);
                    }
                }
            }
        }

        private async void Notice_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NoticeDialog();
            await dialog.ShowAsync();
        }
    }
}
