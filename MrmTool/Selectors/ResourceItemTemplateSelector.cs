using MrmTool.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Selectors
{
    internal partial class ResourceItemTemplateSelector : DataTemplateSelector
    {
        public ResourceItemTemplateSelector() { }

        public DataTemplate? Resource { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            if (item is ResourceItem resourceItem)
                resourceItem.EnsureIconAndType();

            return Resource;
        }
    }
}
