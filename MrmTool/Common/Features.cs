using System.Runtime.Versioning;
using Windows.Foundation.Metadata;
using System.Diagnostics.CodeAnalysis;

namespace Common
{
    [SuppressMessage("Interoperability", "CA1416")]
    internal class Features
    {
        [SupportedOSPlatformGuard("windows10.0.18362.0")]
        public static readonly bool IsXamlRootAvailable = ApiInformation.IsTypePresent("Windows.UI.Xaml.XamlRoot");

        [SupportedOSPlatformGuard("windows10.0.17763.0")]
        public static readonly bool IsFlyoutShowOptionsAvailable = ApiInformation.IsTypePresent("Windows.UI.Xaml.Controls.Primitives.FlyoutShowOptions");

        [SupportedOSPlatformGuard("windows10.0.18362.0")]
        public static readonly bool IsCompositionRadialGradientBrushAvailable = ApiInformation.IsTypePresent("Windows.UI.Composition.CompositionRadialGradientBrush");
        
        [SupportedOSPlatformGuard("windows10.0.16299.0")]
        public static readonly bool IsColumnSpacingAvailable = ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.Grid", "ColumnSpacing");
        
        [SupportedOSPlatformGuard("windows10.0.18362.0")]
        public static readonly bool IsShouldConstrainToRootBoundsAvailable = ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.Primitives.FlyoutBase", "ShouldConstrainToRootBounds");

#if COREDISPATCHER_FALLBACK
        [SupportedOSPlatformGuard("windows10.0.16299.0")]
        public static readonly bool IsDispatcherQueueSupported = ApiInformation.IsTypePresent("Windows.System.DispatcherQueue");
#endif
    }
}
