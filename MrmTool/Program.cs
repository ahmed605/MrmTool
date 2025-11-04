using TerraFX.Interop.WinRT;
using TerraFX.Interop.Windows;

using static TerraFX.Interop.Windows.WM;
using static TerraFX.Interop.Windows.WS;
using static TerraFX.Interop.WinRT.WinRT;
using static TerraFX.Interop.Windows.Windows;

using static MrmTool.ErrorHelpers;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MrmTool
{
    internal class Program
    {
        static private App? _xamlApp = null;

        [STAThread]
        static unsafe void Main()
        {
            sbyte* lpszClassName = (sbyte*)Unsafe.AsPointer(in "MrmToolClass\0"u8.GetPinnableReference());
            sbyte* lpWindowName = (sbyte*)Unsafe.AsPointer(in "MrmTool\0"u8.GetPinnableReference());

            WNDCLASSA wc;
            wc.lpfnWndProc = &WndProc;
            wc.hInstance = GetModuleHandleW(null);
            wc.lpszClassName = lpszClassName;
            ThrowLastErrorIfNull(RegisterClassA(&wc));

            ThrowLastErrorIfDefault(CreateWindowExA(WS_EX_NOREDIRECTIONBITMAP | WS_EX_DLGMODALFRAME, lpszClassName, lpWindowName, WS_OVERLAPPEDWINDOW | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, HWND.NULL, HMENU.NULL, wc.hInstance, null));

            MSG msg;
            while (GetMessageW(&msg, HWND.NULL, 0, 0))
            {
                bool xamlSourceProcessedMessage = _xamlApp is not null && _xamlApp.PreTranslateMessage(&msg);
                if (!xamlSourceProcessedMessage)
                {
                    TranslateMessage(&msg);
                    DispatchMessageW(&msg);
                }
            }
        }

        [UnmanagedCallersOnly]
        private unsafe static LRESULT WndProc(HWND hWnd, uint message, WPARAM wParam, LPARAM lParam)
        {
            switch (message)
            {
                case WM_CREATE:
                    NativeUtils.EnableDarkModeSupport(hWnd);
                    NativeUtils.EnsureTitleBarTheme(hWnd);
                    OnWindowCreate(hWnd);
                    break;
                case WM_SIZE:
                    _xamlApp?.OnResize(LOWORD(lParam), HIWORD(lParam));

                    break;
                case WM_SETTINGCHANGE:
                    if ((BOOL)lParam && new string((char*)lParam) == "ImmersiveColorSet")
                        NativeUtils.EnsureTitleBarTheme(hWnd);

                    goto case WM_THEMECHANGED;
                case WM_THEMECHANGED:
                    _xamlApp?.ProcessCoreWindowMessage(message, wParam, lParam);

                    break;
                case WM_SETFOCUS:
                    _xamlApp?.OnSetFocus();

                    break;
                case WM_DESTROY:
                    _xamlApp = null;
                    PostQuitMessage(0);
                    break;
                default:
                    return DefWindowProcA(hWnd, message, wParam, lParam);
            }
            return 0;
        }

        private static unsafe void OnWindowCreate(HWND hwnd)
        {
            RoInitialize(RO_INIT_TYPE.RO_INIT_SINGLETHREADED);
            NativeUtils.InitializeResourceManager();

            _xamlApp = new(hwnd);
        }
    }
}
