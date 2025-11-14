using Windows.System;
using Windows.UI.Core;
using Windows.Foundation.Metadata;

namespace Microsoft.System
{
    internal static class DispatcherHelper
    {
#if COREDISPATCHER_FALLBACK
        private static readonly bool IsDispatcherQueueSupported = ApiInformation.IsTypePresent("Windows.System.DispatcherQueue");
#endif

        internal static void SetSynchronizationContext()
        {
#if COREDISPATCHER_FALLBACK
            if (IsDispatcherQueueSupported)
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                return;
            }

            SynchronizationContext.SetSynchronizationContext(new CoreDispatcherSynchronizationContext(CoreWindow.GetForCurrentThread().Dispatcher));
#else
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
#endif
        }
    }
}
