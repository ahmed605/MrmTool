using MrmLib;
using MrmTool.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MrmTool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PriPage : Page
    {
        private PriFile? _pri;
        private StorageFile? _currentFile;

        public ObservableCollection<ResourceItem> ResourceItems { get; } = [];

        public PriPage()
        {
            this.InitializeComponent();
        }

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

        private static ResourceItem? FindResourceItem(ObservableCollection<ResourceItem> items, string name)
        {
            foreach (var i in items)
            {
                if (i.Name == name) return i;

                var found = FindResourceItem(i.Children, name);
                if (found is not null) return found;
            }

            return null;
        }

        private ResourceItem GetOrAddResourceItem(string name)
        {
            var item = FindResourceItem(ResourceItems, name);
            if (item is not null) return item;

            var parentName = name.GetParentName();
            if (parentName is null)
            {
                item = new ResourceItem(name);
                ResourceItems.Add(item);
            }
            else
            {
                var parentItem = GetOrAddResourceItem(parentName);
                item = new ResourceItem(name);
                parentItem.Children.Add(item);
            }

            return item;
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

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new();
            picker.FileTypeFilter.Add(".pri");
            picker.CommitButtonText = "Load";
            picker.Initialize();

            if (await picker.PickSingleFileAsync() is { } file)
            {
                try
                {
                    LoadPri(await PriFile.LoadAsync(file));
                    _currentFile = file;
                }
                catch (Exception ex)
                {
                    ContentDialog dialog = new()
                    {
                        Title = "Error",
                        Content = $"Failed to load the selected PRI file.\r\nException: {ex.GetType().Name} (0x{ex.HResult:X8})\r\nException Message: {ex.Message}\r\nStacktrace:\r\n\r\n{ex.StackTrace}",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot,
                        Template = (ControlTemplate)Application.Current.Resources["ScrollableContentDialogTemplate"]
                    };

                    await dialog.ShowAsync();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddResource_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveResources_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetRootFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EmbedPathResources_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            TerraFX.Interop.Windows.Windows.SendMessageW(((App)App.Current).HWND, TerraFX.Interop.Windows.WM.WM_CLOSE, 0, 0);
        }
    }
}
