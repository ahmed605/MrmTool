using Windows.System;
using Windows.UI.Core;
using Windows.Foundation.Metadata;

using DispatcherQueueSynchronizationContext = Microsoft.System.DispatcherQueueSynchronizationContext;

namespace Microsoft.System
{
    internal static class DispatcherHelper
    {
        private static readonly bool IsDispatcherQueueSupported = ApiInformation.IsTypePresent("Windows.System.DispatcherQueue");

        internal static void SetSynchronizationContext()
        {
            if (IsDispatcherQueueSupported)
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                return;
            }

            SynchronizationContext.SetSynchronizationContext(new CoreDispatcherSynchronizationContext(CoreWindow.GetForCurrentThread().Dispatcher));
        }

        /*internal static void RunOnDispatcher(Action action, DispatcherQueue? queue = null, CoreDispatcher? dispatcher = null)
        {
            if (IsDispatcherQueueSupported)
            {
                queue ??= DispatcherQueue.GetForCurrentThread();
                queue.TryEnqueue(() => action());
                return;
            }

            dispatcher ??= CoreWindow.GetForCurrentThread().Dispatcher;
            _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }*/
    }
}
