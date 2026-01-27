using MrmTool.Models;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Dialogs
{
    public sealed partial class RenameDialog : ContentDialog
    {
        private readonly ResourceItem _item;
        private readonly bool _simpleRename;

        public RenameDialog(ResourceItem item, bool simpleRename = true)
        {
            _item = item;
            _simpleRename = simpleRename;

            this.InitializeComponent();

            nameBox.Text = simpleRename ? item.DisplayName : item.Name;
        }

        internal new async Task ShowAsync()
        {
            if (await base.ShowAsync() is ContentDialogResult.Primary)
            {
                if (_simpleRename)
                    _item.DisplayName = nameBox.Text;
                else
                    _item.Name = nameBox.Text;
            }
        }

        private void ValidateInput()
        {
            var name = nameBox.Text;
            IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(name) &&
                                     (_simpleRename ? !name.Contains('/', StringComparison.Ordinal) :
                                     !name.StartsWith('/') &&
                                     !name.EndsWith('/'));
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }
    }
}
