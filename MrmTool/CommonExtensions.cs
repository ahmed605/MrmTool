using TerraFX.Interop.Windows;
using Windows.Storage.Pickers;
using System.Runtime.CompilerServices;

namespace MrmTool
{
    internal static class CommonExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetInProc(this FileOpenPicker picker, bool inProc = true)
        {
            ((IPickerPrivateInitialization)(object)picker).SetInProcOverride((BOOL)inProc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetInProc(this FileSavePicker picker, bool inProc = true)
        {
            ((IPickerPrivateInitialization)(object)picker).SetInProcOverride((BOOL)inProc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetInProc(this FolderPicker picker, bool inProc = true)
        {
            ((IPickerPrivateInitialization)(object)picker).SetInProcOverride((BOOL)inProc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeWithWindow(this FileOpenPicker picker, HWND? hwnd = null)
        {
            hwnd ??= ((App)App.Current).HWND;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeWithWindow(this FileSavePicker picker, HWND? hwnd = null)
        {
            hwnd ??= ((App)App.Current).HWND;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeWithWindow(this FolderPicker picker, HWND? hwnd = null)
        {
            hwnd ??= ((App)App.Current).HWND;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Initialize(this FileOpenPicker picker)
        {
            picker.SetInProc();
            picker.InitializeWithWindow();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Initialize(this FileSavePicker picker)
        {
            picker.SetInProc();
            picker.InitializeWithWindow();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Initialize(this FolderPicker picker)
        {
            picker.SetInProc();
            picker.InitializeWithWindow();
        }
    }
}
