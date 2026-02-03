using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using Windows.Graphics;
using WinRT;
using WinRT.Interop;

namespace MrmTool.SVG
{
    public partial class StaticGeometrySource2D : IGeometrySource2D, IGeometrySource2DInterop
    {
        private ComPtr<ID2D1Geometry> geometry;

        public unsafe StaticGeometrySource2D(ID2D1Geometry* geometry)
        {
            // This automatically increases the reference count.
            this.geometry = geometry;
        }

        public unsafe int GetGeometry(ID2D1Geometry** geometry)
        {
            this.geometry.Get()->AddRef();
            *geometry = this.geometry.Get();
            GC.KeepAlive(this);
            return S.S_OK;
        }

        public unsafe int TryGetGeometryUsingFactory(ID2D1Factory* factory, ID2D1Geometry** geometry)
        {
            *geometry = null;
            using ComPtr<ID2D1Factory> realFactory = default;
            this.geometry.Get()->GetFactory(realFactory.GetAddressOf());

            // Per Win2D's behavior, return S_OK even if the factory doesn't match.

            if (realFactory.Get() == factory)
            {
                this.geometry.Get()->AddRef();
                *geometry = this.geometry.Get();
            }

            GC.KeepAlive(this);
            return S.S_OK;
        }

        ~StaticGeometrySource2D()
        {
            this.geometry.Dispose();
        }
    }

    [Guid("0657af73-53fd-47cf-84ff-c8492d2a80a3")]
    [WindowsRuntimeType("Windows.Foundation.UniversalApiContract")]
    [WindowsRuntimeHelperType(typeof(ABI.MrmTool.SVG.IGeometrySource2DInteropMethods))]
    public unsafe interface IGeometrySource2DInterop
    {
        int GetGeometry(ID2D1Geometry** geometry);

        int TryGetGeometryUsingFactory(ID2D1Factory* factory, ID2D1Geometry** geometry);
    }
}

namespace ABI.MrmTool.SVG
{
    using global::MrmTool.SVG;

    public unsafe static partial class IGeometrySource2DInteropMethods
    {
        [GuidRVAGen.Guid("0657af73-53fd-47cf-84ff-c8492d2a80a3")]
        public static partial ref readonly Guid IID { get; }

        public static readonly nint AbiToProjectionVftablePtr;

        static IGeometrySource2DInteropMethods()
        {
            AbiToProjectionVftablePtr = ComWrappersSupport.AllocateVtableMemory(typeof(IGeometrySource2DInterop), sizeof(IUnknownVftbl) + sizeof(nint) * 2);
            *(IUnknownVftbl*)AbiToProjectionVftablePtr = IUnknownVftbl.AbiToProjectionVftbl;
            ((void**)AbiToProjectionVftablePtr)[3] = (delegate* unmanaged[Stdcall]<nint, ID2D1Geometry**, int>)&Do_Abi_GetGeometry;
            ((void**)AbiToProjectionVftablePtr)[4] = (delegate* unmanaged[Stdcall]<nint, ID2D1Factory*, ID2D1Geometry**, int>)&Do_Abi_TryGetGeometryUsingFactory;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static int Do_Abi_GetGeometry(nint thisPtr, ID2D1Geometry** geometry)
        {
            return ComWrappersSupport.FindObject<IGeometrySource2DInterop>(thisPtr).GetGeometry(geometry);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static int Do_Abi_TryGetGeometryUsingFactory(nint thisPtr, ID2D1Factory* factory, ID2D1Geometry** geometry)
        {
            return ComWrappersSupport.FindObject<IGeometrySource2DInterop>(thisPtr).TryGetGeometryUsingFactory(factory, geometry);
        }
    }
}