using MrmTool.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Selectors
{
    internal partial class ResourceItemTemplateSelector : DataTemplateSelector
    {
        public ResourceItemTemplateSelector() { }

        public DataTemplate? ResourceTree { get; set; }

        public DataTemplate? Resource { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            var resourceItem = (ResourceItem)item;
            resourceItem.EnsureIconAndType();

            if (resourceItem.Children.Count > 0)
            {
                return ResourceTree;
            }
            else
            {
                return Resource;
            }
        }
    }
}
