using WinRT;
using MrmLib;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;
using MrmTool.Common;
using MrmTool.Models;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static MrmTool.Common.ErrorHelpers;

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

        internal static string ToScintillaLanguage(this string extensionAfterPeriod)
        {
            return extensionAfterPeriod switch
            {
                // "cxx" or "h" or "hpp" or "c" or "cc" => "cpp",
                "cs" => "csharp",
                "js" => "javascript",
                "htm" => "html",
                "ini" or "inf" => "props",
                "resw" or "resx" or "xaml" => "xml",
                "scss" or "less" or "hss" => "css",
                _ => extensionAfterPeriod,
            };
        }

        internal static ResourceType DetermineResourceType(this string name)
        {
            return Path.GetExtension(name).ToLowerInvariant() switch
            {
                ".xbf"
                    => ResourceType.Xbf,

                ".xaml"
                    => ResourceType.Xaml,

                ".ttf" or ".otf" or ".ttc"
                    => ResourceType.Font,

                ".mp4" or ".avi" or ".mov" or ".wmv" or ".mkv" or ".webm"
                    => ResourceType.Video,

                ".mp3" or ".wav" or ".wma" or ".ogg" or ".flac" or ".opus"
                    => ResourceType.Audio,

                ".png" or ".jpg" or ".gif" or ".bmp" or ".svg" or ".jpeg" or
                ".webp" or ".heif" or ".tiff"
                    => ResourceType.Image,

                ".txt" or ".xml" or ".csv" or ".ini" or ".inf" or ".json" or ".html" or
                ".htm" or ".css" or ".scss" or ".less" or ".hss" or ".js" or ".cs" or
                ".resw" or ".resx"
                    => ResourceType.Text,

                _
                    => ResourceType.Unknown
            };
        }

        internal static BitmapImage GetCorrespondingIcon(this ResourceType type)
        {
            return type switch
            {
                ResourceType.Folder => Icons.Folder.Value,
                ResourceType.Text => Icons.Text.Value,
                ResourceType.Image => Icons.Image.Value,
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
                ResourceType.Audio => Icons.AudioLarge.Value,
                ResourceType.Video => Icons.VideoLarge.Value,
                ResourceType.Font => Icons.FontLarge.Value,
                ResourceType.Xaml or ResourceType.Xbf => Icons.XamlLarge.Value,
                _ => Icons.UnknownLarge.Value,
            };
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

        internal static string[] SplitIntoResourceNames(this string resourceName)
        {
            int count = resourceName.Count('/');

            if (count is 0)
            {
                return [resourceName];
            }

            int resultIdx = 0;
            int currentIdx = -1;
            var result = new string[count + 1];

            while ((currentIdx = resourceName.IndexOf('/', currentIdx + 1)) >= 0)
            {
                result[resultIdx++] = resourceName.Substring(0, currentIdx);
            }

            result[resultIdx] = resourceName;

            return result;
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

        internal unsafe static byte* GetData(this IBuffer buffer)
        {
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
