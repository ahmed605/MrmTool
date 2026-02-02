using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using TerraFX.Interop.WinRT;
using WinRT;

namespace MrmTool.Polyfills
{
    internal unsafe struct LegacyNonImmersiveView
    {
        [FixedAddressValueType]
        private static readonly Vtable Vtbl;

        private Vtable* vtbl;
        private volatile uint referenceCount;

        private GCHandle _activatedHandlers;

        static LegacyNonImmersiveView()
        {
            Vtbl.QueryInterface = &QueryInterface;
            Vtbl.AddRef = &AddRef;
            Vtbl.Release = &Release;
            Vtbl.GetIids = &GetIids;
            Vtbl.GetRuntimeClassName = &GetRuntimeClassName;
            Vtbl.GetTrustLevel = &GetTrustLevel;
            Vtbl.get_CoreWindow = &get_CoreWindow;
            Vtbl.add_Activated = &add_Activated;
            Vtbl.remove_Activated = &remove_Activated;
            Vtbl.get_IsMain = &get_IsMain;
            Vtbl.get_IsHosted = &get_IsHosted;
        }

        public static LegacyNonImmersiveView* Create()
        {
            LegacyNonImmersiveView* @this = (LegacyNonImmersiveView*)NativeMemory.Alloc((nuint)sizeof(LegacyNonImmersiveView));

            @this->vtbl = (Vtable*)Unsafe.AsPointer(in Vtbl);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct Vtable
        {
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, Guid*, void**, int> QueryInterface;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, uint> AddRef;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, uint> Release;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, uint*, Guid**, int> GetIids;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, nint*, int> GetRuntimeClassName;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, TerraFX.Interop.WinRT.TrustLevel*, int> GetTrustLevel;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, ICoreWindow**, int> get_CoreWindow;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, ITypedEventHandler<Pointer<ICoreApplicationView>, Pointer<IActivatedEventArgs>>*, TerraFX.Interop.WinRT.EventRegistrationToken*, int> add_Activated;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, TerraFX.Interop.WinRT.EventRegistrationToken, int> remove_Activated;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, byte*, int> get_IsMain;
            public delegate* unmanaged[MemberFunction]<LegacyNonImmersiveView*, byte*, int> get_IsHosted;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
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
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static uint AddRef(LegacyNonImmersiveView* @this)
        {
            return Interlocked.Increment(ref @this->referenceCount);
        }

        /// <summary>
        /// Implements <c>IUnknown.Release()</c>.
        /// </summary>
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
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

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static int GetIids(LegacyNonImmersiveView* @this, uint* iidCount, Guid** iids)
        {
            if (iidCount is not null)
            {
                *iidCount = 0;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static int GetRuntimeClassName(LegacyNonImmersiveView* @this, nint* className)
        {
            if (className is not null)
            {
                *className = MarshalString.FromManaged("Windows.ApplicationModel.Core.CoreApplicationView");
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static int GetTrustLevel(LegacyNonImmersiveView* @this, TerraFX.Interop.WinRT.TrustLevel* trustLevel)
        {
            if (trustLevel is not null)
            {
                *trustLevel = TerraFX.Interop.WinRT.TrustLevel.BaseTrust;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static int get_CoreWindow(LegacyNonImmersiveView* @this, ICoreWindow** value)
        {
            if (value is not null)
            {
                *value = null;
            }

            return E.E_NOTIMPL;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
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

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
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

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        public static int get_IsMain(LegacyNonImmersiveView* @this, byte* value)
        {
            if (value is not null)
            {
                *value = 1;
            }

            return S.S_OK;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
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
