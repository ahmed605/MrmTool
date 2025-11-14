using Windows.Foundation.Metadata;

namespace Common
{
    internal class Features
    {
        public static readonly bool IsXamlRootAvailable = ApiInformation.IsTypePresent("Windows.UI.Xaml.XamlRoot");
    }
}
