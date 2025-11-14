using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRT;

namespace Microsoft.UI.Xaml.Controls
{
    [GeneratedBindableCustomProperty]
    internal partial class TreeViewItemWrapper : ContentControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets an object source used to generate the content of the TreeView.
        /// </summary>
        public object ItemsSource
        {
            get => (object)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static DependencyProperty ItemsSourceProperty { get; } =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(TreeViewItem), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs args)
        {
            var wrapper = (TreeViewItemWrapper)sender;
            wrapper.PropertyChanged?.Invoke(wrapper, new(nameof(ItemsSource)));
        }
    }
}
