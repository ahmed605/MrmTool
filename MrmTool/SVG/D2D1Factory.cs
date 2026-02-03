using System.Runtime.InteropServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace MrmTool.SVG;

internal static unsafe class D2D1Factory
{
    private static ID2D1Factory* factory;

    public static ID2D1Factory* GetFactory()
    {
        if (factory == null)
        {
            ID2D1Factory* tempFactory;
            Marshal.ThrowExceptionForHR(D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, __uuidof<ID2D1Factory>(), null, (void**)&tempFactory));
            factory = tempFactory;
        }
        return factory;
    }

    public static HRESULT CreatePathGeometry(ID2D1PathGeometry** pathGeometry)
    {
        return GetFactory()->CreatePathGeometry(pathGeometry);
    }
}