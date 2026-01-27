using MrmTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Dialogs
{
    public sealed partial class RenameDialog : ContentDialog
    {
        private readonly ResourceItem _item;

        public RenameDialog(ResourceItem item, bool simpleRename = true)
        {
            _item = item;

            if (!simpleRename)
            {
                ThrowNotSupportedException();
            }

            this.InitializeComponent();

            nameBox.Text = item.DisplayName;
        }

        [DoesNotReturn]
        private static void ThrowNotSupportedException()
            => throw new NotSupportedException("Full rename is not supported yet");

        internal new async Task ShowAsync()
        {
            if (await base.ShowAsync() is ContentDialogResult.Primary)
            {
                // TODO: handle full rename
                _item.DisplayName = nameBox.Text;
            }
        }

        private void ValidateInput()
        {
            // TODO: handle full rename validation
            var name = nameBox.Text;
            IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(name) &&
                                     !name.Contains('/', StringComparison.Ordinal);
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }
    }
}
