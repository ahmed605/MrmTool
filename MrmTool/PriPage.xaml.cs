using MrmLib;
using MrmTool.Models;
using System.Collections.ObjectModel;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRT;

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

        public ObservableCollection<ResourceItem> ResourceItems { get; } = [];

        public PriPage()
        {
            this.InitializeComponent();
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

        private void Clear()
        {
            ResourceItems.Clear();
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

        }

        private void RemoveResources_Click(object sender, RoutedEventArgs e)
        {

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
            TerraFX.Interop.Windows.Windows.SendMessageW(Program.WindowHandle, TerraFX.Interop.Windows.WM.WM_CLOSE, 0, 0);
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.Handled = true;
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
                }
            }
        }

        private void treeView_SelectionChanged(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewSelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count is 1 && args.AddedItems[0] is ResourceItem item && item.Candidates.Count > 0)
            {
                candidatesList.ItemsSource = item.Candidates;
            }
        }

        private void UnloadPreviewElements()
        {
            UnloadObject(invalidRootPathContainer);
            UnloadObject(valueTextBlock);
            UnloadObject(exportContainer);
            UnloadObject(imagePreviewer);
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        [DynamicWindowsRuntimeCast(typeof(StorageFolder))]
        private async Task DisplayCandidate(CandidateItem item)
        {
            UnloadPreviewElements();

            var candidate = item.Candidate;
            if (candidate.ValueType is ResourceValueType.Path)
            {
                StorageFile? file = null;

                var root = _rootFolder ?? await _currentFile?.GetParentAsync();
                if (await root.TryGetItemAsync(candidate.StringValue) is StorageFile cFile)
                {
                    file = cFile;
                }
                else
                {
                    if (await root.TryGetItemAsync(_currentFile?.DisplayName) is StorageFolder folder)
                    {
                        file = await folder.TryGetItemAsync(candidate.StringValue) as StorageFile;
                    }
                }

                if (file is null)
                {
                    FindName("invalidRootPathContainer");
                    return;
                }

                // TODO: display file
                ResourceType type = ResourceItem.DetermineFileType(candidate.ResourceName);
                if (type == ResourceType.Image)
                {
                    using IRandomAccessStream stream = await file.OpenReadAsync();
                    BitmapImage image = new();
                    await image.SetSourceAsync(stream);
                    FindName("imagePreviewer");
                    imagePreviewer.Source = image;
                }
            }
            else if (candidate.ValueType is ResourceValueType.EmbeddedData)
            {
                // TODO: Detect the file type
                FindName("exportContainer");
                fileSizeLabel.Text = $"File Size: {candidate.DataValue.Length} bytes";
            }
            else
            {
                FindName("valueTextBlock");
                valueTextBlock.Text = candidate.StringValue;
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
                UnloadPreviewElements();
            }
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceCandidate candidate = ((CandidateItem)candidatesList.SelectedItem).Candidate;
            byte[] data = candidate.ValueType is ResourceValueType.EmbeddedData ? candidate.DataValue : Encoding.UTF8.GetBytes(candidate.StringValue);

            string resourceName = candidate.ResourceName;
            int fileNameIndex = resourceName.LastIndexOf('/') + 1;

            string fileName = resourceName[fileNameIndex..];
            string extension = Path.GetExtension(fileName);

            FileSavePicker picker = new();
            picker.Initialize();
            picker.SuggestedFileName = fileName;

            if (!string.IsNullOrEmpty(extension))
            {
                picker.FileTypeChoices.Add($"{extension[1..].ToUpper()} file", new string[] { extension });
            }

            picker.FileTypeChoices.Add("All files", new string[] { "." });

            StorageFile file = await picker.PickSaveFileAsync();
            if (file is not null)
                await FileIO.WriteBytesAsync(file, data);
        }
    }
}
