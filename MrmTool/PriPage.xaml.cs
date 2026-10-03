using Common;
using MrmLib;
using MrmTool.Common;
using MrmTool.Dialogs;
using MrmTool.Models;
using MrmTool.Scintilla;
using MrmTool.SVG;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRT;
using WinUIEditor;
using XbfKit;
using XbfKit.IO;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace MrmTool
{
    public sealed partial class PriPage : Page, INotifyPropertyChanged
    {
        private static readonly IComparer<ResourceItem> ResourceItemComparer =
            Comparer<ResourceItem>.Create(static (left, right) =>
            {
                int folderComparison = (right.Children.Count > 0).CompareTo(left.Children.Count > 0);
                if (folderComparison != 0)
                {
                    return folderComparison;
                }

                int nameComparison = StringComparer.OrdinalIgnoreCase.Compare(left.DisplayName, right.DisplayName);
                return nameComparison != 0
                    ? nameComparison
                    : StringComparer.Ordinal.Compare(left.DisplayName, right.DisplayName);
            });

        private readonly Dictionary<CandidateItem, CandidateEditState> _candidateEdits = [];
        private CandidateEditState? _displayedEdit;
        private Editor? _subscribedEditor;
        private bool _settingEditorText;
        private PriFile? _document;
        private StorageFile? _currentFile;
        private StorageFolder? _rootFolder;
        private ResourceItem? _selectedResource;
        private CandidateItem? _selectedCandidate;
        private ObservableCollection<ResourceItem> _resourceItems = [];
        private XbfDialect _xbf2Dialect = XbfDialect.WUX;
        private bool _isBusy;
        private bool _isDirty;
        private bool _hasDocumentChanges;
        private bool _includeConnectionIds = true;
        private bool _useWebViewForSvg;
        private bool _sortResources;
        private int _loadVersion;

        public event PropertyChangedEventHandler? PropertyChanged;

        private sealed class CandidateEditState(CandidateItem candidate, ResourceType resourceType, string originalText, XbfVersion? xbfVersion, XbfDialect dialect)
        {
            /// <summary>The text captured when this edit session started.</summary>
            internal string OriginalText { get; private set; } = originalText;

            /// <summary>The candidate being edited.</summary>
            internal CandidateItem Candidate { get; } = candidate;

            /// <summary>The resource format used to interpret the editable text.</summary>
            internal ResourceType ResourceType { get; } = resourceType;

            /// <summary>The candidate storage kind captured before editing.</summary>
            internal ResourceValueType OriginalValueType { get; } = candidate.ValueType;

            /// <summary>The current editable text.</summary>
            internal string Text { get; set; } = originalText;

            /// <summary>The original XBF version, or <see langword="null"/> for plain text.</summary>
            internal XbfVersion? XbfVersion { get; } = xbfVersion;

            /// <summary>The framework schema used to recompile XBF2.</summary>
            internal XbfDialect Dialect { get; set; } = dialect;

            /// <summary>Gets whether the current text differs from its original value.</summary>
            internal bool IsDirty => !string.Equals(Text, OriginalText, StringComparison.Ordinal);

            /// <summary>Accepts the current text as the saved value.</summary>
            internal void AcceptChanges() => OriginalText = Text;
        }

        private sealed class PreparedCandidateEdit(CandidateEditState edit, object value)
        {
            /// <summary>The source edit state.</summary>
            internal CandidateEditState Edit { get; } = edit;

            /// <summary>The prepared string or binary candidate value.</summary>
            internal object Value { get; } = value;
        }

        public PriFile? Document
        {
            get => _document;
            private set => SetProperty(ref _document, value);
        }

        public StorageFile? CurrentFile
        {
            get => _currentFile;
            private set => SetProperty(ref _currentFile, value);
        }

        public StorageFolder? RootFolder
        {
            get => _rootFolder;
            private set => SetProperty(ref _rootFolder, value);
        }

        public ObservableCollection<ResourceItem> ResourceItems
        {
            get => _resourceItems;
            private set => SetProperty(ref _resourceItems, value);
        }

        public ResourceItem? SelectedResource
        {
            get => _selectedResource;
            set
            {
                if (SetProperty(ref _selectedResource, value))
                {
                    SelectedCandidate = value?.Candidates.FirstOrDefault();
                    OnPropertyChanged(nameof(CanRemoveResource));
                }
            }
        }

        public CandidateItem? SelectedCandidate
        {
            get => _selectedCandidate;
            set => SetProperty(ref _selectedCandidate, value);
        }

        public XbfDialect Xbf2Dialect
        {
            get => _xbf2Dialect;
            set => SetProperty(ref _xbf2Dialect, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !IsBusy;

        public bool IsDirty
        {
            get => _isDirty;
            private set => SetProperty(ref _isDirty, value);
        }

        public bool CanRemoveResource => SelectedResource is not null;

        public bool IncludeConnectionIds
        {
            get => _includeConnectionIds;
            set => SetProperty(ref _includeConnectionIds, value);
        }

        public bool UseWebViewForSvg
        {
            get => _useWebViewForSvg;
            set => SetProperty(ref _useWebViewForSvg, value);
        }

        public bool SortResources
        {
            get => _sortResources;
            set
            {
                if (SetProperty(ref _sortResources, value) && value)
                {
                    SortResourceItems();
                }
            }
        }

        public PriPage()
        {
            InitializeComponent();
            PropertyChanged += Page_PropertyChanged;
        }

        /// <inheritdoc/>
        [DynamicWindowsRuntimeCast(typeof(PriFile))]
        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;

            if (e.Parameter is ValueTuple<PriFile, StorageFile> loadedDocument)
            {
                ApplyLoadedDocument(loadedDocument.Item1, loadedDocument.Item2);
            }
            else if (e.Parameter is StorageFile storageFile)
            {
                await TryLoadPri(storageFile);
            }
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
            if (_subscribedEditor is not null)
            {
                _subscribedEditor.Modified -= Editor_Modified;
                _subscribedEditor = null;
            }

            CancelPendingLoad();
            base.OnNavigatedFrom(e);
        }

        private void Page_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsBusy))
            {
                resourceLoadingIndicator.Visibility = IsBusy
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                if (_subscribedEditor is not null)
                {
                    _subscribedEditor.ReadOnly = IsBusy;
                }
            }

            if (e.PropertyName is nameof(CurrentFile) or nameof(IsDirty))
            {
                UpdateWindowTitle();
            }
        }

        private async Task<bool> LoadAsync(StorageFile file)
        {
            ArgumentNullException.ThrowIfNull(file);
            int loadVersion = ++_loadVersion;
            IsBusy = true;
            try
            {
                PriFile document = await PriFile.LoadAsync(file);
                // Keep the projected collection enumeration on the current apartment.
                // Moving it into Task.Run can leave the PRI page permanently busy.
                ObservableCollection<ResourceItem> resources = BuildResourceTree(document);
                var result = (Document: document, Resources: resources);

                if (loadVersion != _loadVersion)
                {
                    return false;
                }

                ApplyLoadedDocument(result.Document, file, result.Resources);
                return true;
            }
            catch (Exception) when (loadVersion != _loadVersion)
            {
                return false;
            }
            finally
            {
                if (loadVersion == _loadVersion)
                {
                    IsBusy = false;
                }
            }
        }

        private void CancelPendingLoad()
        {
            _loadVersion++;
            IsBusy = false;
        }

        private void ApplyLoadedDocument(
            PriFile document,
            StorageFile file,
            ObservableCollection<ResourceItem>? resources = null)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(file);

            SelectedResource = null;
            Document = document;
            ResourceItems = resources ?? BuildResourceTree(document);
            SortResourceItems();
            CurrentFile = file;
            RootFolder = null;
            _hasDocumentChanges = false;
            IsDirty = false;
        }

        private async Task SaveAsync(StorageFile file)
        {
            ArgumentNullException.ThrowIfNull(file);
            if (IsBusy || Document is not { } document)
            {
                return;
            }

            IsBusy = true;
            try
            {
                using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                stream.Size = 0;
                await document.WriteAsync(stream);
                CurrentFile = file;
                _hasDocumentChanges = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddResource(CandidateItem candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            if (Document is null)
            {
                return;
            }

            ResourceItem item = GetOrAddResourceItem(candidate.Candidate.ResourceName);
            item.Candidates.Add(candidate);
            Document.ResourceCandidates.Add(candidate.Candidate);
            SortResourceItems();
            MarkDirty();
        }

        private void RemoveResource(ResourceItem resource)
        {
            ArgumentNullException.ThrowIfNull(resource);
            if (Document is null)
            {
                return;
            }

            if (ReferenceEquals(resource, SelectedResource))
            {
                SelectedResource = null;
            }

            resource.Delete(Document);
            SortResourceItems();
            MarkDirty();
        }

        private void SortResourceItems()
        {
            if (SortResources)
            {
                SortResourceTree(ResourceItems, preserveCollectionState: true);
            }
        }

        private void ReparentRenamedResource(ResourceItem resource)
        {
            ArgumentNullException.ThrowIfNull(resource);
            resource.Parent.Remove(resource);
            ObservableCollection<ResourceItem> parent = resource.Name.GetParentName() is string parentName
                ? GetOrAddResourceItem(parentName).Children
                : ResourceItems;
            resource.Parent = parent;
            parent.Add(resource);
            SortResourceItems();
            MarkDirty();
        }

        private void AddCandidate(CandidateItem candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            if (SelectedResource is null || Document is null)
            {
                return;
            }

            SelectedResource.Candidates.Add(candidate);
            Document.ResourceCandidates.Add(candidate.Candidate);
            SelectedCandidate = candidate;
            MarkDirty();
        }

        private void DeleteCandidate(CandidateItem candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            if (SelectedResource is null || Document is null)
            {
                return;
            }

            SelectedResource.Candidates.Remove(candidate);
            Document.ResourceCandidates.Remove(candidate.Candidate);
            if (ReferenceEquals(candidate, SelectedCandidate))
            {
                SelectedCandidate = SelectedResource.Candidates.FirstOrDefault();
            }

            MarkDirty();
        }

        private void SetRootFolder(StorageFolder folder)
        {
            ArgumentNullException.ThrowIfNull(folder);
            RootFolder = folder;
        }

        private async Task EmbedPathResourcesAsync()
        {
            if (Document is not null && RootFolder is not null)
            {
                await Document.ReplacePathCandidatesWithEmbeddedDataAsync(RootFolder);
                MarkDirty();
            }
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        [DynamicWindowsRuntimeCast(typeof(StorageFolder))]
        private async Task<StorageFile?> ResolvePathCandidateAsync(string fileName)
        {
            StorageFolder? root = RootFolder;
            if (root is null && CurrentFile is not null)
            {
                root = await CurrentFile.GetParentAsync();
            }

            if (root is null)
            {
                return null;
            }

            if (await root.TryGetItemAsync(fileName) is StorageFile file)
            {
                return file;
            }

            if (CurrentFile is not null &&
                await root.TryGetItemAsync(CurrentFile.DisplayName) is StorageFolder folder &&
                await folder.TryGetItemAsync(fileName) is StorageFile nestedFile)
            {
                return nestedFile;
            }

            return null;
        }

        private async Task<bool> EmbedPathCandidateAsync(CandidateItem candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            if (await ResolvePathCandidateAsync(candidate.StringValue) is not StorageFile file)
            {
                return false;
            }

            try
            {
                using IRandomAccessStream stream = await file.OpenAsync(
                    FileAccessMode.Read,
                    StorageOpenOptions.AllowReadersAndWriters);
                var buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size)
                {
                    Length = (uint)stream.Size,
                };
                await stream.ReadAsync(buffer, (uint)stream.Size, InputStreamOptions.None);
                candidate.DataValueBuffer = buffer;
                MarkDirty();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MarkDirty()
        {
            if (Document is not null)
            {
                _hasDocumentChanges = true;
                UpdateDirtyState();
            }
        }

        private void UpdateDirtyState()
        {
            IsDirty = _hasDocumentChanges || _candidateEdits.Values.Any(static edit => edit.IsDirty);
        }

        private ResourceItem GetOrAddResourceItem(string name)
        {
            return GetOrAddResourceItem(ResourceItems, name);
        }

        private static ObservableCollection<ResourceItem> BuildResourceTree(PriFile document)
        {
            ObservableCollection<ResourceItem> resourceItems = [];
            Dictionary<string, ResourceItem> knownItems = new(StringComparer.Ordinal);
            foreach (ResourceCandidate candidate in document.ResourceCandidates)
            {
                ResourceItem item = GetOrAddResourceItem(resourceItems, candidate.ResourceName, knownItems);
                item.Candidates.Add(candidate);
            }

            return resourceItems;
        }

        private static ResourceItem GetOrAddResourceItem(
            ObservableCollection<ResourceItem> resourceItems,
            string name,
            Dictionary<string, ResourceItem>? knownItems = null)
        {
            string[] split = name.SplitIntoResourceNames();
            ResourceItem? currentParent = null;
            foreach (string item in split)
            {
                ObservableCollection<ResourceItem> currentList = currentParent?.Children ?? resourceItems;
                currentParent = knownItems is not null
                    ? knownItems.GetValueOrDefault(item)
                    : currentList.FirstOrDefault(i => i.Name.Equals(item, StringComparison.Ordinal));
                if (currentParent is null)
                {
                    currentParent = new ResourceItem(item, currentList);
                    currentList.Add(currentParent);
                    knownItems?.Add(item, currentParent);
                }
            }

            return currentParent!;
        }

        private static void SortResourceTree(
            ObservableCollection<ResourceItem> items,
            bool preserveCollectionState = false)
        {
            foreach (ResourceItem item in items)
            {
                SortResourceTree(item.Children, preserveCollectionState);
            }

            List<ResourceItem> sortedItems = items.ToList();
            sortedItems.Sort(ResourceItemComparer);
            if (!preserveCollectionState)
            {
                items.Clear();
                foreach (ResourceItem item in sortedItems)
                {
                    items.Add(item);
                }

                return;
            }

            for (int targetIndex = 0; targetIndex < sortedItems.Count; targetIndex++)
            {
                int currentIndex = items.IndexOf(sortedItems[targetIndex]);
                if (currentIndex != targetIndex)
                {
                    items.Move(currentIndex, targetIndex);
                }
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

        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async Task TryLoadPri(StorageFile file)
        {
            try
            {
                if (await LoadAsync(file))
                {
                    _candidateEdits.Clear();
                    _displayedEdit = null;
                    UnloadAllPreviewElements();
                    Program.SetWindowTitle(file.Path);
                }
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
            if (await OpenFileHelper.PickAsync() is { } file)
            {
                await OpenFile(file);
            }
        }

        private async Task OpenFile(StorageFile file)
        {
            if (!await ConfirmDiscardChanges())
            {
                return;
            }

            if (OpenFileHelper.IsXbf(file.Name))
            {
                Frame.Navigate(typeof(XbfPage), file);
                return;
            }

            await TryLoadPri(file);
        }

        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async Task SavePri(StorageFile file)
        {
            try
            {
                List<CandidateEditState> appliedEdits = await ApplyCandidateEditsAsync();
                await SaveAsync(file);
                foreach (CandidateEditState edit in appliedEdits)
                {
                    edit.AcceptChanges();
                }

                _candidateEdits.Clear();
                _displayedEdit?.AcceptChanges();
                _subscribedEditor?.SetSavePoint();
                UpdateDirtyState();
                Program.SetWindowTitle(file.Path);
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
            if (CurrentFile is StorageFile file)
            {
                await SavePri(file);
            }
        }

        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            await SaveAsPri();
        }

        private async Task SaveAsPri()
        {
            FileSavePicker picker = new();
            picker.FileTypeChoices.Add("PRI File", new List<string>() { ".pri" });
            picker.SuggestedFileName = CurrentFile?.Name ?? "resources.pri";
            picker.Initialize();

            if (await picker.PickSaveFileAsync() is { } file)
            {
                await SavePri(file);
            }
        }

        private async void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (Document is null || IsBusy || args.VirtualKey != VirtualKey.S ||
                args.EventType is not (CoreAcceleratorKeyEventType.KeyDown or CoreAcceleratorKeyEventType.SystemKeyDown) ||
                !IsKeyDown(VirtualKey.Control))
            {
                return;
            }

            args.Handled = true;
            if (IsKeyDown(VirtualKey.Shift))
            {
                await SaveAsPri();
            }
            else if (CurrentFile is StorageFile file)
            {
                await SavePri(file);
            }
        }

        private static bool IsKeyDown(VirtualKey key)
        {
            return (Window.Current.CoreWindow.GetKeyState(key) & CoreVirtualKeyStates.Down) != 0;
        }

        private void UpdateWindowTitle()
        {
            if (CurrentFile is StorageFile file)
            {
                Program.SetWindowTitle(file.Path, IsDirty);
            }
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

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async void AddResource_Click(object sender, RoutedEventArgs e)
        {
            var parent = sender is MenuFlyoutItem item &&
                         item.DataContext is ResourceItem resourceItem ?
                resourceItem.Name :
                         treeView.SelectedItem is ResourceItem resItem && resItem.IsFolder ?
                resItem.Name : null;

            var dialog = new NewResourceDialog(Document!, parent);

            try
            {
                if (await dialog.ShowAsync() is { } candidate)
                {
                    AddResource(candidate);
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new()
                {
                    Title = "Error",
                    Content = $"Failed to create resource.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}\r\nStacktrace:\r\n\r\n{ex.StackTrace}",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    Template = (ControlTemplate)Program.Application.Resources["ScrollableContentDialogTemplate"]
                };

                await errorDialog.ShowAsync();
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void RemoveResources_Click(object sender, RoutedEventArgs e)
        {
            ResourceItem? resourceItem = sender is MenuFlyoutItem item
                ? item.DataContext as ResourceItem ?? SelectedResource
                : SelectedResource;
            if (Document is not null && resourceItem is not null)
            {
                bool removedSelection = ReferenceEquals(resourceItem, SelectedResource);
                RemoveResource(resourceItem);
                if (removedSelection)
                {
                    UnloadAllPreviewElements();
                }
            }
        }

        private async Task PickRootFolder()
        {
            FolderPicker picker = new();
            picker.FileTypeFilter.Add("*");
            picker.CommitButtonText = "Select PRI Root Folder";
            picker.Initialize();

            if (await picker.PickSingleFolderAsync() is { } folder)
            {
                SetRootFolder(folder);
            }
        }

        private async void SetRootFolder_Click(object sender, RoutedEventArgs e)
        {
            await PickRootFolder();

            if (RootFolder is not null &&
                SelectedCandidate is CandidateItem item &&
                item.Candidate.ValueType is ResourceValueType.Path)
            {
                await DisplayCandidate(item);
            }
        }

        private async void EmbedPathResources_Click(object sender, RoutedEventArgs e)
        {
            if (RootFolder is null)
            {
                await PickRootFolder();

                if (RootFolder is null)
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

            await EmbedPathResourcesAsync();
        }

        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (await ConfirmDiscardChanges())
            {
                Program.Exit();
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            var view = e.DataView;
            if (view.Contains(StandardDataFormats.StorageItems))
            {
                var path = view.GetFirstStorageItemPathUnsafe();
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
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0 && items[0] is StorageFile file && OpenFileHelper.IsSupported(file.Name))
                {
                    await OpenFile(file);
                    e.Handled = true;
                }
            }
        }

        private void TreeView_SelectionChanged(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewSelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count is 1 && args.AddedItems[0] is ResourceItem item)
            {
                SelectedResource = item;
            }
            else
            {
                SelectedResource = null;
                UnloadAllPreviewElements();
            }
        }

        private void UnloadOtherPreviewElements(CandidateItem item)
        {
            UnloadObject(invalidRootPathContainer);
            UnloadObject(failedToOpenFileContainer);
            UnloadObject(xbfFallbackContainer);

            if (SelectedResource?.Type.IsPreviewedAsText is not true)
                UnloadObject(valueTextEditor);

            if (SelectedResource?.Type is not ResourceType.Image)
                UnloadObject(imagePreviewerContainer);

            if (SelectedResource?.Type is ResourceType.Svg)
            {
                if (UseWebViewForSvg)
                {
                    UnloadObject(svgPreviewerContainer);
                }
                else
                {
                    UnloadObject(webView);
                }
            }
            else
            {
                UnloadObject(svgPreviewerContainer);
                UnloadObject(webView);
            }

            if (!(item.ValueType is ResourceValueType.EmbeddedData && SelectedResource?.Type.IsPreviewable is not true))
                UnloadObject(exportContainer);

            if (!(item.ValueType is ResourceValueType.Path && SelectedResource?.Type.IsPreviewable is not true))
                UnloadObject(openFolderContainer);
        }

        private void UnloadAllPreviewElements()
        {
            _displayedEdit = null;
            UnloadObject(invalidRootPathContainer);
            UnloadObject(failedToOpenFileContainer);
            UnloadObject(xbfFallbackContainer);
            UnloadObject(valueTextEditor);
            UnloadObject(imagePreviewerContainer);
            UnloadObject(svgPreviewerContainer);
            UnloadObject(webView);
            UnloadObject(exportContainer);
            UnloadObject(openFolderContainer);
        }

        private void UnloadNonErrorPreviewElements()
        {
            _displayedEdit = null;
            UnloadObject(valueTextEditor);
            UnloadObject(imagePreviewerContainer);
            UnloadObject(svgPreviewerContainer);
            UnloadObject(webView);
            UnloadObject(exportContainer);
            UnloadObject(openFolderContainer);
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        private async Task DisplayCandidate(CandidateItem item)
        {
            UnloadOtherPreviewElements(item);
            _displayedEdit = null;

            if (_candidateEdits.TryGetValue(item, out CandidateEditState? edit))
            {
                if (edit.XbfVersion?.Major == 2)
                {
                    edit.Dialect = Xbf2Dialect;
                }

                DisplayStringCandidate(edit);
                return;
            }

            var candidate = item.Candidate;
            if (candidate.ValueType is ResourceValueType.Path)
            {
                if (await ResolvePathCandidateAsync(candidate.StringValue) is StorageFile file)
                {
                    await DisplayPathCandidate(item, file);
                    return;
                }

                UnloadNonErrorPreviewElements();
                FindName(nameof(invalidRootPathContainer));
            }
            else if (candidate.ValueType is ResourceValueType.EmbeddedData)
            {
                var dataValue = candidate.DataValueReference;
                using (RandomAccessStreamOverBuffer stream = new(dataValue))
                {
                    if (await DisplayBinaryCandidate(item, stream, SelectedResource!.Type))
                        return;
                }

                FindName(nameof(exportContainer));
                fileSizeLabel.Text = $"File Size: {dataValue.Length} bytes";
            }
            else
            {
                DisplayStringCandidate(new CandidateEditState(item, SelectedResource!.Type, candidate.StringValue, null, Xbf2Dialect));
            }
        }

        private void DisplayStringCandidate(CandidateEditState edit)
        {
            FindName(nameof(valueTextEditor));

            var editor = valueTextEditor.Editor;
            SubscribeToEditor(editor);
            _displayedEdit = edit;
            _settingEditorText = true;
            try
            {
                editor.ReadOnly = false;
                editor.WrapMode = Wrap.Word;
                editor.CaretStyle = CaretStyle.Line;
                editor.SetText(edit.Text);
                valueTextEditor.ApplyDefaultsToDocument();
                valueTextEditor.HighlightingLanguage = edit.ResourceType is ResourceType.Xaml or ResourceType.Xbf
                    ? "xml"
                    : edit.Candidate.Candidate.ResourceName.GetDisplayName().GetExtensionAfterPeriod().ToScintillaLanguage();
                editor.SetSavePoint();
            }
            finally
            {
                _settingEditorText = false;
            }
        }

        private void SubscribeToEditor(Editor editor)
        {
            if (ReferenceEquals(_subscribedEditor, editor))
            {
                return;
            }

            if (_subscribedEditor is not null)
            {
                _subscribedEditor.Modified -= Editor_Modified;
            }

            _subscribedEditor = editor;
            editor.Modified += Editor_Modified;
        }

        private void Editor_Modified(Editor sender, ModifiedEventArgs args)
        {
            if (_settingEditorText || _displayedEdit is null ||
                (args.ModificationType & (int)(ModificationFlags.InsertText | ModificationFlags.DeleteText)) == 0)
            {
                return;
            }

            _displayedEdit.Text = valueTextEditor.Text;
            if (_displayedEdit.IsDirty)
            {
                _candidateEdits[_displayedEdit.Candidate] = _displayedEdit;
            }
            else
            {
                _candidateEdits.Remove(_displayedEdit.Candidate);
            }

            UpdateDirtyState();
        }

        private async Task<List<CandidateEditState>> ApplyCandidateEditsAsync()
        {
            List<CandidateEditState> edits = [];
            foreach (CandidateEditState edit in _candidateEdits.Values)
            {
                if (edit.IsDirty)
                {
                    edits.Add(edit);
                }
            }

            List<PreparedCandidateEdit> prepared = await Task.Run(() =>
            {
                List<PreparedCandidateEdit> result = new(edits.Count);
                foreach (CandidateEditState edit in edits)
                {
                    result.Add(PrepareCandidateEdit(edit));
                }

                return result;
            });
            foreach (PreparedCandidateEdit item in prepared)
            {
                if (item.Value is string text)
                {
                    item.Edit.Candidate.StringValue = text;
                }
                else
                {
                    item.Edit.Candidate.SetValue((byte[])item.Value);
                }
            }

            return edits;
        }

        private static PreparedCandidateEdit PrepareCandidateEdit(CandidateEditState edit)
        {
            if (edit.XbfVersion is XbfVersion version)
            {
                try
                {
                    using MemoryStream output = new();
                    XbfDialect dialect = version.Major == 1 ? XbfDialect.WUX : edit.Dialect;
                    XbfCompiler.Compile(edit.Text, output, version, dialect);
                    return new PreparedCandidateEdit(edit, output.ToArray());
                }
                catch (Exception ex)
                {
                    ThrowHelpers.ThrowInvalidDataException($"Failed to compile the edited XBF resource '{edit.Candidate.Candidate.ResourceName}'.", ex);
                }
            }

            object value = edit.OriginalValueType == ResourceValueType.String
                ? edit.Text
                : Encoding.UTF8.GetBytes(edit.Text);
            return new PreparedCandidateEdit(edit, value);
        }

        private async Task<bool> DisplayBinaryCandidate(CandidateItem item, IRandomAccessStream stream, ResourceType type)
        {
            try
            {
                if (type is ResourceType.Image)
                {
                    BitmapImage image = new();
                    await image.SetSourceAsync(stream);
                    FindName(nameof(imagePreviewerContainer));
                    imagePreviewer.Opacity = 0;
                    imagePreviewer.Source = image;

                    imagePreviewer.Stretch = Stretch.None;
                    imagePreviewer.MaxWidth = double.PositiveInfinity;
                    imagePreviewer.MaxHeight = double.PositiveInfinity;

                    imagePreviewerContainer.UpdateLayout();
                    imagePreviewerContainer.ChangeView(null, null, 1f, true);

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
                            imagePreviewerContainer.UpdateLayout();

                            ratio = 0.1d;
                        }

                        if (imagePreviewerContainer.ZoomFactor is not 1f)
                        {
                            await imagePreviewerContainer.WaitForZoomFactorChangeAsync();
                        }

                        imagePreviewerContainer.ChangeView(null, null, (float)ratio, true);
                    }

                    imagePreviewer.Opacity = 1;
                    return true;
                }
                else if (type is ResourceType.Xbf)
                {
                    try
                    {
                        XbfDocument document = XbfDocumentReader.Read(stream.AsStream());
                        XbfDialect dialect = document.Version.Major == 1
                            ? XbfDialect.WUX
                            : Xbf2Dialect;
                        XbfDecompilationOptions options = new()
                        {
                            IncludeConnectionIds = IncludeConnectionIds,
                        };
                        DisplayStringCandidate(new CandidateEditState(item, type, XbfDecompiler.Decompile(document, dialect, options), document.Version, dialect));
                    }
                    catch (Exception ex)
                    {
                        UnloadNonErrorPreviewElements();
                        FindName(nameof(xbfFallbackContainer));

                        xbfFileNameRun.Text = SelectedResource is not null ?
                            SelectedResource.DisplayName :
                            "the XBF file";

                        failedXbfExceptionMessageRun.Text = $"{ex.GetType().Name} (0x{ex.HResult:X8}) -> {ex.Message}";
                        xbfFallbackContainer.Visibility = Visibility.Visible;
                    }

                    return true;
                }
                else if (type.IsText)
                {
                    if (stream is RandomAccessStreamOverBuffer rasob)
                    {
                        unsafe
                        {
#pragma warning disable CS9123 // The '&' operator should not be used on parameters or local variables in async methods.
                            byte* ptr = default;
                            rasob.BufferByteAccess->Buffer(&ptr);
#pragma warning restore CS9123 // The '&' operator should not be used on parameters or local variables in async methods.

                            if (ptr is not null)
                            {
                                DisplayStringCandidate(new CandidateEditState(item, type, Encoding.UTF8.GetString(ptr, (int)rasob.Size), null, Xbf2Dialect));
                                return true;
                            }
                        }
                    }
                    else
                    {
                        var size = (uint)stream.Size;
                        using var buffer = new NativeBuffer(size);
                        await stream.ReadAsync(buffer, size, InputStreamOptions.None);

                        unsafe
                        {
                            DisplayStringCandidate(new CandidateEditState(item, type, Encoding.UTF8.GetString(buffer.Buffer, (int)size), null, Xbf2Dialect));
                            return true;
                        }
                    }
                }
                else if (type is ResourceType.Svg)
                {
                    bool succeeded = false;

                    if (!UseWebViewForSvg)
                    {
                        if (Features.IsCompositionRadialGradientBrushAvailable)
                        {
                            var size = (uint)stream.Size;
                            using var buffer = new NativeBuffer(size + 1);
                            await stream.ReadAsync(buffer, size, InputStreamOptions.None);

                            unsafe
                            {
                                var nativeBuffer = buffer.Buffer;
                                nativeBuffer[size] = 0;

                                var parse = NanoSVG.NanoSVG.nsvgParse(nativeBuffer, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("px"u8)), 96);

                                if (parse != null)
                                {
                                    if (Features.IsCompositionRadialGradientBrushAvailable)
                                    {
                                        Compositor compositor = Window.Current.Compositor;

                                        ShapeVisual visual = compositor.CreateShapeVisual();
                                        visual.Shapes.Add(compositor.CreateShapeFromNSVGImage(parse));
                                        visual.RelativeSizeAdjustment = Vector2.One;

                                        FindName(nameof(svgPreviewerContainer));

                                        svgPreviewer.Width = parse->width;
                                        svgPreviewer.Height = parse->height;
                                        ElementCompositionPreview.SetElementChildVisual(svgPreviewer, visual);

                                        succeeded = true;
                                    }
                                }

                                NanoSVG.NanoSVG.nsvgDelete(parse);
                            }
                        }
                        else
                        {
                            SvgImageSource source = new()
                            {
                                RasterizePixelWidth = 1024,
                                RasterizePixelHeight = 1024
                            };

                            if (await source.SetSourceAsync(stream) is SvgImageSourceLoadStatus.Success)
                            {
                                Viewbox viewbox = new()
                                {
                                    Stretch = Stretch.Uniform,
                                    Width = Math.Min(512, PreviewContainer.ActualWidth - 20),
                                    Height = Math.Min(512, PreviewContainer.ActualHeight - 20),
                                    Child = new Image()
                                    {
                                        Source = source,
                                        Width = 1024,
                                        Height = 1024,
                                        Stretch = Stretch.None
                                    }
                                };

                                FindName(nameof(svgPreviewerContainer));
                                svgPreviewer.Content = viewbox;
                                svgPreviewer.VerticalAlignment = VerticalAlignment.Center;
                                svgPreviewer.HorizontalAlignment = HorizontalAlignment.Center;

                                succeeded = true;
                            }
                        }
                    }

                    if (!succeeded && Program.IsWebViewAvailable)
                    {
                        UnloadObject(svgPreviewerContainer);

                        var size = (uint)stream.Size;
                        using var buffer = new NativeBuffer(size);

                        stream.Seek(0);
                        await stream.ReadAsync(buffer, size, InputStreamOptions.None);

                        unsafe
                        {
                            var svg = Encoding.UTF8.GetString(buffer.Buffer, (int)size);
                            var html = @$"
                            <html>
                                <head>
                                <meta charset=""UTF-16"">
                                <style>
                                    html, body {{
                                    width: 100%;
                                    height: 100%;
                                    margin: 0;
                                    }}
                                </style>
                                </head>
                                <body style=""display:flex;justify-content:center;align-items:center;background-color:transparent;"">
                                <div>{svg}</div>
                                </body>
                            </html>";

                            FindName(nameof(webView));
                            webView.NavigateToString(html);

                            succeeded = true;
                        }
                    }

                    return succeeded;
                }
            } catch { }

            return false;
        }

        private async Task DisplayPathCandidate(CandidateItem item, StorageFile file)
        {
            bool result = false;
            IRandomAccessStream stream;

            if (SelectedResource?.Type.IsPreviewable is true)
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

                result = await DisplayBinaryCandidate(item, stream, SelectedResource.Type);
                stream.Dispose();
            }

            if (!result)
            {
                FindName(nameof(openFolderContainer));
                openFolderContainer.Tag = file.Path;
            }
        }

        private async void CandidatesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count is 1 && e.AddedItems[0] is CandidateItem item)
            {
                SelectedCandidate = item;
                await DisplayCandidate(item);
            }
            else
            {
                SelectedCandidate = null;
                UnloadAllPreviewElements();
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var candidate = sender is MenuFlyoutItem item && item.DataContext is CandidateItem candidateItem ?
                candidateItem.Candidate : (candidatesList.SelectedItem as CandidateItem)?.Candidate;

            if (candidate is not null)
            {
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
                {
                    if (candidate.ValueType is ResourceValueType.EmbeddedData)
                        await FileIO.WriteBufferAsync(file, candidate.DataValueReference);
                    else
                        await FileIO.WriteTextAsync(file, candidate.StringValue, UnicodeEncoding.Utf8);
                }
            }
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

        private async void SystemXaml_Click(object sender, RoutedEventArgs e)
        {
            Xbf2Dialect = XbfDialect.WUX;
            if (SelectedCandidate is CandidateItem item &&
                SelectedResource?.Type is ResourceType.Xbf)
            {
                await DisplayCandidate(item);
            }
        }

        private async void WinUIXaml_Click(object sender, RoutedEventArgs e)
        {
            Xbf2Dialect = XbfDialect.MUX;
            if (SelectedCandidate is CandidateItem item &&
                SelectedResource?.Type is ResourceType.Xbf)
            {
                await DisplayCandidate(item);
            }
        }

        private async void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCandidate is CandidateItem item)
            {
                await DisplayCandidate(item);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var path = sender is MenuFlyoutItem item &&
                       item.DataContext is CandidateItem candidateItem &&
                       await ResolvePathCandidateAsync(candidateItem.StringValue) is { } file ?
                file.Path : openFolderContainer?.Tag as string;

            if (path is not null)
            {
                NativeUtils.ShowFileInExplorer(path);
            }
        }

        private async void Notice_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NoticeDialog();
            await dialog.ShowAsync();
        }

        private unsafe void SyntaxHighlightingApplied(object sender, ElementTheme e)
        {
            valueTextEditor.HandleSyntaxHighlightingApplied(e);
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void DeleteCandidate_Click(object sender, RoutedEventArgs e)
        {
            if (Document is not null &&
                SelectedResource is not null &&
                sender is MenuFlyoutItem item &&
                item.DataContext is CandidateItem candidateItem)
            {
                _candidateEdits.Remove(candidateItem);
                if (ReferenceEquals(_displayedEdit?.Candidate, candidateItem))
                {
                    _displayedEdit = null;
                }

                DeleteCandidate(candidateItem);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void EmbedPathCandidate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item &&
                item.DataContext is CandidateItem candidateItem)
            {
                await EmbedPathCandidateAsync(candidateItem);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void SimpleRename_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item &&
                item.DataContext is ResourceItem resourceItem)
            {
                string originalName = resourceItem.Name;
                var dialog = new RenameDialog(resourceItem, true);
                await dialog.ShowAsync();
                if (!string.Equals(resourceItem.Name, originalName, StringComparison.Ordinal))
                {
                    SortResourceItems();
                    MarkDirty();
                }
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void FullRename_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item &&
                item.DataContext is ResourceItem resourceItem)
            {
                string originalName = resourceItem.Name;
                var dialog = new RenameDialog(resourceItem, false);
                await dialog.ShowAsync();
                if (!string.Equals(resourceItem.Name, originalName, StringComparison.Ordinal))
                {
                    ReparentRenamedResource(resourceItem);
                }
            }
        }

        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void CreateOrModifyCandidate_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedResource is not null && Document is not null)
            {
                if (sender is MenuFlyoutItem item &&
                    item.DataContext is CandidateItem candidateItem)
                {
                    var dialog = new CreateOrModifyCandidateDialog(SelectedResource, candidateItem);

                    if (await dialog.ShowAsync() is not null)
                    {
                        MarkDirty();
                        await DisplayCandidate(candidateItem);
                    }
                }
                else
                {
                    var dialog = new CreateOrModifyCandidateDialog(SelectedResource);

                    if (await dialog.ShowAsync() is { } candidate)
                    {
                        AddCandidate(candidate);
                    }
                }
            }
        }

        [DynamicWindowsRuntimeCast(typeof(ToggleMenuFlyoutItem))]
        private async void IncludeConnectionIds_Click(object sender, RoutedEventArgs e)
        {
            IncludeConnectionIds = ((ToggleMenuFlyoutItem)sender).IsChecked;
            if (SelectedResource?.Type is ResourceType.Xbf &&
                SelectedCandidate is CandidateItem item)
            {
                await DisplayCandidate(item);
            }
        }

        [DynamicWindowsRuntimeCast(typeof(ToggleMenuFlyoutItem))]
        private void SortResources_Click(object sender, RoutedEventArgs e)
        {
            SortResources = ((ToggleMenuFlyoutItem)sender).IsChecked;
        }

        [DynamicWindowsRuntimeCast(typeof(ToggleMenuFlyoutItem))]
        private async void UseWebViewForSvg_Click(object sender, RoutedEventArgs e)
        {
            UseWebViewForSvg = ((ToggleMenuFlyoutItem)sender).IsChecked;
            if (SelectedResource?.Type is ResourceType.Svg &&
                SelectedCandidate is CandidateItem item)
            {
                await DisplayCandidate(item);
            }
        }
    }
}
