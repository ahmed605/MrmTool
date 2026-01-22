using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MrmTool.Scintilla
{
    internal static unsafe partial class Lexilla
    {
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [LibraryImport("WinUIEditor.dll", StringMarshalling = StringMarshalling.Utf8)]
        internal static partial ILexer5* CreateLexer(string language);

        internal const int SCE_CSS_DEFAULT = 0;
        internal const int SCE_CSS_TAG = 1;
        internal const int SCE_CSS_CLASS = 2;
        internal const int SCE_CSS_PSEUDOCLASS = 3;
        internal const int SCE_CSS_UNKNOWN_PSEUDOCLASS = 4;
        internal const int SCE_CSS_OPERATOR = 5;
        internal const int SCE_CSS_IDENTIFIER = 6;
        internal const int SCE_CSS_UNKNOWN_IDENTIFIER = 7;
        internal const int SCE_CSS_VALUE = 8;
        internal const int SCE_CSS_COMMENT = 9;
        internal const int SCE_CSS_ID = 10;
        internal const int SCE_CSS_IMPORTANT = 11;
        internal const int SCE_CSS_DIRECTIVE = 12;
        internal const int SCE_CSS_DOUBLESTRING = 13;
        internal const int SCE_CSS_SINGLESTRING = 14;
        internal const int SCE_CSS_IDENTIFIER2 = 15;
        internal const int SCE_CSS_ATTRIBUTE = 16;
        internal const int SCE_CSS_IDENTIFIER3 = 17;
        internal const int SCE_CSS_PSEUDOELEMENT = 18;
        internal const int SCE_CSS_EXTENDED_IDENTIFIER = 19;
        internal const int SCE_CSS_EXTENDED_PSEUDOCLASS = 20;
        internal const int SCE_CSS_EXTENDED_PSEUDOELEMENT = 21;
        internal const int SCE_CSS_GROUP_RULE = 22;
        internal const int SCE_CSS_VARIABLE = 23;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ILexer5
    {
        private readonly void** vtbl;

        internal nint PropertySetUnsafe(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            return ((delegate* unmanaged[Stdcall]<void*, byte *, byte*, nint>)vtbl[5])(
                Unsafe.AsPointer(in Unsafe.AsRef(in this)),
                (byte*)Unsafe.AsPointer(in name.GetPinnableReference()),
                (byte*)Unsafe.AsPointer(in value.GetPinnableReference()));
        }
    }
}
