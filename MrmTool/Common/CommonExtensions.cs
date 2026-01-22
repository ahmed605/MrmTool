using MrmLib;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static MrmTool.Common.ErrorHelpers;
using MrmTool.Common;

namespace MrmTool.Common
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
            hwnd ??= Program.WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeWithWindow(this FileSavePicker picker, HWND? hwnd = null)
        {
            hwnd ??= Program.WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeWithWindow(this FolderPicker picker, HWND? hwnd = null)
        {
            hwnd ??= Program.WindowHandle;
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
        internal static string SetDisplayName(this string name, string displayName)
        {
            return name.LastIndexOf('/', out var idx) != -1 ? $"{name[..(idx + 1)]}{displayName}" : displayName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string? GetParentName(this string name)
        {
            return name.LastIndexOf('/', out var idx) != -1 ? name[..idx] : null;
        }

        private const char DirectorySeparatorChar = '\\';
        private const char AltDirectorySeparatorChar = '/';

        [return: NotNullIfNotNull(nameof(path))]
        internal static string? GetExtensionAfterPeriod(this string path)
        {
            if (path == null)
                return null;

            return path.ToLowerInvariant().AsSpan().GetExtensionAfterPeriod().ToString();
        }

        internal static ReadOnlySpan<char> GetExtensionAfterPeriod(this ReadOnlySpan<char> path)
        {
            int length = path.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                char ch = path[i];
                if (ch == '.')
                {
                    if (i != length - 1)
                    {
                        var idx = i + 1;
                        return path.Slice(idx, length - idx);
                    }
                    else
                        return ReadOnlySpan<char>.Empty;
                }
                if (IsDirectorySeparator(ch))
                    break;
            }

            return ReadOnlySpan<char>.Empty;
        }

        /// <summary>
        /// True if the given character is a directory separator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDirectorySeparator(char c)
        {
            return c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;
        }

        internal unsafe static HDROP* GetHDropUnsafe(this DataPackageView view)
        {
            using ComPtr<IDataObject> dataObject = default;

            if(SUCCEEDED_LOG(((IUnknown*)((IWinRTObject)view).NativeObject.ThisPtr)->QueryInterface(
                (Guid*)Unsafe.AsPointer(in IID.IID_IDataObject),
                (void**)dataObject.GetAddressOf())))
            {
                FORMATETC format = new()
                {
                    cfFormat = CF.CF_HDROP,
                    dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = null,
                    tymed = (uint)TYMED.TYMED_HGLOBAL
                };

                STGMEDIUM medium = new();
                if (SUCCEEDED_LOG(dataObject.Get()->GetData(&format, &medium)))
                {
                    return (HDROP*)medium.Anonymous.hGlobal; // GMEM_FIXED
                }
            }

            return null;
        }

        internal unsafe static string? GetFirstStorageItemPathUnsafe(this DataPackageView view)
        {
            var hDrop = view.GetHDropUnsafe();
            if (hDrop is null) return null;

            var dropFiles = *(DROPFILES**)hDrop;
            if (dropFiles is null) return null;

            string path = new((char*)((byte*)dropFiles + dropFiles->pFiles));
            LOG_LAST_ERROR_IF(GlobalFree((HGLOBAL)hDrop).Value is not null);

            return path;
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
