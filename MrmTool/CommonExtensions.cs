using TerraFX.Interop.Windows;
using Windows.Storage.Pickers;
using System.Runtime.CompilerServices;
using MrmLib;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int LastIndexOf(this string str, char c, out int index)
        {
            index = str.LastIndexOf(c);
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetDisplayName(this string name)
        {
            return name.LastIndexOf('/', out var idx) != -1 ? name[(idx + 1)..] : name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string? GetParentName(this string name)
        {
            return name.LastIndexOf('/', out var idx) != -1 ? name[..idx] : null;
        }

        extension(QualifierOperator op)
        {
            internal string Symbol
            {
                get
                {
                    return op switch
                    {
                        QualifierOperator.False => "= false",
                        QualifierOperator.True => "= true",
                        QualifierOperator.AttributeDefined => "is defined",
                        QualifierOperator.AttributeUndefined => "is undefined",
                        QualifierOperator.NotEqual => "!≃",
                        QualifierOperator.NoMatch => "!=",
                        QualifierOperator.Less => "<",
                        QualifierOperator.LessOrEqual => "≤",
                        QualifierOperator.Greater => ">",
                        QualifierOperator.GreaterOrEqual => "≥",
                        QualifierOperator.Match => "=",
                        QualifierOperator.Equal => "≃",
                        QualifierOperator.ExtendedOperator or QualifierOperator.Custom => "[Custom Operator]",
                        _ => $"[Custom Operator ({(uint)op})]",
                    };
                }
            }
        }
    }
}
