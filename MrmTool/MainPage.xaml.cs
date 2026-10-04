using WinRT;
using MrmLib;
using MrmTool.Common;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MrmTool.Dialogs;

namespace MrmTool
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OnOpenFileClicked(object sender, RoutedEventArgs e)
        {
            if (await OpenFileHelper.PickAsync() is { } file)
            {
                await OpenFile(file);
            }
        }

        private void MainGrid_DragOver(object sender, DragEventArgs e)
        {
            var view = e.DataView;
            if (view.Contains(StandardDataFormats.StorageItems))
            {
                var path = view.GetFirstStorageItemPathUnsafe();
                if (path is null || OpenFileHelper.IsSupported(path))
                {
                    e.AcceptedOperation = DataPackageOperation.Copy;
                    e.DragUIOverride.Caption = "Drop to load the PRI or XBF file";
                    e.Handled = true;
                }
                else
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                }
            }
        }

        [DynamicWindowsRuntimeCast(typeof(StorageFile))]
        private async void MainGrid_Drop(object sender, DragEventArgs e)
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

        [DynamicWindowsRuntimeCast(typeof(ControlTemplate))]
        private async Task OpenFile(StorageFile file)
        {
            if (OpenFileHelper.IsXbf(file.Name))
            {
                Frame.Navigate(typeof(XbfPage), file);
                return;
            }

            try
            {
                var pri = await PriFile.LoadAsync(file);
                Frame.Navigate(typeof(PriPage), (pri, file));
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

        private async void NoticeButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new NoticeDialog();
            await dialog.ShowAsync();
        }
    }
}
