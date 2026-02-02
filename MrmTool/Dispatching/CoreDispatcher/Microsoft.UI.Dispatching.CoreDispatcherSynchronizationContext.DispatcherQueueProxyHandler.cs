#if COREDISPATCHER_FALLBACK

#if NET5_0_OR_GREATER

using WinRT;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable CA1416

namespace Microsoft.System
{
    /// <inheritdoc/>
    public partial class CoreDispatcherSynchronizationContext
    {
        /// <summary>
        /// A custom <c>IDispatchedHandler</c> object, that internally stores a captured <see cref="SendOrPostCallback"/> instance and the
        /// input captured state. This allows consumers to enqueue a state and a cached stateless delegate without any managed allocations.
        /// </summary>
        private unsafe struct CoreDispatcherProxyHandler
        {
            /// <summary>
            /// The vtable pointer for the current instance.
            /// </summary>
            private Impl.Vtable* vtbl;

            /// <summary>
            /// The <see cref="GCHandle"/> to the captured <see cref="SendOrPostCallback"/>.
            /// </summary>
            private GCHandle callbackHandle;

            /// <summary>
            /// The <see cref="GCHandle"/> to the captured state (if present, or a <see langword="null"/> handle otherwise).
            /// </summary>
            private GCHandle stateHandle;

            /// <summary>
            /// The current reference count for the object (from <c>IUnknown</c>).
            /// </summary>
            private volatile uint referenceCount;

            /// <summary>
            /// Creates a new <see cref="CoreDispatcherProxyHandler"/> instance for the input callback and state.
            /// </summary>
            /// <param name="handler">The input <see cref="SendOrPostCallback"/> callback to enqueue.</param>
            /// <param name="state">The input state to capture and pass to the callback.</param>
            /// <returns>A pointer to the newly initialized <see cref="CoreDispatcherProxyHandler"/> instance.</returns>
            public static CoreDispatcherProxyHandler* Create(SendOrPostCallback handler, object? state)
            {
#if NET6_0_OR_GREATER
                CoreDispatcherProxyHandler* @this = (CoreDispatcherProxyHandler*)NativeMemory.Alloc((nuint)sizeof(CoreDispatcherProxyHandler));
#else
                CoreDispatcherProxyHandler* @this = (CoreDispatcherProxyHandler*)Marshal.AllocHGlobal(sizeof(CoreDispatcherProxyHandler));
#endif

                @this->vtbl = (Impl.Vtable*)Unsafe.AsPointer(in Impl.Vtbl);
                @this->callbackHandle = GCHandle.Alloc(handler);
                @this->stateHandle = state is not null ? GCHandle.Alloc(state) : default;
                @this->referenceCount = 1;

                return @this;
            }

            /// <summary>
            /// Devirtualized API for <c>IUnknown.Release()</c>.
            /// </summary>
            /// <returns>The updated reference count for the current instance.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Release()
            {
                uint referenceCount = Interlocked.Decrement(ref this.referenceCount);

                if (referenceCount == 0)
                {
                    callbackHandle.Free();
                    
                    if (stateHandle.IsAllocated)
                    {
                        stateHandle.Free();
                    }

#if NET6_0_OR_GREATER
                    NativeMemory.Free(Unsafe.AsPointer(ref this));
#else
                    Marshal.FreeHGlobal((IntPtr)Unsafe.AsPointer(ref this));
#endif
                }

                return referenceCount;
            }

            /// <summary>
            /// A private type with the implementation of the unmanaged methods for <see cref="CoreDispatcherProxyHandler"/>.
            /// These methods will be set into the shared vtable and invoked by WinRT from the object passed to it as an interface.
            /// </summary>
            private static class Impl
            {
                [FixedAddressValueType]
                public static readonly Vtable Vtbl;

                /// <summary>
                /// The HRESULT for a successful operation.
                /// </summary>
                private const int S_OK = 0;

                /// <summary>
                /// The HRESULT for an invalid cast from <c>IUnknown.QueryInterface</c>.
                /// </summary>
                private const int E_NOINTERFACE = unchecked((int)0x80004002);

                /// <summary>
                /// The GUID for the <c>IUnknown</c> COM interface.
                /// </summary>
                private static readonly Guid GuidOfIUnknown = new(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

                /// <summary>
                /// The GUID for the <c>IAgileObject</c> WinRT interface.
                /// </summary>
                private static readonly Guid GuidOfIAgileObject = new(0x94EA2B94, 0xE9CC, 0x49E0, 0xC0, 0xFF, 0xEE, 0x64, 0xCA, 0x8F, 0x5B, 0x90);

                /// <summary>
                /// The GUID for the <c>IDispatchedHandler</c> WinRT interface.
                /// </summary>
                private static readonly Guid GuidOfIDispatchedHandler = new(0xD1F276C4, 0x98D8, 0x4636, 0xBF, 0x49, 0xEB, 0x79, 0x50, 0x75, 0x48, 0xE9);

                static Impl()
                {
                    Vtbl.QueryInterface = &QueryInterface;
                    Vtbl.AddRef = &AddRef;
                    Vtbl.Release = &Release;
                    Vtbl.Invoke = &Invoke;
                }

                /// <summary>
                /// Implements <c>IUnknown.QueryInterface(REFIID, void**)</c>.
                /// </summary>
                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
                public static int QueryInterface(CoreDispatcherProxyHandler* @this, Guid* riid, void** ppvObject)
                {
                    if (riid->Equals(GuidOfIUnknown) ||
                        riid->Equals(GuidOfIAgileObject) ||
                        riid->Equals(GuidOfIDispatchedHandler))
                    {
                        Interlocked.Increment(ref @this->referenceCount);

                        *ppvObject = @this;

                        return S_OK;
                    }

                    return E_NOINTERFACE;
                }

                /// <summary>
                /// Implements <c>IUnknown.AddRef()</c>.
                /// </summary>
                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
                public static uint AddRef(CoreDispatcherProxyHandler* @this)
                {
                    return Interlocked.Increment(ref @this->referenceCount);
                }

                /// <summary>
                /// Implements <c>IUnknown.Release()</c>.
                /// </summary>
                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
                public static uint Release(CoreDispatcherProxyHandler* @this)
                {
                    uint referenceCount = Interlocked.Decrement(ref @this->referenceCount);

                    if (referenceCount == 0)
                    {
                        @this->callbackHandle.Free();

                        if (@this->stateHandle.IsAllocated)
                        {
                            @this->stateHandle.Free();
                        }

#if NET6_0_OR_GREATER
                        NativeMemory.Free(@this);
#else
                        Marshal.FreeHGlobal((IntPtr)@this);
#endif
                    }

                    return referenceCount;
                }

                /// <summary>
                /// Implements <c>IDispatchedHandler.Invoke()</c>.
                /// </summary>
                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
                public static int Invoke(CoreDispatcherProxyHandler* @this)
                {
                    object callback = @this->callbackHandle.Target!;
                    object? state = @this->stateHandle.IsAllocated ? @this->stateHandle.Target! : null;

                    try
                    {
                        Unsafe.As<SendOrPostCallback>(callback)(state);
                    }
                    catch (Exception e)
                    {
                        ExceptionHelpers.SetErrorInfo(e);

                        return ExceptionHelpers.GetHRForException(e);
                    }

                    return S_OK;
                }

                public struct Vtable
                {
                    public delegate* unmanaged[MemberFunction]<CoreDispatcherProxyHandler*, Guid*, void**, int> QueryInterface;
                    public delegate* unmanaged[MemberFunction]<CoreDispatcherProxyHandler*, uint> AddRef;
                    public delegate* unmanaged[MemberFunction]<CoreDispatcherProxyHandler*, uint> Release;
                    public delegate* unmanaged[MemberFunction]<CoreDispatcherProxyHandler*, int> Invoke;
                }
            }
        }
    }
}

#endif

#endif