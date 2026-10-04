using MrmLib;
using MrmTool.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TerraFX.Interop.Windows;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using WinRT;

using static MrmTool.Common.ErrorHelpers;
using static TerraFX.Interop.Windows.Windows;

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

        internal static string ToScintillaLanguage(this string extensionAfterPeriod)
        {
            return extensionAfterPeriod switch
            {
                // "cxx" or "h" or "hpp" or "c" or "cc" => "cpp",
                "cs" => "csharp",
                "js" => "javascript",
                "htm" => "html",
                "ini" or "inf" => "props",
                "xsl" or "resw" or "resx" or "xaml" => "xml",
                "scss" or "less" or "hss" => "css",
                _ => extensionAfterPeriod,
            };
        }

        internal static BitmapImage GetCorrespondingIcon(this ResourceType type)
        {
            return type switch
            {
                ResourceType.Folder => Icons.Folder.Value,
                ResourceType.Text => Icons.Text.Value,
                ResourceType.Image => Icons.Image.Value,
                ResourceType.Svg => Icons.Image.Value,
                ResourceType.Audio => Icons.Audio.Value,
                ResourceType.Video => Icons.Video.Value,
                ResourceType.Font => Icons.Font.Value,
                ResourceType.Xaml or ResourceType.Xbf => Icons.Xaml.Value,
                _ => Icons.Unknown.Value,
            };
        }

        internal static BitmapImage GetCorrespondingLargeIcon(this ResourceType type)
        {
            return type switch
            {
                ResourceType.Folder => Icons.FolderLarge.Value,
                ResourceType.Text => Icons.TextLarge.Value,
                ResourceType.Image => Icons.ImageLarge.Value,
                ResourceType.Svg => Icons.ImageLarge.Value,
                ResourceType.Audio => Icons.AudioLarge.Value,
                ResourceType.Video => Icons.VideoLarge.Value,
                ResourceType.Font => Icons.FontLarge.Value,
                ResourceType.Xaml or ResourceType.Xbf => Icons.XamlLarge.Value,
                _ => Icons.UnknownLarge.Value,
            };
        }

        internal static bool Matches(this Qualifier left, Qualifier right)
        {
            return left.Attribute == right.Attribute &&
                   left.Operator == right.Operator &&
                   left.Value.Equals(right.Value, StringComparison.Ordinal) &&
                   left.Priority == right.Priority &&
                   left.FallbackScore == right.FallbackScore;
        }

        internal static bool Matches(this Qualifier left, QualifierAttribute attribute, QualifierOperator op, string value, int priority, double fallbackScore)
        {
            return left.Attribute == attribute &&
                   left.Operator == op &&
                   left.Value.Equals(value, StringComparison.Ordinal) &&
                   left.Priority == priority &&
                   left.FallbackScore == fallbackScore;
        }

        /*internal static string Format(this IReadOnlyList<Qualifier> qualifiers)
        {
            return qualifiers.Count is 0 ? "(None)" : string.Join(", ",
                   qualifiers.Select(q => $"({q.AttributeName} {q.Operator.Symbol} {q.Value}{(q.Priority is { } p && p != 0 ? $", Priority = {p}" : string.Empty)}{(q.FallbackScore is { } s && s != 0 ? $", Fallback Score = {s}" : string.Empty)})"));
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Format(this Qualifier qualifier)
        {
            return $"{qualifier.AttributeName} {qualifier.Operator.Symbol} {qualifier.Value}{(qualifier.Priority is { } p && p != 0 ? $", Priority = {p}" : string.Empty)}{(qualifier.FallbackScore is { } s && s != 0 ? $", Fallback Score = {s}" : string.Empty)}";
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

        internal unsafe static IReadOnlyList<string> GetStorageItemPathsUnsafe(this DataPackageView view)
        {
            var hDrop = view.GetHDropUnsafe();
            if (hDrop is null) return [];

            var dropFiles = *(DROPFILES**)hDrop;
            if (dropFiles is null) return [];

            var buffer = (char*)((byte*)dropFiles + dropFiles->pFiles);

            var start = buffer;
            var paths = new List<string>();
            while (*(uint*)buffer != 0)
            {
                var pNextChar = ++buffer;
                if (*pNextChar == 0)
                {
                    var length = (int)(pNextChar - start);
                    paths.Add(new(start, 0, length));

                    start = pNextChar + 1;
                }
            }

            LOG_LAST_ERROR_IF(GlobalFree((HGLOBAL)hDrop).Value is not null);
            return paths;
        }

        internal unsafe static byte* GetData(this IBuffer buffer)
        {
            if (buffer is WindowsRuntimeBuffer winrtBuffer)
            {
                WindowsRuntimeMarshal.TryGetDataUnsafe(winrtBuffer, out nint data);
                return (byte*)data;
            }

            using ComPtr<TerraFX.Interop.WinRT.IBufferByteAccess> bufferByteAccess = default;
            if (SUCCEEDED_LOG(((IUnknown*)((IWinRTObject)buffer).NativeObject.ThisPtr)->QueryInterface(
                (Guid*)Unsafe.AsPointer(in IID.IID_IBufferByteAccess),
                (void**)bufferByteAccess.GetAddressOf())))
            {
                byte* dataPtr = null;
                bufferByteAccess.Get()->Buffer(&dataPtr);
                return dataPtr;
            }

            return null;
        }

        internal unsafe static NativeBuffer GetBuffer(this Encoding encoding, string s)
        {
            var span = s.AsSpan();
            var size = encoding.GetByteCount(span);
            var buffer = new NativeBuffer((uint)size);

            encoding.GetBytes(span, new(buffer.Buffer, size));
            return buffer;
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
