using Common;
using Windows.System;
using Windows.UI.Core;
using System.Runtime.Versioning;
using Windows.Foundation.Metadata;

using DispatcherQueueSynchronizationContext = Microsoft.System.DispatcherQueueSynchronizationContext;

namespace Microsoft.System
{
    internal static class DispatcherHelper
    {
        internal static void SetSynchronizationContext()
        {
#if COREDISPATCHER_FALLBACK
            if (Features.IsDispatcherQueueSupported)
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
