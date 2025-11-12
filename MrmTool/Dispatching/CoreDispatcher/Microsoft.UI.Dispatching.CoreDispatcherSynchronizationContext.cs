using System;
using TerraFX.Interop.WinRT;
using Windows.System;
using Windows.UI.Core;
using WinRT;
using CoreDispatcherPriority = TerraFX.Interop.WinRT.CoreDispatcherPriority;

#nullable enable

// CA1416 is "validate platform compatibility". This suppresses the warnings for IWinRTObject.NativeObject and
// IObjectReference.ThisPtr only being supported on Windows (since WASDK as a whole only works on Windows anyway).
#pragma warning disable CA1416

namespace Microsoft.System
{
    /// <summary>
    /// DispatcherQueueSyncContext allows developers to await calls and get back onto the
    /// UI thread. Needs to be installed on the UI thread through DispatcherQueueSyncContext.SetForCurrentThread
    /// </summary>
    public partial class CoreDispatcherSynchronizationContext : SynchronizationContext
    {
        private readonly CoreDispatcher m_coreDispatcher;

        public CoreDispatcherSynchronizationContext(CoreDispatcher coreDispatcher)
        {
            m_coreDispatcher = coreDispatcher;
        }

        /// <inheritdoc/>
        public override unsafe void Post(SendOrPostCallback d, object? state)
        {
            if (d is null)
            {
                throw new ArgumentNullException(nameof(d));
            }

#if NET5_0_OR_GREATER
            IAsyncAction* action = default;
            IDispatchedHandler* dispatcherQueueProxyHandler = (IDispatchedHandler*)CoreDispatcherProxyHandler.Create(d, state);
            int hResult;

            try
            {
                ICoreDispatcher* dispatcherQueue = (ICoreDispatcher*)((IWinRTObject)m_coreDispatcher).NativeObject.ThisPtr;
                
                hResult = dispatcherQueue->RunAsync(CoreDispatcherPriority.CoreDispatcherPriority_Normal, dispatcherQueueProxyHandler, &action);
                GC.KeepAlive(this);
            }
            finally
            {
                dispatcherQueueProxyHandler->Release();
            }

            if (hResult != 0)
            {
                ExceptionHelpers.ThrowExceptionForHR(hResult);
            }

            action->Release();
#else
            _ = m_coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => d!(state));
#endif
        }

        /// <inheritdoc/>
        public override void Send(SendOrPostCallback d, object? state)
        {
            throw new NotSupportedException("Send not supported");
        }

        /// <inheritdoc/>
        public override SynchronizationContext CreateCopy()
        {
            return new CoreDispatcherSynchronizationContext(m_coreDispatcher);
        }
    }
}