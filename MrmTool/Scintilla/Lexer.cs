using System.Text;
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

        internal const int SCE_PROPS_DEFAULT = 0;
        internal const int SCE_PROPS_COMMENT = 1;
        internal const int SCE_PROPS_SECTION = 2;
        internal const int SCE_PROPS_ASSIGNMENT = 3;
        internal const int SCE_PROPS_DEFVAL = 4;
        internal const int SCE_PROPS_KEY = 5;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ILexer5
    {
        private readonly void** vtbl;

        internal nint PropertySetUnsafe(byte* name, byte* value)
        {
            return ((delegate* unmanaged[Stdcall]<void*, byte *, byte*, nint>)vtbl[5])(
                Unsafe.AsPointer(in this),
                name,
                value);
        }

        /*
        
        // Uncomment if needed

        internal nint PropertySet(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            fixed(byte* pName = name)
            fixed(byte* pValue = value)
            {
                return ((delegate* unmanaged[Stdcall]<void*, byte*, byte*, nint>)vtbl[5])(
                    Unsafe.AsPointer(in Unsafe.AsRef(in this)),
                    pName,
                    pValue);
            }
        }

        internal nint PropertySet(string name, string value)
        {
            Span<byte> nameSpan = stackalloc byte[name.Length + 1];
            Span<byte> valueSpan = stackalloc byte[value.Length + 1];

            Encoding.UTF8.GetBytes(name, nameSpan);
            Encoding.UTF8.GetBytes(value, valueSpan);

            return ((delegate* unmanaged[Stdcall]<void*, byte*, byte*, nint>)vtbl[5])(
                Unsafe.AsPointer(in Unsafe.AsRef(in this)),
                (byte*)Unsafe.AsPointer(in nameSpan.GetPinnableReference()),
                (byte*)Unsafe.AsPointer(in valueSpan.GetPinnableReference()));
        }
        */
    }
}
