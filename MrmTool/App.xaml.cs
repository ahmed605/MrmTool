using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

using WinRT;
using System.Runtime.CompilerServices;

using TerraFX.Interop.Windows;
using TerraFX.Interop.WinRT;

using static TerraFX.Interop.Windows.WM;
using static TerraFX.Interop.Windows.SW;
using static TerraFX.Interop.Windows.SWP;
using static TerraFX.Interop.Windows.Windows;
using System.Threading;
using Windows.System;

namespace MrmTool
{
    public partial class App : Application
    {
        private HWND _hwnd = default;
        private HWND _xamlHwnd = default;
        private HWND _coreHwnd = default;

        private bool _xamlInitialized = false;

        private DesktopWindowXamlSource? _desktopWindowXamlSource = null;
        private WindowsXamlManager? _xamlManager = null;
        private CoreWindow? _coreWindow = null;

        private ComPtr<IDesktopWindowXamlSourceNative2> _nativeSource = default;

        internal Frame? Frame = null;

        public App(HWND hwnd)
        {
            _hwnd = hwnd;
            InitializeXaml();
        }

        public HWND HWND => _hwnd;

        private unsafe void InitializeXaml()
        {
            LoadLibraryA((sbyte*)Unsafe.AsPointer(ref Unsafe.AsRef(in "twinapi.appcore.dll\0"u8.GetPinnableReference())));
            LoadLibraryA((sbyte*)Unsafe.AsPointer(ref Unsafe.AsRef(in "threadpoolwinrt.dll\0"u8.GetPinnableReference())));

            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();
            _desktopWindowXamlSource = new();

            ThrowIfFailed(((IUnknown*)((IWinRTObject)_desktopWindowXamlSource).NativeObject.ThisPtr)->QueryInterface(__uuidof<IDesktopWindowXamlSourceNative2>(), (void**)_nativeSource.GetAddressOf()));

            ThrowIfFailed(_nativeSource.Get()->AttachToWindow(_hwnd));
            ThrowIfFailed(_nativeSource.Get()->get_WindowHandle((HWND*)Unsafe.AsPointer(ref _xamlHwnd)));

            RECT wRect;
            GetClientRect(_hwnd, &wRect);
            SetWindowPos(_xamlHwnd, HWND.NULL, 0, 0, wRect.right - wRect.left, wRect.bottom - wRect.top, SWP_SHOWWINDOW | SWP_NOACTIVATE | SWP_NOZORDER);

            _coreWindow = CoreWindow.GetForCurrentThread();

            using ComPtr<ICoreWindowInterop> interop = default;
            ThrowIfFailed(((IUnknown*)((IWinRTObject)_coreWindow).NativeObject.ThisPtr)->QueryInterface(__uuidof<ICoreWindowInterop>(), (void**)interop.GetAddressOf()));
            ThrowIfFailed(interop.Get()->get_WindowHandle((HWND*)Unsafe.AsPointer(ref _coreHwnd)));

            Frame = new Frame();
            _desktopWindowXamlSource.Content = Frame;

            _xamlInitialized = true;
            OnXamlInitialized();
        }

        private void OnXamlInitialized()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
            Frame?.Navigate(typeof(MainPage));
        }

        internal void OnResize(int x, int y)
        {
            if (_xamlHwnd != default)
                SetWindowPos(_xamlHwnd, HWND.NULL, 0, 0, x, y, SWP_SHOWWINDOW | SWP_NOACTIVATE | SWP_NOZORDER);

            if (_coreHwnd != default)
                SendMessageA(_coreHwnd, WM_SIZE, (WPARAM)x, y);
        }

        internal void ProcessCoreWindowMessage(uint message, WPARAM wParam, LPARAM lParam)
        {
            if (_coreHwnd != default)
                SendMessageA(_coreHwnd, message, wParam, lParam);
        }

        internal void OnSetFocus()
        {
            if (_xamlHwnd != default)
                SetFocus(_xamlHwnd);
        }

        internal unsafe bool PreTranslateMessage(MSG* msg)
        {
            BOOL result = false;

            if (_xamlInitialized)
                _nativeSource.Get()->PreTranslateMessage(msg, &result);

            return result;
        }
    }
}
