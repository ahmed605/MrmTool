using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using TerraFX.Interop.WinRT;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRT;
using static MrmTool.ErrorHelpers;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.Windows.WM;
using static TerraFX.Interop.Windows.WS;

namespace MrmTool
{
    internal class Program
    {
        static private App? _xamlApp = null;
        static private HWND _coreHwnd;
        public static HWND WindowHandle;

        [STAThread]
        static unsafe void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            NativeUtils.InitializeResourceManager();
            _xamlApp = new App();

            ReadOnlySpan<char> className = ['M', 'r', 'm', 'T', 'o', 'o', 'l', 'C', 'l', 'a', 's', 's', '\0'];
            ReadOnlySpan<char> windowName = ['M', 'r', 'm', 'T', 'o', 'o', 'l', '\0'];

            char* lpszClassName = (char*)Unsafe.AsPointer(in MemoryMarshal.GetReference(className));
            char* lpWindowName = (char*)Unsafe.AsPointer(in MemoryMarshal.GetReference(windowName));

            WNDCLASSW wc;
            wc.lpfnWndProc = &WndProc;
            wc.hInstance = GetModuleHandleW(null);
            wc.lpszClassName = lpszClassName;
            ThrowLastErrorIfNull(RegisterClassW(&wc));

            WindowHandle = CreateWindowExW(WS_EX_NOREDIRECTIONBITMAP | WS_EX_DLGMODALFRAME, lpszClassName, lpWindowName, WS_OVERLAPPEDWINDOW | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, HWND.NULL, HMENU.NULL, wc.hInstance, null);
            ThrowLastErrorIfDefault(WindowHandle);

            LoadLibraryA((sbyte*)Unsafe.AsPointer(in "twinapi.appcore.dll"u8.GetPinnableReference()));
            LoadLibraryA((sbyte*)Unsafe.AsPointer(in "threadpoolwinrt.dll"u8.GetPinnableReference()));

            nint pCoreWindow;

            char empty = '\0';

            NativeUtils.PrivateCreateCoreWindow(
                    NativeUtils.CoreWindowType.IMMERSIVE_HOSTED,
                    &empty,
                    0, 0, 0, 0,
                    0,
                    WindowHandle,
                    NativeUtils.IID_ICoreWindow,
                    &pCoreWindow);

            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));

            CoreWindow coreWindow = CoreWindow.FromAbi(pCoreWindow);
            Marshal.Release(pCoreWindow);

            nint pCoreApplicationView = CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView();
            CoreApplicationView view = CoreApplicationView.FromAbi(pCoreApplicationView);
            Marshal.Release(pCoreApplicationView);

            FrameworkView frameworkView = new();
            frameworkView.Initialize(view);
            frameworkView.SetWindow(coreWindow);

            HWND coreHwnd;
            using ComPtr<ICoreWindowInterop> interop = default;
            ThrowIfFailed(((IUnknown*)((IWinRTObject)coreWindow).NativeObject.ThisPtr)->QueryInterface(__uuidof<ICoreWindowInterop>(), (void**)interop.GetAddressOf()));
            ThrowIfFailed(interop.Get()->get_WindowHandle(&coreHwnd));

            _coreHwnd = coreHwnd;

            RECT clientRect;
            GetClientRect(WindowHandle, &clientRect);

            SetParent(coreHwnd, WindowHandle);
            SetWindowLong(coreHwnd, GWL.GWL_STYLE, WS_CHILD | WS_VISIBLE);
            SetWindowPos(coreHwnd, HWND.NULL, 0, 0, clientRect.right - clientRect.left, clientRect.bottom - clientRect.top, SWP.SWP_NOZORDER | SWP.SWP_SHOWWINDOW | SWP.SWP_NOACTIVATE);

            Frame frame = new();
            frame.Navigate(typeof(MainPage));

            Window.Current.Content = frame;

            frameworkView.Run();
        }

        [UnmanagedCallersOnly]
        private unsafe static LRESULT WndProc(HWND hWnd, uint message, WPARAM wParam, LPARAM lParam)
        {
            switch (message)
            {
                case WM_CREATE:
                    NativeUtils.EnableDarkModeSupport(hWnd);
                    NativeUtils.EnsureTitleBarTheme(hWnd);
                    break;
                case WM_SIZE:
                    SetWindowPos(_coreHwnd, HWND.NULL, 0, 0, LOWORD(lParam), HIWORD(lParam), SWP.SWP_NOZORDER | SWP.SWP_SHOWWINDOW | SWP.SWP_NOACTIVATE);
                    SendMessageW(_coreHwnd, message, wParam, lParam);
                    break;
                case WM_SETTINGCHANGE:
                    if ((BOOL)lParam && new string((char*)lParam) == "ImmersiveColorSet")
                        NativeUtils.EnsureTitleBarTheme(hWnd);

                    goto case WM_THEMECHANGED;
                case WM_THEMECHANGED:
                    SendMessageW(_coreHwnd, message, wParam, lParam);
                    break;
                case WM_SETFOCUS:
                    SetFocus(_coreHwnd);
                    break;
                case WM_DESTROY:
                    _xamlApp = null;
                    PostQuitMessage(0);
                    break;
                default:
                    return DefWindowProcW(hWnd, message, wParam, lParam);
            }
            return 0;
        }
    }
}
