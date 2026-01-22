using Windows.UI.Xaml.Resources;

namespace MrmTool.Resources
{
    internal partial class XamlResourceLoader : CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            return Program.ResourceMap[$"Strings/{resourceId}"].Resolve().ValueAsString;
        }
    }
}
