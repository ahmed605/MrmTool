using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using TerraFX.Interop.WinRT;
using WinRT;

namespace MrmTool.Polyfills
{
    internal unsafe struct LegacyNonImmersiveView
    {
        private static readonly void** Vtbl = InitVtbl();

        private void** vtbl;
        private volatile uint referenceCount;

        private GCHandle _activatedHandlers;

        public static LegacyNonImmersiveView* Create()
        {
            LegacyNonImmersiveView* @this = (LegacyNonImmersiveView*)NativeMemory.Alloc((nuint)sizeof(LegacyNonImmersiveView));

            @this->vtbl = Vtbl;
            @this->_activatedHandlers = GCHandle.Alloc(new Dictionary<TerraFX.Interop.WinRT.EventRegistrationToken, Pointer<ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>>>());
            @this->referenceCount = 1;

            return @this;
        }

        public void Activate()
        {
            var handlers = Unsafe.As<Dictionary<TerraFX.Interop.WinRT.EventRegistrationToken, Pointer<ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>>>>(_activatedHandlers.Target)!;
            foreach (var handlerPtr in handlers.Values)
            {
                var handler = handlerPtr.Value;
                // TODO: pass IActivatedEventArgs
                handler->Invoke(new Pointer<ICoreApplicationView>((ICoreApplicationView*)Unsafe.AsPointer(in this)), null);
            }
        }

        private static void** InitVtbl()
        {
            void** vtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(LegacyNonImmersiveView), sizeof(void*) * 10);

            vtbl[0] = (delegate* unmanaged<LegacyNonImmersiveView*, Guid*, void**, int>)&QueryInterface;
            vtbl[1] = (delegate* unmanaged<LegacyNonImmersiveView*, uint>)&AddRef;
            vtbl[2] = (delegate* unmanaged<LegacyNonImmersiveView*, uint>)&Release;
            vtbl[3] = (delegate* unmanaged<LegacyNonImmersiveView*, uint*, Guid**, int>)&GetIids;
            vtbl[4] = (delegate* unmanaged<LegacyNonImmersiveView*, nint*, int>)&GetRuntimeClassName;
            vtbl[5] = (delegate* unmanaged<LegacyNonImmersiveView*, TerraFX.Interop.WinRT.TrustLevel*, int>)&GetTrustLevel;
            vtbl[6] = (delegate* unmanaged<LegacyNonImmersiveView*, ICoreWindow**, int>)&get_CoreWindow;
            vtbl[7] = (delegate* unmanaged<LegacyNonImmersiveView*, ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>*, TerraFX.Interop.WinRT.EventRegistrationToken*, int>)&add_Activated;
            vtbl[8] = (delegate* unmanaged<LegacyNonImmersiveView*, TerraFX.Interop.WinRT.EventRegistrationToken, int>)&remove_Activated;
            vtbl[9] = (delegate* unmanaged<LegacyNonImmersiveView*, byte*, int>)&get_IsMain;
            vtbl[10] = (delegate* unmanaged<LegacyNonImmersiveView*, byte*, int>)&get_IsHosted;

            return vtbl;
        }

        [UnmanagedCallersOnly]
        public static int QueryInterface(LegacyNonImmersiveView* @this, Guid* riid, void** ppvObject)
        {
            if (riid->Equals(IID.IID_IUnknown) ||
                riid->Equals(IID.IID_IInspectable) ||
                riid->Equals(IID.IID_IAgileObject) ||
                riid->Equals(IID.IID_ICoreApplicationView))
            {
                Interlocked.Increment(ref @this->referenceCount);

                *ppvObject = @this;

                return S.S_OK;
            }

            return E.E_NOINTERFACE;
        }

        /// <summary>
        /// Implements <c>IUnknown.AddRef()</c>.
        /// </summary>
        [UnmanagedCallersOnly]
        public static uint AddRef(LegacyNonImmersiveView* @this)
        {
            return Interlocked.Increment(ref @this->referenceCount);
        }

        /// <summary>
        /// Implements <c>IUnknown.Release()</c>.
        /// </summary>
        [UnmanagedCallersOnly]
        public static uint Release(LegacyNonImmersiveView* @this)
        {
            uint referenceCount = Interlocked.Decrement(ref @this->referenceCount);

            if (referenceCount == 0)
            {
                @this->_activatedHandlers.Free();
                NativeMemory.Free(@this);
            }

            return referenceCount;
        }

        [UnmanagedCallersOnly]
        public static int GetIids(LegacyNonImmersiveView* @this, uint* iidCount, Guid** iids)
        {
            if (iidCount is not null)
            {
                *iidCount = 0;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int GetRuntimeClassName(LegacyNonImmersiveView* @this, nint* className)
        {
            if (className is not null)
            {
                *className = MarshalString.FromManaged("Windows.ApplicationModel.Core.CoreApplicationView");
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int GetTrustLevel(LegacyNonImmersiveView* @this, TerraFX.Interop.WinRT.TrustLevel* trustLevel)
        {
            if (trustLevel is not null)
            {
                *trustLevel = TerraFX.Interop.WinRT.TrustLevel.BaseTrust;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int get_CoreWindow(LegacyNonImmersiveView* @this, ICoreWindow** value)
        {
            if (value is not null)
            {
                *value = null;
            }

            return E.E_NOTIMPL;
        }

        [UnmanagedCallersOnly]
        public static int add_Activated(LegacyNonImmersiveView* @this, ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>* handler, TerraFX.Interop.WinRT.EventRegistrationToken* token)
        {
            if (handler is not null && token is not null)
            {
                handler->AddRef();

                var handlers = Unsafe.As<Dictionary<TerraFX.Interop.WinRT.EventRegistrationToken, Pointer<ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>>>>(@this->_activatedHandlers.Target)!;
                
                TerraFX.Interop.WinRT.EventRegistrationToken newToken = new() { value = Guid.NewGuid().GetHashCode() };
                handlers.Add(newToken, new Pointer<ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>>(handler));
                
                *token = newToken;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int remove_Activated(LegacyNonImmersiveView* @this, TerraFX.Interop.WinRT.EventRegistrationToken token)
        {
            if (token.value is not 0)
            {
                var handlers = Unsafe.As<Dictionary<TerraFX.Interop.WinRT.EventRegistrationToken, Pointer<ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>>>>(@this->_activatedHandlers.Target)!;
                if (handlers.TryGetValue(token, out var handlerPtr))
                {
                    handlerPtr.Value->Release();
                    handlers.Remove(token);
                }
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int get_IsMain(LegacyNonImmersiveView* @this, byte* value)
        {
            if (value is not null)
            {
                *value = 1;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly]
        public static int get_IsHosted(LegacyNonImmersiveView* @this, byte* value)
        {
            if (value is not null)
            {
                *value = 0;
            }

            return S.S_OK;
        }
    }
}
