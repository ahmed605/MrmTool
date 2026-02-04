using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NanoSVG
{
    using static Constants;
    using static NanoSVG;
    using static Parser;

    public enum NSVGpaintType : sbyte
    {
        NSVG_PAINT_UNDEF = -1,
        NSVG_PAINT_NONE = 0,
        NSVG_PAINT_COLOR = 1,
        NSVG_PAINT_LINEAR_GRADIENT = 2,
        NSVG_PAINT_RADIAL_GRADIENT = 3
    }

    public enum NSVGspreadType : byte
    {
        NSVG_SPREAD_PAD = 0,
        NSVG_SPREAD_REFLECT = 1,
        NSVG_SPREAD_REPEAT = 2
    }

    public enum NSVGlineJoin : byte
    {
        NSVG_JOIN_MITER = 0,
        NSVG_JOIN_ROUND = 1,
        NSVG_JOIN_BEVEL = 2
    }

    public enum NSVGlineCap : byte
    {
        NSVG_CAP_BUTT = 0,
        NSVG_CAP_ROUND = 1,
        NSVG_CAP_SQUARE = 2
    }

    public enum NSVGfillRule : byte
    {
        NSVG_FILLRULE_NONZERO = 0,
        NSVG_FILLRULE_EVENODD = 1
    }

    [Flags]
    public enum NSVGflags : byte
    {
        NSVG_FLAGS_NONE = 0x00,
        NSVG_FLAGS_VISIBLE = 0x01
    }

#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/118
    [Flags]
    public enum NSVGvisibility : byte
    {
        NSVG_VIS_NONE = 0x00,
        NSVG_VIS_DISPLAY = 0x01,
        NSVG_VIS_VISIBLE = 0x02
    }
#endif

    public enum NSVGpaintOrder : byte
    {
        NSVG_PAINT_FILL = 0x00,
        NSVG_PAINT_MARKERS = 0x01,
        NSVG_PAINT_STROKE = 0x02,
    }

    public enum NSVGgradientUnits : byte
    {
        NSVG_USER_SPACE = 0,
        NSVG_OBJECT_SPACE = 1
    }

    public enum NSVGunits : byte
    {
        NSVG_UNITS_USER,
        NSVG_UNITS_PX,
        NSVG_UNITS_PT,
        NSVG_UNITS_PC,
        NSVG_UNITS_MM,
        NSVG_UNITS_CM,
        NSVG_UNITS_IN,
        NSVG_UNITS_PERCENT,
        NSVG_UNITS_EM,
        NSVG_UNITS_EX
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSVGgradientStop
    {
        public uint color;
        public float offset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGgradient
    {
        public fixed float xform[6];
        public NSVGspreadType spread;
        public float fx, fy;
        public int nstops;
        private NSVGgradientStop _stops;

        public NSVGgradientStop* stops
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (NSVGgradientStop*)Unsafe.AsPointer(in _stops);
        }

        public ReadOnlySpan<NSVGgradientStop> Stops
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(stops, nstops);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGpaint
    {
        public NSVGpaintType type;
        public _UNSVGpaint union;

        [StructLayout(LayoutKind.Explicit)]
        public struct _UNSVGpaint
        {
            [FieldOffset(0)]
            public uint color;

            [FieldOffset(0)]
            public NSVGgradient* gradient;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGpath
    {
        public float* pts;              // Cubic bezier points: x0,y0, [cpx1,cpx1,cpx2,cpy2,x1,y1], ...
        public int npts;                // Total number of bezier points.
        public bool closed;             // Flag indicating if shapes should be treated as closed.
        public fixed float bounds[4];   // Tight bounding box of the shape [minx,miny,maxx,maxy].
        public NSVGpath* next;		    // Pointer to next path, or NULL if last element.
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGshape
    {
        public fixed byte id[64];                       // Optional 'id' attr of the shape or its group
        public NSVGpaint fill;                          // Fill paint
        public NSVGpaint stroke;                        // Stroke paint
        public float opacity;                           // Opacity of the shape.
        public float strokeWidth;                       // Stroke width (scaled).
        public float strokeDashOffset;                  // Stroke dash offset (scaled).
        public fixed float strokeDashArray[8];          // Stroke dash array (scaled).
        public byte strokeDashCount;                    // Number of dash values in dash array.
        public NSVGlineJoin strokeLineJoin;             // Stroke join type.
        public NSVGlineCap strokeLineCap;               // Stroke cap type.
        public float miterLimit;                        // Miter limit
        public NSVGfillRule fillRule;                   // Fill rule, see NSVGfillRule.
        public byte paintOrder;                         // Encoded paint order (3×2-bit fields) see NSVGpaintOrder
        public NSVGflags flags;                         // Logical or of NSVG_FLAGS_* flags
        public fixed float bounds[4];                   // Tight bounding box of the shape [minx,miny,maxx,maxy].
        public fixed byte fillGradient[64];             // Optional 'id' of fill gradient
        public fixed byte strokeGradient[64];           // Optional 'id' of stroke gradient
        public fixed float xform[6];                    // Root transformation for fill/stroke gradient
        public NSVGpath* paths;                         // Linked list of paths in the image.
        public NSVGshape* next;		                    // Pointer to next shape, or NULL if last element.
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGimage
    {
        public float width;         // Width of the image.
        public float height;        // Height of the image.
        public NSVGshape* shapes;   // Linked list of shapes in the image.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSVGcoordinate
    {
        public float value;
        public int units;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSVGlinearData
    {
        public NSVGcoordinate x1, y1, x2, y2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSVGradialData
    {
        public NSVGcoordinate cx, cy, r, fx, fy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGgradientData
    {
        public fixed byte id[64];
        public fixed byte @ref[64];
        public NSVGpaintType type;
        private _UNSVGgradientData _union;

        [UnscopedRef]
        public ref NSVGlinearData linear
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _union.linear;
        }

        [UnscopedRef]
        public ref NSVGradialData radial
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _union.radial;
        }

        public NSVGspreadType spread;
        public byte units;
        public fixed float xform[6];
        public int nstops;
        public NSVGgradientStop* stops;
        public NSVGgradientData* next;

        [StructLayout(LayoutKind.Explicit)]
        private struct _UNSVGgradientData
        {
            [FieldOffset(0)]
            public NSVGlinearData linear;

            [FieldOffset(0)]
            public NSVGradialData radial;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGattrib
    {
        public fixed byte id[64];
        public fixed float xform[6];
        public uint fillColor;
        public uint strokeColor;
        public float opacity;
        public float fillOpacity;
        public float strokeOpacity;
        public fixed byte fillGradient[64];
        public fixed byte strokeGradient[64];
        public float strokeWidth;
        public float strokeDashOffset;
        public fixed float strokeDashArray[NSVG_MAX_DASHES];
        public int strokeDashCount;
        public NSVGlineJoin strokeLineJoin;
        public NSVGlineCap strokeLineCap;
        public float miterLimit;
        public NSVGfillRule fillRule;
        public float fontSize;
        public uint stopColor;
        public float stopOpacity;
        public float stopOffset;
        public byte hasFill;
        public byte hasStroke;
#if NANOSVG_PORT_DISABLE_PATCHES
        public bool visible;
#else // https://github.com/memononen/nanosvg/pull/118
        public NSVGvisibility visible;
#endif
        public byte paintOrder;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGparser
    {
        public NSVGparser_NSVGattrib_Array attr;
        public int attrHead;
        public float* pts;
        public int npts;
        public int cpts;
        public NSVGpath* plist;
        public NSVGimage* image;
        public NSVGgradientData* gradients;
        public NSVGshape* shapesTail;
        public float viewMinx, viewMiny, viewWidth, viewHeight;
        public int alignX, alignY, alignType;
        public float dpi;
        public bool pathFlag;
        public bool defsFlag;

        [InlineArray(NSVG_MAX_ATTR)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct NSVGparser_NSVGattrib_Array
        {
            public NSVGattrib e0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NSVGNamedColor
    {
        public byte* name;
        public uint color;
    }

    public static class Constants
    {
        public const float NSVG_PI = 3.14159265358979323846264338327f;
        public const float NSVG_KAPPA90 = 0.5522847493f;

        public const int NSVG_ALIGN_MIN = 0;
        public const int NSVG_ALIGN_MID = 1;
        public const int NSVG_ALIGN_MAX = 2;
        public const int NSVG_ALIGN_NONE = 0;
        public const int NSVG_ALIGN_MEET = 1;
        public const int NSVG_ALIGN_SLICE = 2;

        public const int NSVG_MAX_ATTR = 128;
        public const int NSVG_MAX_DASHES = 8;

        public const double NSVG_EPSILON = 1e-12;

        public const int NSVG_XML_TAG = 1;
        public const int NSVG_XML_CONTENT = 2;
        public const int NSVG_XML_MAX_ATTRIBS = 256;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NSVG_RGB(uint r, uint g, uint b) => (r) | (g << 8) | (b << 16);

#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/163
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NSVG_RGBA(uint r, uint g, uint b, uint a) => (r) | (g << 8) | (b << 16) | (a << 24);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool nsvg__isspace(byte c)
        {
            return " \t\n\v\f\r"u8.IndexOf(c) is not -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool nsvg__isdigit(byte c)
        {
            return c >= (byte)'0' && c <= (byte)'9';
        }

        internal static unsafe readonly NSVGNamedColor[] nsvg__colors =
        [
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("red"u8)), color = NSVG_RGB(255, 0, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("green"u8)), color = NSVG_RGB( 0, 128, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("blue"u8)), color = NSVG_RGB( 0, 0, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("yellow"u8)), color = NSVG_RGB(255, 255, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cyan"u8)), color = NSVG_RGB( 0, 255, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("magenta"u8)), color = NSVG_RGB(255, 0, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("black"u8)), color = NSVG_RGB( 0, 0, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("grey"u8)), color = NSVG_RGB(128, 128, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("gray"u8)), color = NSVG_RGB(128, 128, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("white"u8)), color = NSVG_RGB(255, 255, 255) },

#if NANOSVG_ALL_COLOR_KEYWORDS
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("aliceblue"u8)), color = NSVG_RGB(240, 248, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("antiquewhite"u8)), color = NSVG_RGB(250, 235, 215) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("aqua"u8)), color = NSVG_RGB( 0, 255, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("aquamarine"u8)), color = NSVG_RGB(127, 255, 212) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("azure"u8)), color = NSVG_RGB(240, 255, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("beige"u8)), color = NSVG_RGB(245, 245, 220) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("bisque"u8)), color = NSVG_RGB(255, 228, 196) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("blanchedalmond"u8)), color = NSVG_RGB(255, 235, 205) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("blueviolet"u8)), color = NSVG_RGB(138, 43, 226) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("brown"u8)), color = NSVG_RGB(165, 42, 42) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("burlywood"u8)), color = NSVG_RGB(222, 184, 135) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cadetblue"u8)), color = NSVG_RGB( 95, 158, 160) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("chartreuse"u8)), color = NSVG_RGB(127, 255, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("chocolate"u8)), color = NSVG_RGB(210, 105, 30) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("coral"u8)), color = NSVG_RGB(255, 127, 80) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cornflowerblue"u8)), color = NSVG_RGB(100, 149, 237) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cornsilk"u8)), color = NSVG_RGB(255, 248, 220) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("crimson"u8)), color = NSVG_RGB(220, 20, 60) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkblue"u8)), color = NSVG_RGB( 0, 0, 139) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkcyan"u8)), color = NSVG_RGB( 0, 139, 139) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkgoldenrod"u8)), color = NSVG_RGB(184, 134, 11) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkgray"u8)), color = NSVG_RGB(169, 169, 169) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkgreen"u8)), color = NSVG_RGB( 0, 100, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkgrey"u8)), color = NSVG_RGB(169, 169, 169) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkkhaki"u8)), color = NSVG_RGB(189, 183, 107) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkmagenta"u8)), color = NSVG_RGB(139, 0, 139) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkolivegreen"u8)), color = NSVG_RGB( 85, 107, 47) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkorange"u8)), color = NSVG_RGB(255, 140, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkorchid"u8)), color = NSVG_RGB(153, 50, 204) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkred"u8)), color = NSVG_RGB(139, 0, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darksalmon"u8)), color = NSVG_RGB(233, 150, 122) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkseagreen"u8)), color = NSVG_RGB(143, 188, 143) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkslateblue"u8)), color = NSVG_RGB( 72, 61, 139) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkslategray"u8)), color = NSVG_RGB( 47, 79, 79) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkslategrey"u8)), color = NSVG_RGB( 47, 79, 79) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkturquoise"u8)), color = NSVG_RGB( 0, 206, 209) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("darkviolet"u8)), color = NSVG_RGB(148, 0, 211) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("deeppink"u8)), color = NSVG_RGB(255, 20, 147) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("deepskyblue"u8)), color = NSVG_RGB( 0, 191, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("dimgray"u8)), color = NSVG_RGB(105, 105, 105) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("dimgrey"u8)), color = NSVG_RGB(105, 105, 105) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("dodgerblue"u8)), color = NSVG_RGB( 30, 144, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("firebrick"u8)), color = NSVG_RGB(178, 34, 34) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("floralwhite"u8)), color = NSVG_RGB(255, 250, 240) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("forestgreen"u8)), color = NSVG_RGB( 34, 139, 34) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fuchsia"u8)), color = NSVG_RGB(255, 0, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("gainsboro"u8)), color = NSVG_RGB(220, 220, 220) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ghostwhite"u8)), color = NSVG_RGB(248, 248, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("gold"u8)), color = NSVG_RGB(255, 215, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("goldenrod"u8)), color = NSVG_RGB(218, 165, 32) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("greenyellow"u8)), color = NSVG_RGB(173, 255, 47) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("honeydew"u8)), color = NSVG_RGB(240, 255, 240) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("hotpink"u8)), color = NSVG_RGB(255, 105, 180) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("indianred"u8)), color = NSVG_RGB(205, 92, 92) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("indigo"u8)), color = NSVG_RGB( 75, 0, 130) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ivory"u8)), color = NSVG_RGB(255, 255, 240) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("khaki"u8)), color = NSVG_RGB(240, 230, 140) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lavender"u8)), color = NSVG_RGB(230, 230, 250) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lavenderblush"u8)), color = NSVG_RGB(255, 240, 245) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lawngreen"u8)), color = NSVG_RGB(124, 252, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lemonchiffon"u8)), color = NSVG_RGB(255, 250, 205) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightblue"u8)), color = NSVG_RGB(173, 216, 230) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightcoral"u8)), color = NSVG_RGB(240, 128, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightcyan"u8)), color = NSVG_RGB(224, 255, 255) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightgoldenrodyellow"u8)), color = NSVG_RGB(250, 250, 210) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightgray"u8)), color = NSVG_RGB(211, 211, 211) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightgreen"u8)), color = NSVG_RGB(144, 238, 144) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightgrey"u8)), color = NSVG_RGB(211, 211, 211) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightpink"u8)), color = NSVG_RGB(255, 182, 193) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightsalmon"u8)), color = NSVG_RGB(255, 160, 122) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightseagreen"u8)), color = NSVG_RGB( 32, 178, 170) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightskyblue"u8)), color = NSVG_RGB(135, 206, 250) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightslategray"u8)), color = NSVG_RGB(119, 136, 153) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightslategrey"u8)), color = NSVG_RGB(119, 136, 153) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightsteelblue"u8)), color = NSVG_RGB(176, 196, 222) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lightyellow"u8)), color = NSVG_RGB(255, 255, 224) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("lime"u8)), color = NSVG_RGB( 0, 255, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("limegreen"u8)), color = NSVG_RGB( 50, 205, 50) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("linen"u8)), color = NSVG_RGB(250, 240, 230) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("maroon"u8)), color = NSVG_RGB(128, 0, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumaquamarine"u8)), color = NSVG_RGB(102, 205, 170) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumblue"u8)), color = NSVG_RGB( 0, 0, 205) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumorchid"u8)), color = NSVG_RGB(186, 85, 211) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumpurple"u8)), color = NSVG_RGB(147, 112, 219) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumseagreen"u8)), color = NSVG_RGB( 60, 179, 113) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumslateblue"u8)), color = NSVG_RGB(123, 104, 238) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumspringgreen"u8)), color = NSVG_RGB( 0, 250, 154) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumturquoise"u8)), color = NSVG_RGB( 72, 209, 204) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mediumvioletred"u8)), color = NSVG_RGB(199, 21, 133) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("midnightblue"u8)), color = NSVG_RGB( 25, 25, 112) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mintcream"u8)), color = NSVG_RGB(245, 255, 250) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("mistyrose"u8)), color = NSVG_RGB(255, 228, 225) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("moccasin"u8)), color = NSVG_RGB(255, 228, 181) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("navajowhite"u8)), color = NSVG_RGB(255, 222, 173) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("navy"u8)), color = NSVG_RGB( 0, 0, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("oldlace"u8)), color = NSVG_RGB(253, 245, 230) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("olive"u8)), color = NSVG_RGB(128, 128, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("olivedrab"u8)), color = NSVG_RGB(107, 142, 35) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("orange"u8)), color = NSVG_RGB(255, 165, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("orangered"u8)), color = NSVG_RGB(255, 69, 0) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("orchid"u8)), color = NSVG_RGB(218, 112, 214) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("palegoldenrod"u8)), color = NSVG_RGB(238, 232, 170) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("palegreen"u8)), color = NSVG_RGB(152, 251, 152) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("paleturquoise"u8)), color = NSVG_RGB(175, 238, 238) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("palevioletred"u8)), color = NSVG_RGB(219, 112, 147) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("papayawhip"u8)), color = NSVG_RGB(255, 239, 213) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("peachpuff"u8)), color = NSVG_RGB(255, 218, 185) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("peru"u8)), color = NSVG_RGB(205, 133, 63) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("pink"u8)), color = NSVG_RGB(255, 192, 203) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("plum"u8)), color = NSVG_RGB(221, 160, 221) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("powderblue"u8)), color = NSVG_RGB(176, 224, 230) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("purple"u8)), color = NSVG_RGB(128, 0, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rosybrown"u8)), color = NSVG_RGB(188, 143, 143) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("royalblue"u8)), color = NSVG_RGB( 65, 105, 225) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("saddlebrown"u8)), color = NSVG_RGB(139, 69, 19) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("salmon"u8)), color = NSVG_RGB(250, 128, 114) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("sandybrown"u8)), color = NSVG_RGB(244, 164, 96) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("seagreen"u8)), color = NSVG_RGB( 46, 139, 87) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("seashell"u8)), color = NSVG_RGB(255, 245, 238) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("sienna"u8)), color = NSVG_RGB(160, 82, 45) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("silver"u8)), color = NSVG_RGB(192, 192, 192) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("skyblue"u8)), color = NSVG_RGB(135, 206, 235) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("slateblue"u8)), color = NSVG_RGB(106, 90, 205) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("slategray"u8)), color = NSVG_RGB(112, 128, 144) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("slategrey"u8)), color = NSVG_RGB(112, 128, 144) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("snow"u8)), color = NSVG_RGB(255, 250, 250) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("springgreen"u8)), color = NSVG_RGB( 0, 255, 127) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("steelblue"u8)), color = NSVG_RGB( 70, 130, 180) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("tan"u8)), color = NSVG_RGB(210, 180, 140) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("teal"u8)), color = NSVG_RGB( 0, 128, 128) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("thistle"u8)), color = NSVG_RGB(216, 191, 216) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("tomato"u8)), color = NSVG_RGB(255, 99, 71) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("turquoise"u8)), color = NSVG_RGB( 64, 224, 208) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("violet"u8)), color = NSVG_RGB(238, 130, 238) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("wheat"u8)), color = NSVG_RGB(245, 222, 179) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("whitesmoke"u8)), color = NSVG_RGB(245, 245, 245) },
            new() { name = (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("yellowgreen"u8)), color = NSVG_RGB(154, 205, 50) },
#endif
        ];
    }

    file static unsafe class Parser
    {
        internal static void nsvg__parseContent(byte* s, delegate*<void*, byte*, void> contentCb, void* ud)
        {
            // Trim start white spaces
            while (*s is not 0 && nsvg__isspace(*s)) s++;
            if (*s is 0) return;

            if (contentCb is not null)
                contentCb(ud, s);
        }

        internal static void nsvg__parseElement(byte* s,
                                                delegate*<void*, byte*, byte**, void> startelCb,
                                                delegate*<void*, byte*, void> endelCb,
                                                void* ud)
        {

            byte** attr = stackalloc byte*[NSVG_XML_MAX_ATTRIBS];
            int nattr = 0;
            byte* name;
            int start = 0;
            int end = 0;
            byte quote;

            // Skip white space after the '<'
            while (*s is not 0 && nsvg__isspace(*s)) s++;

            // Check if the tag is end tag
            if (*s == (byte)'/') {
                s++;
                end = 1;
            } else {
                start = 1;
            }

            // Skip comments, data and preprocessor stuff.
            if (*s is 0 || *s == (byte)'?' || *s == (byte)'!')
                return;

            // Get tag name
            name = s;
            while (*s is not 0 && !nsvg__isspace(*s)) s++;
            if (*s is not 0) { *s++ = (byte)'\0'; }

            // Get attribs
            while (end is 0 && *s is not 0 && nattr < NSVG_XML_MAX_ATTRIBS - 3)
            {
                byte* _name = null;
                byte* value = null;

                // Skip white space before the attrib name
                while (*s is not 0 && nsvg__isspace(*s)) s++;
                if (*s is 0) break;
                if (*s == (byte)'/')
                {
                    end = 1;
                    break;
                }
                _name = s;
                // Find end of the attrib name.
                while (*s is not 0 && !nsvg__isspace(*s) && *s != (byte)'=') s++;
                if (*s is not 0) { *s++ = (byte)'\0'; }
                // Skip until the beginning of the value.
                while (*s is not 0 && *s != (byte)'\"' && *s != (byte)'\'') s++;
                if (*s is 0) break;
                quote = *s;
                s++;
                // Store value and find the end of it.
                value = s;
                while (*s is not 0 && *s != quote) s++;
                if (*s is not 0) { *s++ = (byte)'\0'; }

                // Store only well formed attributes
                if (_name is not null && value is not null)
                {
                    attr[nattr++] = _name;
                    attr[nattr++] = value;
                }
            }

            // List terminator
            attr[nattr++] = null;
            attr[nattr++] = null;

            // Call callbacks.
            if (start > 0 && startelCb is not null)
                startelCb(ud, name, attr);

            if (end > 0 && endelCb is not null)
                endelCb(ud, name);
        }

        internal static int nsvg__parseXML(byte* input,
                                           delegate*<void*, byte*, byte**, void> startelCb,
                                           delegate*<void*, byte*, void> endelCb,
                                           delegate*<void*, byte*, void> contentCb,
                                           void* ud)
        {
            byte* s = input;
            byte* mark = s;
            int state = NSVG_XML_CONTENT;

            while (*s is not 0) {
                if (*s == (byte)'<' && state == NSVG_XML_CONTENT) {
                    // Start of a tag
                    *s++ = (byte)'\0';
                    nsvg__parseContent(mark, contentCb, ud);
                    mark = s;
                    state = NSVG_XML_TAG;
                } else if (*s == (byte)'>' && state == NSVG_XML_TAG) {
                    // Start of a content or new tag.
                    *s++ = (byte)'\0';
                    nsvg__parseElement(mark, startelCb, endelCb, ud);
                    mark = s;
                    state = NSVG_XML_CONTENT;
                } else {
                    s++;
                }
            }

            return 1;
        }
    }

    public static unsafe class NanoSVG
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* malloc(int size)
        {
            return NativeMemory.Alloc((nuint)size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* realloc(void* ptr, int size)
        {
            return NativeMemory.Realloc(ptr, (nuint)size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void free(void* ptr)
        {
            NativeMemory.Free(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void memcpy(void* dest, void* src, int n)
        {
            NativeMemory.Copy(src, dest, (nuint)n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void memset(void* dest, byte value, int n)
        {
            NativeMemory.Fill(dest, (nuint)n, value);
        }

        private static int strcmp(byte* s1, byte* s2)
        {
            while (*s1 is not 0 && *s1 == *s2)
            {
                s1++;
                s2++;
            }

            return *s1 - *s2;
        }

        private static int strncmp(byte* s1, byte* s2, int n)
        {
            while (n > 0 && *s1 is not 0 && *s1 == *s2)
            {
                s1++;
                s2++;
                n--;
            }
            if (n is 0)
                return 0;
            return *s1 - *s2;
        }

        private static int strlen(byte* s)
        {
            int len = 0;
            while (*s is not 0)
            {
                len++;
                s++;
            }

            return len;
        }

        private static byte* strncpy(byte* dest, byte* src, int n)
        {
            byte* d = dest;
            while (n > 0 && *src is not 0)
            {
                *d++ = *src++;
                n--;
            }
            while (n > 0)
            {
                *d++ = (byte)'\0';
                n--;
            }
            return dest;
        }

        private static int strstr(byte* haystack, byte* needle)
        {
            int hlen = strlen(haystack);
            int nlen = strlen(needle);
            for (int i = 0; i <= hlen - nlen; i++)
            {
                if (strncmp(haystack + i, needle, nlen) == 0)
                    return i;
            }
            return -1;
        }

        private static void nsvg__xformIdentity(float* t)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        private static void nsvg__xformSetTranslation(float* t, float tx, float ty)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = tx; t[5] = ty;
        }

        private static void nsvg__xformSetScale(float* t, float sx, float sy)
        {
            t[0] = sx; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = sy;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        private static void nsvg__xformSetSkewX(float* t, float a)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = MathF.Tan(a); t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        private static void nsvg__xformSetSkewY(float* t, float a)
        {
            t[0] = 1.0f; t[1] = MathF.Tan(a);
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        private static void nsvg__xformSetRotation(float* t, float a)
        {
            float cs = MathF.Cos(a), sn = MathF.Sin(a);
            t[0] = cs; t[1] = sn;
            t[2] = -sn; t[3] = cs;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        private static void nsvg__xformMultiply(float* t, float* s)
        {
            float t0 = t[0] * s[0] + t[1] * s[2];
            float t2 = t[2] * s[0] + t[3] * s[2];
            float t4 = t[4] * s[0] + t[5] * s[2] + s[4];
            t[1] = t[0] * s[1] + t[1] * s[3];
            t[3] = t[2] * s[1] + t[3] * s[3];
            t[5] = t[4] * s[1] + t[5] * s[3] + s[5];
            t[0] = t0;
            t[2] = t2;
            t[4] = t4;
        }

#if NANOSVG_NO_XFORM_INVERSE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void nsvg__xformInverse(float* inv, float* t)
        {
#if !NANOSVG_NO_XFORM_INVERSE
            double invdet, det = (double)t[0] * t[3] - (double)t[2] * t[1];
            if (det > -1e-6 && det < 1e-6)
            {
                nsvg__xformIdentity(t);
                return;
            }
            invdet = 1.0 / det;
            inv[0] = (float)(t[3] * invdet);
            inv[2] = (float)(-t[2] * invdet);
            inv[4] = (float)(((double)t[2] * t[5] - (double)t[3] * t[4]) * invdet);
            inv[1] = (float)(-t[1] * invdet);
            inv[3] = (float)(t[0] * invdet);
            inv[5] = (float)(((double)t[1] * t[4] - (double)t[0] * t[5]) * invdet);
#else
            memcpy(inv, t, sizeof(float) * 6);
#endif
        }

        private static void nsvg__xformPremultiply(float* t, float* s)
        {
            float* s2 = stackalloc float[6];
            memcpy(s2, s, sizeof(float) * 6);
            nsvg__xformMultiply(s2, t);
            memcpy(t, s2, sizeof(float) * 6);
        }

        private static void nsvg__xformPoint(float* dx, float* dy, float x, float y, float* t)
        {
            *dx = x * t[0] + y * t[2] + t[4];
            *dy = x * t[1] + y * t[3] + t[5];
        }

        private static void nsvg__xformVec(float* dx, float* dy, float x, float y, float* t)
        {
            *dx = x * t[0] + y * t[2];
            *dy = x * t[1] + y * t[3];
        }

        private static bool nsvg__ptInBounds(float* pt, float* bounds)
        {
            return pt[0] >= bounds[0] && pt[0] <= bounds[2] && pt[1] >= bounds[1] && pt[1] <= bounds[3];
        }

        private static double nsvg__evalBezier(double t, double p0, double p1, double p2, double p3)
        {
            double it = 1.0 - t;
            return it * it * it * p0 + 3.0 * it * it * t * p1 + 3.0 * it * t * t * p2 + t * t * t * p3;
        }

        private static void nsvg__curveBounds(float* bounds, float* curve)
        {
            int i, j, count;
            double* roots = stackalloc double[2];
            double a, b, c, b2ac, t, v;
            float* v0 = &curve[0];
            float* v1 = &curve[2];
            float* v2 = &curve[4];
            float* v3 = &curve[6];

            // Start the bounding box by end points
            bounds[0] = MathF.Min(v0[0], v3[0]);
            bounds[1] = MathF.Min(v0[1], v3[1]);
            bounds[2] = MathF.Max(v0[0], v3[0]);
            bounds[3] = MathF.Max(v0[1], v3[1]);

            // Bezier curve fits inside the convex hull of it's control points.
            // If control points are inside the bounds, we're done.
            if (nsvg__ptInBounds(v1, bounds) && nsvg__ptInBounds(v2, bounds))
                return;

            // Add bezier curve inflection points in X and Y.
            for (i = 0; i < 2; i++)
            {
                a = -3.0 * v0[i] + 9.0 * v1[i] - 9.0 * v2[i] + 3.0 * v3[i];
                b = 6.0 * v0[i] - 12.0 * v1[i] + 6.0 * v2[i];
                c = 3.0 * v1[i] - 3.0 * v0[i];
                count = 0;
                if (Math.Abs(a) < NSVG_EPSILON)
                {
                    if (Math.Abs(b) > NSVG_EPSILON)
                    {
                        t = -c / b;
                        if (t > NSVG_EPSILON && t < 1.0 - NSVG_EPSILON)
                            roots[count++] = t;
                    }
                }
                else
                {
                    b2ac = b * b - 4.0 * c * a;
                    if (b2ac > NSVG_EPSILON)
                    {
                        t = (-b + Math.Sqrt(b2ac)) / (2.0 * a);
                        if (t > NSVG_EPSILON && t < 1.0 - NSVG_EPSILON)
                            roots[count++] = t;
                        t = (-b - Math.Sqrt(b2ac)) / (2.0 * a);
                        if (t > NSVG_EPSILON && t < 1.0 - NSVG_EPSILON)
                            roots[count++] = t;
                    }
                }
                for (j = 0; j < count; j++)
                {
                    v = nsvg__evalBezier(roots[j], v0[i], v1[i], v2[i], v3[i]);
                    bounds[0 + i] = MathF.Min(bounds[0 + i], (float)v);
                    bounds[2 + i] = MathF.Max(bounds[2 + i], (float)v);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte nsvg__encodePaintOrder(NSVGpaintOrder a, NSVGpaintOrder b, NSVGpaintOrder c)
        {
            return (byte)(((int)a & 0x03) | (((int)b & 0x03) << 2) | (((int)c & 0x03) << 4));
        }

        private static NSVGparser* nsvg__createParser()
        {
            NSVGparser* p;
            p = (NSVGparser*)malloc(sizeof(NSVGparser));
            if (p is null) goto error;
            memset(p, 0, sizeof(NSVGparser));

            p->image = (NSVGimage*)malloc(sizeof(NSVGimage));
            if (p->image is null) goto error;
            memset(p->image, 0, sizeof(NSVGimage));

            // Init style
            nsvg__xformIdentity(p->attr[0].xform);
            memset(p->attr[0].id, 0, 64);
            p->attr[0].fillColor = NSVG_RGB(0, 0, 0);
            p->attr[0].strokeColor = NSVG_RGB(0, 0, 0);
            p->attr[0].opacity = 1;
            p->attr[0].fillOpacity = 1;
            p->attr[0].strokeOpacity = 1;
            p->attr[0].stopOpacity = 1;
            p->attr[0].strokeWidth = 1;
            p->attr[0].strokeLineJoin = (byte)NSVGlineJoin.NSVG_JOIN_MITER;
            p->attr[0].strokeLineCap = (byte)NSVGlineCap.NSVG_CAP_BUTT;
            p->attr[0].miterLimit = 4;
            p->attr[0].fillRule = (byte)NSVGfillRule.NSVG_FILLRULE_NONZERO;
            p->attr[0].hasFill = 1;

#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/118
            p->attr[0].visible = NSVGvisibility.NSVG_VIS_DISPLAY | NSVGvisibility.NSVG_VIS_VISIBLE;
#else
            p->attr[0].visible = true;
#endif

            p->attr[0].paintOrder = nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_MARKERS);

            return p;

        error:
            if (p is not null)
            {
                if (p->image is not null) free(p->image);
                free(p);
            }
            return null;
        }

        private static void nsvg__deletePaths(NSVGpath* path)
        {
            while (path is not null)
            {
                NSVGpath* next = path->next;
                if (path->pts is not null)
                    free(path->pts);
                free(path);
                path = next;
            }
        }

        private static void nsvg__deletePaint(NSVGpaint* paint)
        {
            if (paint->type == NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT || paint->type == NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT)
                free(paint->union.gradient);
        }

        private static void nsvg__deleteGradientData(NSVGgradientData* grad)
        {
            NSVGgradientData* next;
            while (grad != null)
            {
                next = grad->next;
                free(grad->stops);
                free(grad);
                grad = next;
            }
        }

        private static void nsvg__deleteParser(NSVGparser* p)
        {
            if (p != null)
            {
                nsvg__deletePaths(p->plist);
                nsvg__deleteGradientData(p->gradients);
                nsvgDelete(p->image);
                free(p->pts);
                free(p);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void nsvg__resetPath(NSVGparser* p)
        {
            p->npts = 0;
        }

        private static void nsvg__addPoint(NSVGparser* p, float x, float y)
        {
            if (p->npts + 1 > p->cpts)
            {
                p->cpts = p->cpts > 0 ? p->cpts * 2 : 8;
                p->pts = (float*)realloc(p->pts, p->cpts * 2 * sizeof(float));
                if (p->pts is null) return;
            }
            p->pts[p->npts * 2 + 0] = x;
            p->pts[p->npts * 2 + 1] = y;
            p->npts++;
        }

        private static void nsvg__moveTo(NSVGparser* p, float x, float y)
        {
            if (p->npts > 0)
            {
                p->pts[(p->npts - 1) * 2 + 0] = x;
                p->pts[(p->npts - 1) * 2 + 1] = y;
            }
            else
            {
                nsvg__addPoint(p, x, y);
            }
        }

        private static void nsvg__lineTo(NSVGparser* p, float x, float y)
        {
            float px, py, dx, dy;
            if (p->npts > 0)
            {
                px = p->pts[(p->npts - 1) * 2 + 0];
                py = p->pts[(p->npts - 1) * 2 + 1];
                dx = x - px;
                dy = y - py;
                nsvg__addPoint(p, px + dx / 3.0f, py + dy / 3.0f);
                nsvg__addPoint(p, x - dx / 3.0f, y - dy / 3.0f);
                nsvg__addPoint(p, x, y);
            }
        }

        private static void nsvg__cubicBezTo(NSVGparser* p, float cpx1, float cpy1, float cpx2, float cpy2, float x, float y)
        {
            if (p->npts > 0)
            {
                nsvg__addPoint(p, cpx1, cpy1);
                nsvg__addPoint(p, cpx2, cpy2);
                nsvg__addPoint(p, x, y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NSVGattrib* nsvg__getAttr(NSVGparser* p)
        {
            return &p->attr[p->attrHead];
        }

        private static void nsvg__pushAttr(NSVGparser* p)
        {
            if (p->attrHead < NSVG_MAX_ATTR - 1)
            {
                p->attrHead++;
                memcpy(&p->attr[p->attrHead], &p->attr[p->attrHead - 1], sizeof(NSVGattrib));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void nsvg__popAttr(NSVGparser* p)
        {
            if (p->attrHead > 0)
                p->attrHead--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__actualOrigX(NSVGparser* p)
        {
            return p->viewMinx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__actualOrigY(NSVGparser* p)
        {
            return p->viewMiny;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__actualWidth(NSVGparser* p)
        {
            return p->viewWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__actualHeight(NSVGparser* p)
        {
            return p->viewHeight;
        }

        private static float nsvg__actualLength(NSVGparser* p)
        {
            float w = nsvg__actualWidth(p), h = nsvg__actualHeight(p);
            return MathF.Sqrt(w * w + h * h) / MathF.Sqrt(2.0f);
        }

        private static float nsvg__convertToPixels(NSVGparser* p, NSVGcoordinate c, float orig, float length)
        {
            NSVGattrib* attr = nsvg__getAttr(p);
            switch (c.units)
            {
                case (int)NSVGunits.NSVG_UNITS_USER: return c.value;
                case (int)NSVGunits.NSVG_UNITS_PX: return c.value;
                case (int)NSVGunits.NSVG_UNITS_PT: return c.value / 72.0f * p->dpi;
                case (int)NSVGunits.NSVG_UNITS_PC: return c.value / 6.0f * p->dpi;
                case (int)NSVGunits.NSVG_UNITS_MM: return c.value / 25.4f * p->dpi;
                case (int)NSVGunits.NSVG_UNITS_CM: return c.value / 2.54f * p->dpi;
                case (int)NSVGunits.NSVG_UNITS_IN: return c.value * p->dpi;
                case (int)NSVGunits.NSVG_UNITS_EM: return c.value * attr->fontSize;
                case (int)NSVGunits.NSVG_UNITS_EX: return c.value * attr->fontSize * 0.52f; // x-height of Helvetica.
                case (int)NSVGunits.NSVG_UNITS_PERCENT: return orig + c.value / 100.0f * length;
                default: return c.value;
            }
        }

        private static NSVGgradientData* nsvg__findGradientData(NSVGparser* p, byte* id)
        {
            NSVGgradientData* grad = p->gradients;
            if (id == null || *id == (byte)'\0')
                return null;
            while (grad != null)
            {
                if (strcmp(grad->id, id) == 0)
                    return grad;
                grad = grad->next;
            }
            return null;
        }

        private static NSVGgradient* nsvg__createGradient(NSVGparser* p, byte* id, float* localBounds, float* xform, NSVGpaintType* paintType)
        {
            NSVGgradientData* data = null;
            NSVGgradientData* @ref = null;
            NSVGgradientStop* stops = null;
            NSVGgradient* grad;
            float ox, oy, sw, sh, sl;
            int nstops = 0;
            int refIter;

            data = nsvg__findGradientData(p, id);
            if (data == null) return null;

            // TODO: use ref to fill in all unset values too.
            @ref = data;
            refIter = 0;
            while (@ref != null)
            {
                NSVGgradientData* nextRef = null;
                if (stops == null && @ref->stops != null)
                {
                    stops = @ref->stops;
                    nstops = @ref->nstops;
                    break;
                }
                nextRef = nsvg__findGradientData(p, @ref->@ref);
                if (nextRef == @ref) break; // prevent infite loops on malformed data
                @ref = nextRef;
                refIter++;
                if (refIter > 32) break; // prevent infite loops on malformed data
            }
            if (stops == null) return null;

            grad = (NSVGgradient*)malloc(sizeof(NSVGgradient) + sizeof(NSVGgradientStop) * (nstops - 1));
            if (grad == null) return null;

            // The shape width and height.
            if (data->units == (byte)NSVGgradientUnits.NSVG_OBJECT_SPACE)
            {
                ox = localBounds[0];
                oy = localBounds[1];
                sw = localBounds[2] - localBounds[0];
                sh = localBounds[3] - localBounds[1];
            }
            else
            {
                ox = nsvg__actualOrigX(p);
                oy = nsvg__actualOrigY(p);
                sw = nsvg__actualWidth(p);
                sh = nsvg__actualHeight(p);
            }
            sl = MathF.Sqrt(sw * sw + sh * sh) / MathF.Sqrt(2.0f);

            if (data->type == NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT)
            {
                float x1, y1, x2, y2, dx, dy;
                x1 = nsvg__convertToPixels(p, data->linear.x1, ox, sw);
                y1 = nsvg__convertToPixels(p, data->linear.y1, oy, sh);
                x2 = nsvg__convertToPixels(p, data->linear.x2, ox, sw);
                y2 = nsvg__convertToPixels(p, data->linear.y2, oy, sh);
                // Calculate transform aligned to the line
                dx = x2 - x1;
                dy = y2 - y1;
                grad->xform[0] = dy; grad->xform[1] = -dx;
                grad->xform[2] = dx; grad->xform[3] = dy;
                grad->xform[4] = x1; grad->xform[5] = y1;
            }
            else
            {
                float cx, cy, fx, fy, r;
                cx = nsvg__convertToPixels(p, data->radial.cx, ox, sw);
                cy = nsvg__convertToPixels(p, data->radial.cy, oy, sh);
                fx = nsvg__convertToPixels(p, data->radial.fx, ox, sw);
                fy = nsvg__convertToPixels(p, data->radial.fy, oy, sh);
                r = nsvg__convertToPixels(p, data->radial.r, 0, sl);
                // Calculate transform aligned to the circle
                grad->xform[0] = r; grad->xform[1] = 0;
                grad->xform[2] = 0; grad->xform[3] = r;
                grad->xform[4] = cx; grad->xform[5] = cy;

#if NANOSVG_PORT_DISABLE_PATCHES
                grad->fx = fx / r;
                grad->fy = fy / r;
#else // https://github.com/memononen/nanosvg/pull/164
                grad->fx = (fx - cx) / r;
                grad->fy = (fy - cy) / r;
#endif
            }

            nsvg__xformMultiply(grad->xform, data->xform);
            nsvg__xformMultiply(grad->xform, xform);

            grad->spread = data->spread;
            memcpy(grad->stops, stops, nstops * sizeof(NSVGgradientStop));
            grad->nstops = nstops;

            *paintType = data->type;

            return grad;
        }

        private static float nsvg__getAverageScale(float* t)
        {
            float sx = MathF.Sqrt(t[0] * t[0] + t[2] * t[2]);
            float sy = MathF.Sqrt(t[1] * t[1] + t[3] * t[3]);
            return (sx + sy) * 0.5f;
        }

        private static void nsvg__getLocalBounds(float* bounds, NSVGshape* shape, float* xform)
        {
            NSVGpath* path;
            float* curve = stackalloc float[4 * 2], curveBounds = stackalloc float[4];
            int i, first = 1;
            for (path = shape->paths; path != null; path = path->next)
            {
                nsvg__xformPoint(&curve[0], &curve[1], path->pts[0], path->pts[1], xform);
                for (i = 0; i < path->npts - 1; i += 3)
                {
                    nsvg__xformPoint(&curve[2], &curve[3], path->pts[(i + 1) * 2], path->pts[(i + 1) * 2 + 1], xform);
                    nsvg__xformPoint(&curve[4], &curve[5], path->pts[(i + 2) * 2], path->pts[(i + 2) * 2 + 1], xform);
                    nsvg__xformPoint(&curve[6], &curve[7], path->pts[(i + 3) * 2], path->pts[(i + 3) * 2 + 1], xform);
                    nsvg__curveBounds(curveBounds, curve);
                    if (first > 0)
                    {
                        bounds[0] = curveBounds[0];
                        bounds[1] = curveBounds[1];
                        bounds[2] = curveBounds[2];
                        bounds[3] = curveBounds[3];
                        first = 0;
                    }
                    else
                    {
                        bounds[0] = MathF.Min(bounds[0], curveBounds[0]);
                        bounds[1] = MathF.Min(bounds[1], curveBounds[1]);
                        bounds[2] = MathF.Max(bounds[2], curveBounds[2]);
                        bounds[3] = MathF.Max(bounds[3], curveBounds[3]);
                    }
                    curve[0] = curve[6];
                    curve[1] = curve[7];
                }
            }
        }

        private static void nsvg__addShape(NSVGparser* p)
        {
            NSVGattrib* attr = nsvg__getAttr(p);
            float scale = 1.0f;
            NSVGshape* shape;
            NSVGpath* path;
            int i;

            if (p->plist == null)
                return;

            shape = (NSVGshape*)malloc(sizeof(NSVGshape));
            if (shape == null) goto error;
            memset(shape, 0, sizeof(NSVGshape));

            memcpy(shape->id, attr->id, 64);
            memcpy(shape->fillGradient, attr->fillGradient, 64);
            memcpy(shape->strokeGradient, attr->strokeGradient, 64);
            memcpy(shape->xform, attr->xform, sizeof(float) * 6);
            scale = nsvg__getAverageScale(attr->xform);
            shape->strokeWidth = attr->strokeWidth * scale;
            shape->strokeDashOffset = attr->strokeDashOffset * scale;
            shape->strokeDashCount = (byte)attr->strokeDashCount;
            for (i = 0; i < attr->strokeDashCount; i++)
                shape->strokeDashArray[i] = attr->strokeDashArray[i] * scale;
            shape->strokeLineJoin = attr->strokeLineJoin;
            shape->strokeLineCap = attr->strokeLineCap;
            shape->miterLimit = attr->miterLimit;
            shape->fillRule = attr->fillRule;
            shape->opacity = attr->opacity;
            shape->paintOrder = attr->paintOrder;

            shape->paths = p->plist;
            p->plist = null;

            // Calculate shape bounds
            shape->bounds[0] = shape->paths->bounds[0];
            shape->bounds[1] = shape->paths->bounds[1];
            shape->bounds[2] = shape->paths->bounds[2];
            shape->bounds[3] = shape->paths->bounds[3];
            for (path = shape->paths->next; path != null; path = path->next)
            {
                shape->bounds[0] = MathF.Min(shape->bounds[0], path->bounds[0]);
                shape->bounds[1] = MathF.Min(shape->bounds[1], path->bounds[1]);
                shape->bounds[2] = MathF.Max(shape->bounds[2], path->bounds[2]);
                shape->bounds[3] = MathF.Max(shape->bounds[3], path->bounds[3]);
            }

            // Set fill
            if (attr->hasFill == 0)
            {
                shape->fill.type = NSVGpaintType.NSVG_PAINT_NONE;
            }
            else if (attr->hasFill == 1)
            {
                shape->fill.type = NSVGpaintType.NSVG_PAINT_COLOR;
                shape->fill.union.color = attr->fillColor;
                shape->fill.union.color |= (uint)(attr->fillOpacity * 255) << 24;
            }
            else if (attr->hasFill == 2)
            {
                shape->fill.type = NSVGpaintType.NSVG_PAINT_UNDEF;
            }

            // Set stroke
            if (attr->hasStroke == 0)
            {
                shape->stroke.type = NSVGpaintType.NSVG_PAINT_NONE;
            }
            else if (attr->hasStroke == 1)
            {
                shape->stroke.type = NSVGpaintType.NSVG_PAINT_COLOR;
                shape->stroke.union.color = attr->strokeColor;
                shape->stroke.union.color |= (uint)(attr->strokeOpacity * 255) << 24;
            }
            else if (attr->hasStroke == 2)
            {
                shape->stroke.type = NSVGpaintType.NSVG_PAINT_UNDEF;
            }

            // Set flags
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/118
            shape->flags = (attr->visible & NSVGvisibility.NSVG_VIS_DISPLAY) is NSVGvisibility.NSVG_VIS_DISPLAY &&
                           (attr->visible & NSVGvisibility.NSVG_VIS_VISIBLE) is NSVGvisibility.NSVG_VIS_VISIBLE ?
                           NSVGflags.NSVG_FLAGS_VISIBLE :
                           0x00;
#else
            shape->flags = attr->visible ? NSVGflags.NSVG_FLAGS_VISIBLE : NSVGflags.NSVG_FLAGS_NONE;
#endif

            // Add to tail
            if (p->image->shapes == null)
                p->image->shapes = shape;
            else
                p->shapesTail->next = shape;
            p->shapesTail = shape;

            return;

        error:
            if (shape is not null) free(shape);
        }

        private static void nsvg__addPath(NSVGparser* p, bool closed)
        {
            NSVGattrib* attr = nsvg__getAttr(p);
            NSVGpath* path = null;
            float* bounds = stackalloc float[4];
            float* curve;
            int i;

            if (p->npts < 4)
                return;

            if (closed)
                nsvg__lineTo(p, p->pts[0], p->pts[1]);

            // Expect 1 + N*3 points (N = number of cubic bezier segments).
            if ((p->npts % 3) != 1)
                return;

            path = (NSVGpath*)malloc(sizeof(NSVGpath));
            if (path == null) goto error;
            memset(path, 0, sizeof(NSVGpath));

            path->pts = (float*)malloc(p->npts * 2 * sizeof(float));
            if (path->pts == null) goto error;
            path->closed = closed;
            path->npts = p->npts;

            // Transform path.
            for (i = 0; i < p->npts; ++i)
                nsvg__xformPoint(&path->pts[i * 2], &path->pts[i * 2 + 1], p->pts[i * 2], p->pts[i * 2 + 1], attr->xform);

            // Find bounds
            for (i = 0; i < path->npts - 1; i += 3)
            {
                curve = &path->pts[i * 2];
                nsvg__curveBounds(bounds, curve);
                if (i == 0)
                {
                    path->bounds[0] = bounds[0];
                    path->bounds[1] = bounds[1];
                    path->bounds[2] = bounds[2];
                    path->bounds[3] = bounds[3];
                }
                else
                {
                    path->bounds[0] = MathF.Min(path->bounds[0], bounds[0]);
                    path->bounds[1] = MathF.Min(path->bounds[1], bounds[1]);
                    path->bounds[2] = MathF.Max(path->bounds[2], bounds[2]);
                    path->bounds[3] = MathF.Max(path->bounds[3], bounds[3]);
                }
            }

            path->next = p->plist;
            p->plist = path;

            return;

        error:
            if (path != null)
            {
                if (path->pts != null) free(path->pts);
                free(path);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__atof(byte* s)
        {
            float.TryParse(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(s), NumberFormatInfo.InvariantInfo, out var result);
            return (float)result;

            /*byte * cur = s;
            byte* end = null;
            double res = 0.0, sign = 1.0;
            long intPart = 0, fracPart = 0;
            bool hasIntPart = false, hasFracPart = false;

            // Parse optional sign
            if (*cur == '+') {
                cur++;
            } else if (*cur == '-') {
                sign = -1;
                cur++;
            }

            // Parse integer part
            if (nsvg__isdigit(*cur)) {
                // Parse digit sequence
                intPart = strtoll(cur, &end, 10);
                if (cur != end) {
                    res = (double)intPart;
                    hasIntPart = true;
                    cur = end;
                }
            }

            // Parse fractional part.
            if (*cur == '.') {
                cur++; // Skip '.'
                if (nsvg__isdigit(*cur)) {
                    // Parse digit sequence
                    fracPart = strtoll(cur, &end, 10);
                    if (cur != end) {
                        res += (double)fracPart / Math.Pow(10.0, (double)(end - cur));
                        hasFracPart = true;
                        cur = end;
                    }
                }
            }

            // A valid number should have integer or fractional part.
            if (!hasIntPart && !hasFracPart)
                return 0.0;

            // Parse optional exponent
            if (*cur == 'e' || *cur == 'E') {
                long expPart = 0;
                cur++; // skip 'E'
                expPart = strtol(cur, &end, 10); // Parse digit sequence with sign
                if (cur != end) {
                    res *= Math.Pow(10.0, (double)expPart);
                }
            }

            return res * sign;*/
        }

        private static byte* nsvg__parseNumber(byte* s, byte* it, int size)
        {
            int last = size - 1;
            int i = 0;

            // sign
            if (*s == '-' || *s == '+')
            {
                if (i < last) it[i++] = *s;
                s++;
            }
            // integer part
            while (*s is not 0 && nsvg__isdigit(*s))
            {
                if (i < last) it[i++] = *s;
                s++;
            }
            if (*s == '.')
            {
                // decimal point
                if (i < last) it[i++] = *s;
                s++;
                // fraction part
                while (*s is not 0 && nsvg__isdigit(*s))
                {
                    if (i < last) it[i++] = *s;
                    s++;
                }
            }
            // exponent
            if ((*s == 'e' || *s == 'E') && (s[1] != 'm' && s[1] != 'x'))
            {
                if (i < last) it[i++] = *s;
                s++;
                if (*s == '-' || *s == '+')
                {
                    if (i < last) it[i++] = *s;
                    s++;
                }
                while (*s is not 0 && nsvg__isdigit(*s))
                {
                    if (i < last) it[i++] = *s;
                    s++;
                }
            }
            it[i] = (byte)'\0';

            return s;
        }

        private static byte* nsvg__getNextPathItemWhenArcFlag(byte* s, byte* it)
        {
            it[0] = (byte)'\0';
            while (*s is not 0 && (nsvg__isspace(*s) || *s == (byte)',')) s++;
            if (*s is 0) return s;
            if (*s == (byte)'0' || *s == (byte)'1')
            {
                it[0] = *s++;
                it[1] = (byte)'\0';
                return s;
            }
            return s;
        }

        private static byte* nsvg__getNextPathItem(byte* s, byte* it)
        {
            it[0] = (byte)'\0';
            // Skip white spaces and commas
            while (*s is not 0 && (nsvg__isspace(*s) || *s == (byte)',')) s++;
            if (*s is 0) return s;
            if (*s == (byte)'-' || *s == (byte)'+' || *s == (byte)'.' || nsvg__isdigit(*s))
            {
                s = nsvg__parseNumber(s, it, 64);
            }
            else
            {
                // Parse command
                it[0] = *s++;
                it[1] = (byte)'\0';
                return s;
            }

            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHexDigit(byte c) =>
            (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HexValue(byte c) =>
            c switch
            {
                >= (byte)'0' and <= (byte)'9' => (uint)(c - '0'),
                >= (byte)'a' and <= (byte)'f' => (uint)(c - 'a' + 10),
                >= (byte)'A' and <= (byte)'F' => (uint)(c - 'A' + 10),
                _ => 0
            };

        private static uint nsvg__parseColorHex(byte* str)
        {
            var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(str);
            if (!span.IsEmpty && span[0] == (byte)'#')
            {
                if (span.Length >= 7 && Utf8Parser.TryParse(span.Slice(1, 6), out uint v, out int c, 'x') && c == 6)
                    return NSVG_RGB((v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF);

                if (span.Length >= 4 && Utf8Parser.TryParse(span.Slice(1, 3), out v, out c, 'x') && c == 3)
                    return NSVG_RGB(((v >> 8) & 0xF) * 17, ((v >> 4) & 0xF) * 17, (v & 0xF) * 17);
            }

            return NSVG_RGB(128, 128, 128);
        }

        // Parse rgb color. The pointer 'str' must point at "rgb(" (4+ characters).
        // This function returns gray (rgb(128, 128, 128) == '#808080') on parse errors
        // for backwards compatibility. Note: other image viewers return black instead.

        private static uint nsvg__parseColorRGB(byte* str)
        {
            int i;
            uint* rgbi = stackalloc uint[3];
            float* rgbf = stackalloc float[3];

            bool TryParseInts()
            {
                var s = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(str);
                if (!s.StartsWith("rgb("u8)) return false;
                s = s.Slice(4);

                for (int k = 0; k < 3; k++)
                {
                    while (!s.IsEmpty && nsvg__isspace(s[0])) s = s.Slice(1);
                    if (!Utf8Parser.TryParse(s, out rgbi[k], out int c)) return false;
                    s = s.Slice(c);

                    if (k < 2)
                    {
                        while (!s.IsEmpty && nsvg__isspace(s[0])) s = s.Slice(1);
                        if (s.IsEmpty || s[0] != (byte)',') return false;
                        s = s.Slice(1);
                    }
                }

                return true;
            }

            // try decimal integers first
            if (!TryParseInts())
            {
                // integers failed, try percent values (float, locale independent)
                ReadOnlySpan<byte> delimiter = [(byte)',', (byte)',', (byte)')'];
                str += 4; // skip "rgb("
                for (i = 0; i < 3; i++)
                {
                    while (*str is not 0 && (nsvg__isspace(*str))) str++; 	// skip leading spaces
                    if (*str == '+') str++;				// skip '+' (don't allow '-')
                    if (*str is 0) break;
                    rgbf[i] = nsvg__atof(str);

                    // Note 1: it would be great if nsvg__atof() returned how many
                    // bytes it consumed but it doesn't. We need to skip the number,
                    // the '%' character, spaces, and the delimiter ',' or ')'.

                    // Note 2: The following code does not allow values like "33.%",
                    // i.e. a decimal point w/o fractional part, but this is consistent
                    // with other image viewers, e.g. firefox, chrome, eog, gimp.

                    while (*str is not 0 && nsvg__isdigit(*str)) str++;		// skip integer part
                    if (*str == (byte)'.')
                    {
                        str++;
                        if (!nsvg__isdigit(*str)) break;		// error: no digit after '.'
                        while (*str is not 0 && nsvg__isdigit(*str)) str++;	// skip fractional part
                    }
                    if (*str == '%') str++; else break;
                    while (*str is not 0 && nsvg__isspace(*str)) str++;
                    if (*str == delimiter[i]) str++;
                    else break;
                }
                if (i == 3)
                {
                    rgbi[0] = (uint)MathF.Round(rgbf[0] * 2.55f);
                    rgbi[1] = (uint)MathF.Round(rgbf[1] * 2.55f);
                    rgbi[2] = (uint)MathF.Round(rgbf[2] * 2.55f);
                }
                else
                {
                    rgbi[0] = rgbi[1] = rgbi[2] = 128;
                }
            }

            // clip values as the CSS spec requires
            for (i = 0; i < 3; i++)
            {
                if (rgbi[i] > 255) rgbi[i] = 255;
            }

            return NSVG_RGB(rgbi[0], rgbi[1], rgbi[2]);
        }

#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/163
        private static uint nsvg__parseColorRGBA(byte* str)
        {
            int i;
            uint* rgbai = stackalloc uint[4];
            float* rgbaf = stackalloc float[4];

            bool TryParseInts()
            {
                var s = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(str);
                if (!s.StartsWith("rgba("u8)) return false;
                s = s.Slice(5);

                for (int k = 0; k < 4; k++)
                {
                    while (!s.IsEmpty && nsvg__isspace(s[0])) s = s.Slice(1);
                    if (!Utf8Parser.TryParse(s, out rgbai[k], out int c)) return false;
                    s = s.Slice(c);

                    if (k < 3)
                    {
                        while (!s.IsEmpty && nsvg__isspace(s[0])) s = s.Slice(1);
                        if (s.IsEmpty || s[0] != (byte)',') return false;
                        s = s.Slice(1);
                    }
                }

                return true;
            }

            // try decimal integers first
            if (!TryParseInts())
            {
                // integers failed, try percent values (float, locale independent)
                ReadOnlySpan<byte> delimiter = [(byte)',', (byte)',', (byte)',', (byte)')'];
                str += 5; // skip "rgba("
                for (i = 0; i < 4; i++)
                {
                    while (*str is not 0 && (nsvg__isspace(*str))) str++; 	// skip leading spaces
                    if (*str == '+') str++;				// skip '+' (don't allow '-')
                    if (*str is 0) break;
                    rgbaf[i] = nsvg__atof(str);

                    // Note 1: it would be great if nsvg__atof() returned how many
                    // bytes it consumed but it doesn't. We need to skip the number,
                    // the '%' character, spaces, and the delimiter ',' or ')'.

                    // Note 2: The following code does not allow values like "33.%",
                    // i.e. a decimal point w/o fractional part, but this is consistent
                    // with other image viewers, e.g. firefox, chrome, eog, gimp.

                    while (*str is not 0 && nsvg__isdigit(*str)) str++;		// skip integer part
                    if (*str == (byte)'.')
                    {
                        str++;
                        if (!nsvg__isdigit(*str)) break;		// error: no digit after '.'
                        while (*str is not 0 && nsvg__isdigit(*str)) str++;	// skip fractional part
                    }
                    if (*str == '%') str++; else break;
                    while (*str is not 0 && nsvg__isspace(*str)) str++;
                    if (*str == delimiter[i]) str++;
                    else break;
                }
                if (i == 4)
                {
                    rgbai[0] = (uint)MathF.Round(rgbaf[0] * 2.55f);
                    rgbai[1] = (uint)MathF.Round(rgbaf[1] * 2.55f);
                    rgbai[2] = (uint)MathF.Round(rgbaf[2] * 2.55f);
                    rgbai[3] = (uint)MathF.Round(rgbaf[3] * 2.55f);
                }
                else
                {
                    rgbai[0] = rgbai[1] = rgbai[2] = rgbai[3] = 128;
                }
            }

            // clip values as the CSS spec requires
            for (i = 0; i < 4; i++)
            {
                if (rgbai[i] > 255) rgbai[i] = 255;
            }

            return NSVG_RGBA(rgbai[0], rgbai[1], rgbai[2], rgbai[3]);
        }
#endif

        private static uint nsvg__parseColorName(byte* str)
        {
            int i, ncolors = nsvg__colors.Length;

            for (i = 0; i < ncolors; i++)
            {
                ref var color = ref nsvg__colors[i];
                if (strcmp(color.name, str) == 0)
                {
                    return color.color;
                }
            }

            return NSVG_RGB(128, 128, 128);
        }

        private static uint nsvg__parseColor(byte* str)
        {
            int len = 0;
            while (*str == ' ') ++str;
            len = strlen(str);
            if (len >= 1 && *str == '#')
                return nsvg__parseColorHex(str);
            else if (len >= 4 && str[0] == 'r' && str[1] == 'g' && str[2] == 'b' && str[3] == '(')
                return nsvg__parseColorRGB(str);
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/163
            else if (len >= 5 && str[0] == 'r' && str[1] == 'g' && str[2] == 'b' && str[3] == 'a' && str[4] == '(')
                return nsvg__parseColorRGBA(str);
#endif
            return nsvg__parseColorName(str);
        }

        private static float nsvg__parseOpacity(byte* str)
        {
            float val = nsvg__atof(str);
            if (val < 0.0f) val = 0.0f;
            if (val > 1.0f) val = 1.0f;
            return val;
        }

        private static float nsvg__parseMiterLimit(byte* str)
        {
            float val = nsvg__atof(str);
            if (val < 0.0f) val = 0.0f;
            return val;
        }

        private static int nsvg__parseUnits(byte* units)
        {
            if (units[0] == 'p' && units[1] == 'x')
                return (int)NSVGunits.NSVG_UNITS_PX;
            else if (units[0] == 'p' && units[1] == 't')
                return (int)NSVGunits.NSVG_UNITS_PT;
            else if (units[0] == 'p' && units[1] == 'c')
                return (int)NSVGunits.NSVG_UNITS_PC;
            else if (units[0] == 'm' && units[1] == 'm')
                return (int)NSVGunits.NSVG_UNITS_MM;
            else if (units[0] == 'c' && units[1] == 'm')
                return (int)NSVGunits.NSVG_UNITS_CM;
            else if (units[0] == 'i' && units[1] == 'n')
                return (int)NSVGunits.NSVG_UNITS_IN;
            else if (units[0] == '%')
                return (int)NSVGunits.NSVG_UNITS_PERCENT;
            else if (units[0] == 'e' && units[1] == 'm')
                return (int)NSVGunits.NSVG_UNITS_EM;
            else if (units[0] == 'e' && units[1] == 'x')
                return (int)NSVGunits.NSVG_UNITS_EX;
            return (int)NSVGunits.NSVG_UNITS_USER;
        }

        private static bool nsvg__isCoordinate(byte* s)
        {
            // optional sign
            if (*s == '-' || *s == '+')
                s++;
            // must have at least one digit, or start by a dot
            return (nsvg__isdigit(*s) || *s == '.');
        }

        private static NSVGcoordinate nsvg__parseCoordinateRaw(byte* str)
        {
            NSVGcoordinate coord = new() { value = 0, units = (int)NSVGunits.NSVG_UNITS_USER };
            byte* buf = stackalloc byte[64];
            coord.units = nsvg__parseUnits(nsvg__parseNumber(str, buf, 64));
            coord.value = nsvg__atof(buf);
            return coord;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NSVGcoordinate nsvg__coord(float v, int units)
        {
            return new() { value = v, units = units };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__parseCoordinate(NSVGparser* p, byte* str, float orig, float length)
        {
            return nsvg__convertToPixels(p, nsvg__parseCoordinateRaw(str), orig, length);
        }

        private static int nsvg__parseTransformArgs(byte* str, float* args, int maxNa, int* na)
        {
            byte* end;
            byte* ptr;
            byte* it = stackalloc byte[64];

            *na = 0;
            ptr = str;
            while (*ptr is not 0 && *ptr != '(') ++ptr;
            if (*ptr == 0)
                return 1;
            end = ptr;
            while (*end is not 0 && *end != ')') ++end;
            if (*end == 0)
                return 1;

            while (ptr < end)
            {
                if (*ptr == '-' || *ptr == '+' || *ptr == '.' || nsvg__isdigit(*ptr))
                {
                    if (*na >= maxNa) return 0;
                    ptr = nsvg__parseNumber(ptr, it, 64);
                    args[(*na)++] = (float)nsvg__atof(it);
                }
                else
                {
                    ++ptr;
                }
            }

            return (int)(end - str);
        }

        private static int nsvg__parseMatrix(float* xform, byte* str)
        {
            float* t = stackalloc float[6];
            int na = 0;
            int len = nsvg__parseTransformArgs(str, t, 6, &na);
            if (na != 6) return len;
            memcpy(xform, t, sizeof(float) * 6);
            return len;
        }

        private static int nsvg__parseTranslate(float* xform, byte* str)
        {
            float* args = stackalloc float[2];
            float* t = stackalloc float[6];
            int na = 0;
            int len = nsvg__parseTransformArgs(str, args, 2, &na);
            if (na == 1) args[1] = 0.0f;

            nsvg__xformSetTranslation(t, args[0], args[1]);
            memcpy(xform, t, sizeof(float) * 6);
            return len;
        }

        private static int nsvg__parseScale(float* xform, byte* str)
        {
            float* args = stackalloc float[2];
            int na = 0;
            float* t = stackalloc float[6];
            int len = nsvg__parseTransformArgs(str, args, 2, &na);
            if (na == 1) args[1] = args[0];
            nsvg__xformSetScale(t, args[0], args[1]);
            memcpy(xform, t, sizeof(float) * 6);
            return len;
        }

        private static int nsvg__parseSkewX(float* xform, byte* str)
        {
            float* args = stackalloc float[1];
            int na = 0;
            float* t = stackalloc float[6];
            int len = nsvg__parseTransformArgs(str, args, 1, &na);
            nsvg__xformSetSkewX(t, args[0] / 180.0f * NSVG_PI);
            memcpy(xform, t, sizeof(float) * 6);
            return len;
        }

        private static int nsvg__parseSkewY(float* xform, byte* str)
        {
            float* args = stackalloc float[1];
            int na = 0;
            float* t = stackalloc float[6];
            int len = nsvg__parseTransformArgs(str, args, 1, &na);
            nsvg__xformSetSkewY(t, args[0] / 180.0f * NSVG_PI);
            memcpy(xform, t, sizeof(float) * 6);
            return len;
        }

        private static int nsvg__parseRotate(float* xform, byte* str)
        {
            float* args = stackalloc float[3];
            int na = 0;
            float* m = stackalloc float[6];
            float* t = stackalloc float[6];
            int len = nsvg__parseTransformArgs(str, args, 3, &na);
            if (na == 1)
                args[1] = args[2] = 0.0f;
            nsvg__xformIdentity(m);

            if (na > 1)
            {
                nsvg__xformSetTranslation(t, -args[1], -args[2]);
                nsvg__xformMultiply(m, t);
            }

            nsvg__xformSetRotation(t, args[0] / 180.0f * NSVG_PI);
            nsvg__xformMultiply(m, t);

            if (na > 1)
            {
                nsvg__xformSetTranslation(t, args[1], args[2]);
                nsvg__xformMultiply(m, t);
            }

            memcpy(xform, m, sizeof(float) * 6);

            return len;
        }

        private static void nsvg__parseTransform(float* xform, byte* str)
        {
            float* t = stackalloc float[6];
            int len;
            nsvg__xformIdentity(xform);
            while (*str is not 0)
            {
                if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("matrix"u8)), 6) == 0)
                    len = nsvg__parseMatrix(t, str);
                else if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("translate"u8)), 9) == 0)
                    len = nsvg__parseTranslate(t, str);
                else if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("scale"u8)), 5) == 0)
                    len = nsvg__parseScale(t, str);
                else if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rotate"u8)), 6) == 0)
                    len = nsvg__parseRotate(t, str);
                else if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("skewX"u8)), 5) == 0)
                    len = nsvg__parseSkewX(t, str);
                else if (strncmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("skewY"u8)), 5) == 0)
                    len = nsvg__parseSkewY(t, str);
                else
                {
                    ++str;
                    continue;
                }
                if (len != 0)
                {
                    str += len;
                }
                else
                {
                    ++str;
                    continue;
                }

                nsvg__xformPremultiply(xform, t);
            }
        }

        private static void nsvg__parseUrl(byte* id, byte* str)
        {
            int i = 0;
            str += 4; // "url(";
            if (*str is not 0 && *str == '#')
                str++;
            while (i < 63 && *str is not 0 && *str != ')')
            {
                id[i] = *str++;
                i++;
            }
            id[i] = (byte)'\0';
        }

        private static NSVGlineCap nsvg__parseLineCap(byte* str)
        {
            if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("butt"u8))) == 0)
                return NSVGlineCap.NSVG_CAP_BUTT;
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("round"u8))) == 0)
                return NSVGlineCap.NSVG_CAP_ROUND;
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("square"u8))) == 0)
                return NSVGlineCap.NSVG_CAP_SQUARE;
            // TODO: handle inherit.
            return NSVGlineCap.NSVG_CAP_BUTT;
        }

        private static NSVGlineJoin nsvg__parseLineJoin(byte* str)
        {
            if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("miter"u8))) == 0)
                return NSVGlineJoin.NSVG_JOIN_MITER;
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("round"u8))) == 0)
                return NSVGlineJoin.NSVG_JOIN_ROUND;
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("bevel"u8))) == 0)
                return NSVGlineJoin.NSVG_JOIN_BEVEL;
            // TODO: handle inherit.
            return NSVGlineJoin.NSVG_JOIN_MITER;
        }

        private static NSVGfillRule nsvg__parseFillRule(byte* str)
        {
            if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("nonzero"u8))) == 0)
                return NSVGfillRule.NSVG_FILLRULE_NONZERO;
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("evenodd"u8))) == 0)
                return NSVGfillRule.NSVG_FILLRULE_EVENODD;
            // TODO: handle inherit.
            return NSVGfillRule.NSVG_FILLRULE_NONZERO;
        }

        private static byte nsvg__parsePaintOrder(byte* str)
        {
            if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("normal"u8))) == 0 || strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fill stroke markers"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_MARKERS);
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fill markers stroke"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_MARKERS, NSVGpaintOrder.NSVG_PAINT_STROKE);
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("markers fill stroke"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_MARKERS, NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_STROKE);
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("markers stroke fill"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_MARKERS, NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_FILL);
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke fill markers"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_MARKERS);
            else if (strcmp(str, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke markers fill"u8))) == 0)
                return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_MARKERS, NSVGpaintOrder.NSVG_PAINT_FILL);
            // TODO: handle inherit.
            return nsvg__encodePaintOrder(NSVGpaintOrder.NSVG_PAINT_FILL, NSVGpaintOrder.NSVG_PAINT_STROKE, NSVGpaintOrder.NSVG_PAINT_MARKERS);
        }

        private static byte* nsvg__getNextDashItem(byte* s, byte* it)
        {
            int n = 0;
            it[0] = (byte)'\0';
            // Skip white spaces and commas
            while (*s is not 0 && (nsvg__isspace(*s) || *s == ',')) s++;
            // Advance until whitespace, comma or end.
            while (*s is not 0 && (!nsvg__isspace(*s) && *s != ','))
            {
                if (n < 63)
                    it[n++] = *s;
                s++;
            }
            it[n++] = (byte)'\0';
            return s;
        }

        private static int nsvg__parseStrokeDashArray(NSVGparser* p, byte* str, float* strokeDashArray)
        {
            byte* item = stackalloc byte[64];
            int count = 0, i;
            float sum = 0.0f;

            // Handle "none"
            if (str[0] == 'n')
                return 0;

            // Parse dashes
            while (*str is not 0)
            {
                str = nsvg__getNextDashItem(str, item);
                if (*item is 0) break;
                if (count < NSVG_MAX_DASHES)
                    strokeDashArray[count++] = MathF.Abs(nsvg__parseCoordinate(p, item, 0.0f, nsvg__actualLength(p)));
            }

            for (i = 0; i < count; i++)
                sum += strokeDashArray[i];
            if (sum <= 1e-6f)
                count = 0;

            return count;
        }

        private static bool nsvg__parseAttr(NSVGparser* p, byte* name, byte* value)
        {
            float* xform = stackalloc float[6];
            NSVGattrib* attr = nsvg__getAttr(p);
            if (attr is null) return false;

            if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("style"u8))) == 0)
            {
                nsvg__parseStyle(p, value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("display"u8))) == 0)
            {
                if (strcmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("none"u8))) == 0)
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/118
                    attr->visible &= ~NSVGvisibility.NSVG_VIS_DISPLAY;
#else
                    attr->visible = false;
#endif
                // Don't reset ->visible on display:inline, one display:none hides the whole subtree

            }
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/118
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("visibility"u8))) == 0)
            {
                if (strcmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("hidden"u8))) == 0)
                {
                    attr->visible &= ~NSVGvisibility.NSVG_VIS_VISIBLE;
                }
                else if (strcmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("visible"u8))) == 0)
                {
                    attr->visible |= NSVGvisibility.NSVG_VIS_VISIBLE;
                }
            }
#endif
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fill"u8))) == 0)
            {
                if (strcmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("none"u8))) == 0)
                {
                    attr->hasFill = 0;
                }
                else if (strncmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("url("u8)), 4) == 0)
                {
                    attr->hasFill = 2;
                    nsvg__parseUrl(attr->fillGradient, value);
                }
                else
                {
                    attr->hasFill = 1;
                    attr->fillColor = nsvg__parseColor(value);

#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/163
                    // if the fillColor has an alpha value then use it to
                    // set the fillOpacity
                    if ((attr->fillColor & 0xFF000000) is not 0)
                    {
                        attr->fillOpacity = ((attr->fillColor >> 24) & 0xFF) / 255.0f;
                        // remove the alpha value from the color
                        attr->fillColor &= 0x00FFFFFF;
                    }
#endif
                }
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("opacity"u8))) == 0)
            {
                attr->opacity = nsvg__parseOpacity(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fill-opacity"u8))) == 0)
            {
#if NANOSVG_PORT_DISABLE_PATCHES
                attr->fillOpacity = nsvg__parseOpacity(value);
#else // https://github.com/memononen/nanosvg/pull/163
                attr->fillOpacity *= nsvg__parseOpacity(value);
#endif
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke"u8))) == 0)
            {
                if (strcmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("none"u8))) == 0)
                {
                    attr->hasStroke = 0;
                }
                else if (strncmp(value, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("url("u8)), 4) == 0)
                {
                    attr->hasStroke = 2;
                    nsvg__parseUrl(attr->strokeGradient, value);
                }
                else
                {
                    attr->hasStroke = 1;
                    attr->strokeColor = nsvg__parseColor(value);
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/163
                    // if the strokeColor has an alpha value then use it to
                    // set the strokeOpacity
                    if ((attr->strokeColor & 0xFF000000) is not 0)
                    {
                        attr->strokeOpacity = ((attr->strokeColor >> 24) & 0xFF) / 255.0f;
                        // remove the alpha value from the color
                        attr->strokeColor &= 0x00FFFFFF;
                    }
#endif
                }
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-width"u8))) == 0)
            {
                attr->strokeWidth = nsvg__parseCoordinate(p, value, 0.0f, nsvg__actualLength(p));
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-dasharray"u8))) == 0)
            {
                attr->strokeDashCount = nsvg__parseStrokeDashArray(p, value, attr->strokeDashArray);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-dashoffset"u8))) == 0)
            {
                attr->strokeDashOffset = nsvg__parseCoordinate(p, value, 0.0f, nsvg__actualLength(p));
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-opacity"u8))) == 0)
            {
#if NANOSVG_PORT_DISABLE_PATCHES
                attr->strokeOpacity = nsvg__parseOpacity(value);
#else // https://github.com/memononen/nanosvg/pull/163
                attr->strokeOpacity *= nsvg__parseOpacity(value);
#endif
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-linecap"u8))) == 0)
            {
                attr->strokeLineCap = nsvg__parseLineCap(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-linejoin"u8))) == 0)
            {
                attr->strokeLineJoin = nsvg__parseLineJoin(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stroke-miterlimit"u8))) == 0)
            {
                attr->miterLimit = nsvg__parseMiterLimit(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fill-rule"u8))) == 0)
            {
                attr->fillRule = nsvg__parseFillRule(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("font-size"u8))) == 0)
            {
                attr->fontSize = nsvg__parseCoordinate(p, value, 0.0f, nsvg__actualLength(p));
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("transform"u8))) == 0)
            {
                nsvg__parseTransform(xform, value);
                nsvg__xformPremultiply(attr->xform, xform);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stop-color"u8))) == 0)
            {
                attr->stopColor = nsvg__parseColor(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stop-opacity"u8))) == 0)
            {
                attr->stopOpacity = nsvg__parseOpacity(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("offset"u8))) == 0)
            {
                attr->stopOffset = nsvg__parseCoordinate(p, value, 0.0f, 1.0f);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("paint-order"u8))) == 0)
            {
                attr->paintOrder = nsvg__parsePaintOrder(value);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("id"u8))) == 0)
            {
                strncpy(attr->id, value, 63);
                attr->id[63] = (byte)'\0';
            }
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/131
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x"u8))) == 0)
            {
                nsvg__xformSetTranslation(xform, (float)nsvg__atof(value), 0);
                nsvg__xformPremultiply(attr->xform, xform);
            }
            else if (strcmp(name, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y"u8))) == 0)
            {
                nsvg__xformSetTranslation(xform, 0, (float)nsvg__atof(value));
                nsvg__xformPremultiply(attr->xform, xform);
            }
#endif
            else
            {
                return false;
            }
            return true;
        }

        private static bool nsvg__parseNameValue(NSVGparser* p, byte* start, byte* end)
        {
            byte* str;
            byte* val;
            byte* name = stackalloc byte[512];
            byte* value = stackalloc byte[512];
            int n;

            str = start;
            while (str < end && *str != ':') ++str;

            val = str;

            // Right Trim
            while (str > start && (*str == ':' || nsvg__isspace(*str))) --str;
            ++str;

            n = (int)(str - start);
            if (n > 511) n = 511;
            if (n > 0) memcpy(name, start, n);
            name[n] = 0;

            while (val < end && (*val == ':' || nsvg__isspace(*val))) ++val;

            n = (int)(end - val);
            if (n > 511) n = 511;
            if (n > 0) memcpy(value, val, n);
            value[n] = 0;

            return nsvg__parseAttr(p, name, value);
        }

        private static void nsvg__parseStyle(NSVGparser* p, byte* str)
        {
            byte* start;
            byte* end;

            while (*str is not 0)
            {
                // Left Trim
                while (*str is not 0 && nsvg__isspace(*str)) ++str;
                start = str;
                while (*str is not 0 && *str != ';') ++str;
                end = str;

                // Right Trim
                while (end > start && (*end == ';' || nsvg__isspace(*end))) --end;
                ++end;

                nsvg__parseNameValue(p, start, end);
                if (*str is not 0) ++str;
            }
        }

        private static void nsvg__parseAttribs(NSVGparser* p, byte** attr)
        {
            int i;
            for (i = 0; attr[i] is not null; i += 2)
            {
                if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("style"u8))) == 0)
                    nsvg__parseStyle(p, attr[i + 1]);
                else
                    nsvg__parseAttr(p, attr[i], attr[i + 1]);
            }
        }

        private static int nsvg__getArgsPerElement(byte cmd)
        {
            switch (cmd)
            {
                case (byte)'v':
                case (byte)'V':
                case (byte)'h':
                case (byte)'H':
                    return 1;
                case (byte)'m':
                case (byte)'M':
                case (byte)'l':
                case (byte)'L':
                case (byte)'t':
                case (byte)'T':
                    return 2;
                case (byte)'q':
                case (byte)'Q':
                case (byte)'s':
                case (byte)'S':
                    return 4;
                case (byte)'c':
                case (byte)'C':
                    return 6;
                case (byte)'a':
                case (byte)'A':
                    return 7;
                case (byte)'z':
                case (byte)'Z':
                    return 0;
            }
            return -1;
        }

        private static void nsvg__pathMoveTo(NSVGparser* p, float* cpx, float* cpy, float* args, bool rel)
        {
            if (rel)
            {
                *cpx += args[0];
                *cpy += args[1];
            }
            else
            {
                *cpx = args[0];
                *cpy = args[1];
            }
            nsvg__moveTo(p, *cpx, *cpy);
        }

        private static void nsvg__pathLineTo(NSVGparser* p, float* cpx, float* cpy, float* args, bool rel)
        {
            if (rel)
            {
                *cpx += args[0];
                *cpy += args[1];
            }
            else
            {
                *cpx = args[0];
                *cpy = args[1];
            }
            nsvg__lineTo(p, *cpx, *cpy);
        }

        private static void nsvg__pathHLineTo(NSVGparser* p, float* cpx, float* cpy, float* args, bool rel)
        {
            if (rel)
                *cpx += args[0];
            else
                *cpx = args[0];
            nsvg__lineTo(p, *cpx, *cpy);
        }

        private static void nsvg__pathVLineTo(NSVGparser* p, float* cpx, float* cpy, float* args, bool rel)
        {
            if (rel)
                *cpy += args[0];
            else
                *cpy = args[0];
            nsvg__lineTo(p, *cpx, *cpy);
        }

        private static void nsvg__pathCubicBezTo(NSVGparser* p, float* cpx, float* cpy,
                                 float* cpx2, float* cpy2, float* args, bool rel)
        {
            float x2, y2, cx1, cy1, cx2, cy2;

            if (rel)
            {
                cx1 = *cpx + args[0];
                cy1 = *cpy + args[1];
                cx2 = *cpx + args[2];
                cy2 = *cpy + args[3];
                x2 = *cpx + args[4];
                y2 = *cpy + args[5];
            }
            else
            {
                cx1 = args[0];
                cy1 = args[1];
                cx2 = args[2];
                cy2 = args[3];
                x2 = args[4];
                y2 = args[5];
            }

            nsvg__cubicBezTo(p, cx1, cy1, cx2, cy2, x2, y2);

            *cpx2 = cx2;
            *cpy2 = cy2;
            *cpx = x2;
            *cpy = y2;
        }

        private static void nsvg__pathCubicBezShortTo(NSVGparser* p, float* cpx, float* cpy,
                                      float* cpx2, float* cpy2, float* args, bool rel)
        {
            float x1, y1, x2, y2, cx1, cy1, cx2, cy2;

            x1 = *cpx;
            y1 = *cpy;
            if (rel)
            {
                cx2 = *cpx + args[0];
                cy2 = *cpy + args[1];
                x2 = *cpx + args[2];
                y2 = *cpy + args[3];
            }
            else
            {
                cx2 = args[0];
                cy2 = args[1];
                x2 = args[2];
                y2 = args[3];
            }

            cx1 = 2 * x1 - *cpx2;
            cy1 = 2 * y1 - *cpy2;

            nsvg__cubicBezTo(p, cx1, cy1, cx2, cy2, x2, y2);

            *cpx2 = cx2;
            *cpy2 = cy2;
            *cpx = x2;
            *cpy = y2;
        }

        private static void nsvg__pathQuadBezTo(NSVGparser* p, float* cpx, float* cpy,
                                float* cpx2, float* cpy2, float* args, bool rel)
        {
            float x1, y1, x2, y2, cx, cy;
            float cx1, cy1, cx2, cy2;

            x1 = *cpx;
            y1 = *cpy;
            if (rel)
            {
                cx = *cpx + args[0];
                cy = *cpy + args[1];
                x2 = *cpx + args[2];
                y2 = *cpy + args[3];
            }
            else
            {
                cx = args[0];
                cy = args[1];
                x2 = args[2];
                y2 = args[3];
            }

            // Convert to cubic bezier
            cx1 = x1 + 2.0f / 3.0f * (cx - x1);
            cy1 = y1 + 2.0f / 3.0f * (cy - y1);
            cx2 = x2 + 2.0f / 3.0f * (cx - x2);
            cy2 = y2 + 2.0f / 3.0f * (cy - y2);

            nsvg__cubicBezTo(p, cx1, cy1, cx2, cy2, x2, y2);

            *cpx2 = cx;
            *cpy2 = cy;
            *cpx = x2;
            *cpy = y2;
        }

        private static void nsvg__pathQuadBezShortTo(NSVGparser* p, float* cpx, float* cpy,
                                     float* cpx2, float* cpy2, float* args, bool rel)
        {
            float x1, y1, x2, y2, cx, cy;
            float cx1, cy1, cx2, cy2;

            x1 = *cpx;
            y1 = *cpy;
            if (rel)
            {
                x2 = *cpx + args[0];
                y2 = *cpy + args[1];
            }
            else
            {
                x2 = args[0];
                y2 = args[1];
            }

            cx = 2 * x1 - *cpx2;
            cy = 2 * y1 - *cpy2;

            // Convert to cubix bezier
            cx1 = x1 + 2.0f / 3.0f * (cx - x1);
            cy1 = y1 + 2.0f / 3.0f * (cy - y1);
            cx2 = x2 + 2.0f / 3.0f * (cx - x2);
            cy2 = y2 + 2.0f / 3.0f * (cy - y2);

            nsvg__cubicBezTo(p, cx1, cy1, cx2, cy2, x2, y2);

            *cpx2 = cx;
            *cpy2 = cy;
            *cpx = x2;
            *cpy = y2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__vmag(float x, float y) { return MathF.Sqrt(x * x + y * y); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float nsvg__sqr(float x) { return x * x; }

        private static float nsvg__vecrat(float ux, float uy, float vx, float vy)
        {
            return (ux * vx + uy * vy) / (nsvg__vmag(ux, uy) * nsvg__vmag(vx, vy));
        }

        private static float nsvg__vecang(float ux, float uy, float vx, float vy)
        {
            float r = nsvg__vecrat(ux, uy, vx, vy);
            if (r < -1.0f) r = -1.0f;
            if (r > 1.0f) r = 1.0f;
            return ((ux * vy < uy * vx) ? -1.0f : 1.0f) * MathF.Acos(r);
        }

        private static void nsvg__pathArcTo(NSVGparser* p, float* cpx, float* cpy, float* args, bool rel)
        {
            // Ported from canvg (https://code.google.com/p/canvg/)
            float rx, ry, rotx;
            float x1, y1, x2, y2, cx, cy, dx, dy, d;
            float x1p, y1p, cxp, cyp, s, sa, sb;
            float ux, uy, vx, vy, a1, da;
            float x, y, tanx, tany, a, px = 0, py = 0, ptanx = 0, ptany = 0;
            float* t = stackalloc float[6];
            float sinrx, cosrx;
            int fa, fs;
            int i, ndivs;
            float hda, kappa;

            rx = MathF.Abs(args[0]);                // y radius
            ry = MathF.Abs(args[1]);                // x radius
            rotx = args[2] / 180.0f * NSVG_PI;      // x rotation angle
            fa = MathF.Abs(args[3]) > 1e-6 ? 1 : 0; // Large arc
            fs = MathF.Abs(args[4]) > 1e-6 ? 1 : 0; // Sweep direction
            x1 = *cpx;                          // start point
            y1 = *cpy;
            if (rel)
            {                           // end point
                x2 = *cpx + args[5];
                y2 = *cpy + args[6];
            }
            else
            {
                x2 = args[5];
                y2 = args[6];
            }

            dx = x1 - x2;
            dy = y1 - y2;
            d = MathF.Sqrt(dx * dx + dy * dy);
            if (d < 1e-6f || rx < 1e-6f || ry < 1e-6f)
            {
                // The arc degenerates to a line
                nsvg__lineTo(p, x2, y2);
                *cpx = x2;
                *cpy = y2;
                return;
            }

            sinrx = MathF.Sin(rotx);
            cosrx = MathF.Cos(rotx);

            // Convert to center point parameterization.
            // http://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
            // 1) Compute x1', y1'
            x1p = cosrx * dx / 2.0f + sinrx * dy / 2.0f;
            y1p = -sinrx * dx / 2.0f + cosrx * dy / 2.0f;
            d = nsvg__sqr(x1p) / nsvg__sqr(rx) + nsvg__sqr(y1p) / nsvg__sqr(ry);
            if (d > 1)
            {
                d = MathF.Sqrt(d);
                rx *= d;
                ry *= d;
            }
            // 2) Compute cx', cy'
            s = 0.0f;
            sa = nsvg__sqr(rx) * nsvg__sqr(ry) - nsvg__sqr(rx) * nsvg__sqr(y1p) - nsvg__sqr(ry) * nsvg__sqr(x1p);
            sb = nsvg__sqr(rx) * nsvg__sqr(y1p) + nsvg__sqr(ry) * nsvg__sqr(x1p);
            if (sa < 0.0f) sa = 0.0f;
            if (sb > 0.0f)
                s = MathF.Sqrt(sa / sb);
            if (fa == fs)
                s = -s;
            cxp = s * rx * y1p / ry;
            cyp = s * -ry * x1p / rx;

            // 3) Compute cx,cy from cx',cy'
            cx = (x1 + x2) / 2.0f + cosrx * cxp - sinrx * cyp;
            cy = (y1 + y2) / 2.0f + sinrx * cxp + cosrx * cyp;

            // 4) Calculate theta1, and delta theta.
            ux = (x1p - cxp) / rx;
            uy = (y1p - cyp) / ry;
            vx = (-x1p - cxp) / rx;
            vy = (-y1p - cyp) / ry;
            a1 = nsvg__vecang(1.0f, 0.0f, ux, uy);  // Initial angle
            da = nsvg__vecang(ux, uy, vx, vy);      // Delta angle

            //	if (vecrat(ux,uy,vx,vy) <= -1.0f) da = NSVG_PI;
            //	if (vecrat(ux,uy,vx,vy) >= 1.0f) da = 0;

            if (fs == 0 && da > 0)
                da -= 2 * NSVG_PI;
            else if (fs == 1 && da < 0)
                da += 2 * NSVG_PI;

            // Approximate the arc using cubic spline segments.
            t[0] = cosrx; t[1] = sinrx;
            t[2] = -sinrx; t[3] = cosrx;
            t[4] = cx; t[5] = cy;

            // Split arc into max 90 degree segments.
            // The loop assumes an iteration per end point (including start and end), this +1.
            ndivs = (int)(MathF.Abs(da) / (NSVG_PI * 0.5f) + 1.0f);
            hda = (da / (float)ndivs) / 2.0f;
            // Fix for ticket #179: division by 0: avoid cotangens around 0 (infinite)
            if ((hda < 1e-3f) && (hda > -1e-3f))
                hda *= 0.5f;
            else
                hda = (1.0f - MathF.Cos(hda)) / MathF.Sin(hda);
            kappa = MathF.Abs(4.0f / 3.0f * hda);
            if (da < 0.0f)
                kappa = -kappa;

            for (i = 0; i <= ndivs; i++)
            {
                a = a1 + da * ((float)i / (float)ndivs);
                dx = MathF.Cos(a);
                dy = MathF.Sin(a);
                nsvg__xformPoint(&x, &y, dx * rx, dy * ry, t); // position
                nsvg__xformVec(&tanx, &tany, -dy * rx * kappa, dx * ry * kappa, t); // tangent
                if (i > 0)
                    nsvg__cubicBezTo(p, px + ptanx, py + ptany, x - tanx, y - tany, x, y);
                px = x;
                py = y;
                ptanx = tanx;
                ptany = tany;
            }

            *cpx = x2;
            *cpy = y2;
        }

        private static void nsvg__parsePath(NSVGparser* p, byte** attr)
        {
            byte* s = null;
            byte cmd = (byte)'\0';
            float* args = stackalloc float[10];
            int nargs;
            int rargs = 0;
            byte initPoint;
            float cpx, cpy, cpx2, cpy2;
            byte** tmp = stackalloc byte*[4];
            bool closedFlag;
            int i;
            byte* item = stackalloc byte[64];

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("d"u8))) == 0)
                {
                    s = attr[i + 1];
                }
                else
                {
                    tmp[0] = attr[i];
                    tmp[1] = attr[i + 1];
                    tmp[2] = null;
                    tmp[3] = null;
                    nsvg__parseAttribs(p, tmp);
                }
            }

            if (s is not null)
            {
                nsvg__resetPath(p);
                cpx = 0; cpy = 0;
                cpx2 = 0; cpy2 = 0;
                initPoint = 0;
                closedFlag = false;
                nargs = 0;

                while (*s is not 0)
                {
                    item[0] = (byte)'\0';
                    if ((cmd == 'A' || cmd == 'a') && (nargs == 3 || nargs == 4))
                        s = nsvg__getNextPathItemWhenArcFlag(s, item);
                    if (*item is 0)
                        s = nsvg__getNextPathItem(s, item);
                    if (*item is 0) break;
                    if (cmd != '\0' && nsvg__isCoordinate(item))
                    {
                        if (nargs < 10)
                            args[nargs++] = (float)nsvg__atof(item);
                        if (nargs >= rargs)
                        {
                            switch (cmd)
                            {
                                case (byte)'m':
                                case (byte)'M':
                                    nsvg__pathMoveTo(p, &cpx, &cpy, args, cmd == (byte)'m');
                                    // Moveto can be followed by multiple coordinate pairs,
                                    // which should be treated as linetos.
                                    cmd = (cmd == (byte)'m') ? (byte)'l' : (byte)'L';
                                    rargs = nsvg__getArgsPerElement(cmd);
                                    cpx2 = cpx; cpy2 = cpy;
                                    initPoint = 1;
                                    break;
                                case (byte)'l':
                                case (byte)'L':
                                    nsvg__pathLineTo(p, &cpx, &cpy, args, cmd == 'l');
                                    cpx2 = cpx; cpy2 = cpy;
                                    break;
                                case (byte)'H':
                                case (byte)'h':
                                    nsvg__pathHLineTo(p, &cpx, &cpy, args, cmd == 'h');
                                    cpx2 = cpx; cpy2 = cpy;
                                    break;
                                case (byte)'V':
                                case (byte)'v':
                                    nsvg__pathVLineTo(p, &cpx, &cpy, args, cmd == 'v');
                                    cpx2 = cpx; cpy2 = cpy;
                                    break;
                                case (byte)'C':
                                case (byte)'c':
                                    nsvg__pathCubicBezTo(p, &cpx, &cpy, &cpx2, &cpy2, args, cmd == 'c');
                                    break;
                                case (byte)'S':
                                case (byte)'s':
                                    nsvg__pathCubicBezShortTo(p, &cpx, &cpy, &cpx2, &cpy2, args, cmd == 's');
                                    break;
                                case (byte)'Q':
                                case (byte)'q':
                                    nsvg__pathQuadBezTo(p, &cpx, &cpy, &cpx2, &cpy2, args, cmd == 'q');
                                    break;
                                case (byte)'T':
                                case (byte)'t':
                                    nsvg__pathQuadBezShortTo(p, &cpx, &cpy, &cpx2, &cpy2, args, cmd == 't');
                                    break;
                                case (byte)'A':
                                case (byte)'a':
                                    nsvg__pathArcTo(p, &cpx, &cpy, args, cmd == 'a');
                                    cpx2 = cpx; cpy2 = cpy;
                                    break;
                                default:
                                    if (nargs >= 2)
                                    {
                                        cpx = args[nargs - 2];
                                        cpy = args[nargs - 1];
                                        cpx2 = cpx; cpy2 = cpy;
                                    }
                                    break;
                            }
                            nargs = 0;
                        }
                    }
                    else
                    {
                        cmd = item[0];
                        if (cmd == 'M' || cmd == 'm')
                        {
                            // Commit path.
                            if (p->npts > 0)
                                nsvg__addPath(p, closedFlag);
                            // Start new subpath.
                            nsvg__resetPath(p);
                            closedFlag = false;
                            nargs = 0;
                        }
                        else if (initPoint == 0)
                        {
                            // Do not allow other commands until initial point has been set (moveTo called once).
                            cmd = (byte)'\0';
                        }
                        if (cmd == 'Z' || cmd == 'z')
                        {
                            closedFlag = true;
                            // Commit path.
                            if (p->npts > 0)
                            {
                                // Move current point to first point
                                cpx = p->pts[0];
                                cpy = p->pts[1];
                                cpx2 = cpx; cpy2 = cpy;
                                nsvg__addPath(p, closedFlag);
                            }
                            // Start new subpath.
                            nsvg__resetPath(p);
                            nsvg__moveTo(p, cpx, cpy);
                            closedFlag = false;
                            nargs = 0;
                        }
                        rargs = nsvg__getArgsPerElement(cmd);
                        if (rargs == -1)
                        {
                            // Command not recognized
                            cmd = (byte)'\0';
                            rargs = 0;
                        }
                    }
                }
                // Commit path.
                if (p->npts > 0)
                    nsvg__addPath(p, closedFlag);
            }

            nsvg__addShape(p);
        }

        private static void nsvg__parseRect(NSVGparser* p, byte** attr)
        {
            float x = 0.0f;
            float y = 0.0f;
            float w = 0.0f;
            float h = 0.0f;
            float rx = -1.0f; // marks not set
            float ry = -1.0f;
            int i;

            for (i = 0; attr[i] is not null; i += 2)
            {
#if NANOSVG_PORT_DISABLE_PATCHES
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x"u8))) == 0) x = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y"u8))) == 0) y = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("width"u8))) == 0) w = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("height"u8))) == 0) h = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualHeight(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rx"u8))) == 0) rx = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualWidth(p)));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ry"u8))) == 0) ry = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualHeight(p)));
                }
#else // https://github.com/memononen/nanosvg/pull/131
                if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x"u8))) == 0) x = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y"u8))) == 0) y = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("width"u8))) == 0) w = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualWidth(p));
                else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("height"u8))) == 0) h = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualHeight(p));
                else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rx"u8))) == 0) rx = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualWidth(p)));
                else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ry"u8))) == 0) ry = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualHeight(p)));
                else nsvg__parseAttr(p, attr[i], attr[i + 1]);
#endif
            }

            if (rx < 0.0f && ry > 0.0f) rx = ry;
            if (ry < 0.0f && rx > 0.0f) ry = rx;
            if (rx < 0.0f) rx = 0.0f;
            if (ry < 0.0f) ry = 0.0f;
            if (rx > w / 2.0f) rx = w / 2.0f;
            if (ry > h / 2.0f) ry = h / 2.0f;

            if (w != 0.0f && h != 0.0f)
            {
                nsvg__resetPath(p);

                if (rx < 0.00001f || ry < 0.0001f)
                {
                    nsvg__moveTo(p, x, y);
                    nsvg__lineTo(p, x + w, y);
                    nsvg__lineTo(p, x + w, y + h);
                    nsvg__lineTo(p, x, y + h);
                }
                else
                {
                    // Rounded rectangle
                    nsvg__moveTo(p, x + rx, y);
                    nsvg__lineTo(p, x + w - rx, y);
                    nsvg__cubicBezTo(p, x + w - rx * (1 - NSVG_KAPPA90), y, x + w, y + ry * (1 - NSVG_KAPPA90), x + w, y + ry);
                    nsvg__lineTo(p, x + w, y + h - ry);
                    nsvg__cubicBezTo(p, x + w, y + h - ry * (1 - NSVG_KAPPA90), x + w - rx * (1 - NSVG_KAPPA90), y + h, x + w - rx, y + h);
                    nsvg__lineTo(p, x + rx, y + h);
                    nsvg__cubicBezTo(p, x + rx * (1 - NSVG_KAPPA90), y + h, x, y + h - ry * (1 - NSVG_KAPPA90), x, y + h - ry);
                    nsvg__lineTo(p, x, y + ry);
                    nsvg__cubicBezTo(p, x, y + ry * (1 - NSVG_KAPPA90), x + rx * (1 - NSVG_KAPPA90), y, x + rx, y);
                }

                nsvg__addPath(p, true);

                nsvg__addShape(p);
            }
        }
        private static void nsvg__parseCircle(NSVGparser* p, byte** attr)
        {
            float cx = 0.0f;
            float cy = 0.0f;
            float r = 0.0f;
            int i;

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cx"u8))) == 0) cx = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cy"u8))) == 0) cy = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("r"u8))) == 0) r = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualLength(p)));
                }
            }

            if (r > 0.0f)
            {
                nsvg__resetPath(p);

                nsvg__moveTo(p, cx + r, cy);
                nsvg__cubicBezTo(p, cx + r, cy + r * NSVG_KAPPA90, cx + r * NSVG_KAPPA90, cy + r, cx, cy + r);
                nsvg__cubicBezTo(p, cx - r * NSVG_KAPPA90, cy + r, cx - r, cy + r * NSVG_KAPPA90, cx - r, cy);
                nsvg__cubicBezTo(p, cx - r, cy - r * NSVG_KAPPA90, cx - r * NSVG_KAPPA90, cy - r, cx, cy - r);
                nsvg__cubicBezTo(p, cx + r * NSVG_KAPPA90, cy - r, cx + r, cy - r * NSVG_KAPPA90, cx + r, cy);

                nsvg__addPath(p, true);

                nsvg__addShape(p);
            }
        }

        private static void nsvg__parseEllipse(NSVGparser* p, byte** attr)
        {
            float cx = 0.0f;
            float cy = 0.0f;
            float rx = 0.0f;
            float ry = 0.0f;
            int i;

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cx"u8))) == 0) cx = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cy"u8))) == 0) cy = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rx"u8))) == 0) rx = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualWidth(p)));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ry"u8))) == 0) ry = MathF.Abs(nsvg__parseCoordinate(p, attr[i + 1], 0.0f, nsvg__actualHeight(p)));
                }
            }

            if (rx > 0.0f && ry > 0.0f)
            {

                nsvg__resetPath(p);

                nsvg__moveTo(p, cx + rx, cy);
                nsvg__cubicBezTo(p, cx + rx, cy + ry * NSVG_KAPPA90, cx + rx * NSVG_KAPPA90, cy + ry, cx, cy + ry);
                nsvg__cubicBezTo(p, cx - rx * NSVG_KAPPA90, cy + ry, cx - rx, cy + ry * NSVG_KAPPA90, cx - rx, cy);
                nsvg__cubicBezTo(p, cx - rx, cy - ry * NSVG_KAPPA90, cx - rx * NSVG_KAPPA90, cy - ry, cx, cy - ry);
                nsvg__cubicBezTo(p, cx + rx * NSVG_KAPPA90, cy - ry, cx + rx, cy - ry * NSVG_KAPPA90, cx + rx, cy);

                nsvg__addPath(p, true);

                nsvg__addShape(p);
            }
        }

        private static void nsvg__parseLine(NSVGparser* p, byte** attr)
        {
            float x1 = 0.0f;
            float y1 = 0.0f;
            float x2 = 0.0f;
            float y2 = 0.0f;
            int i;

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x1"u8))) == 0) x1 = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y1"u8))) == 0) y1 = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x2"u8))) == 0) x2 = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigX(p), nsvg__actualWidth(p));
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y2"u8))) == 0) y2 = nsvg__parseCoordinate(p, attr[i + 1], nsvg__actualOrigY(p), nsvg__actualHeight(p));
                }
            }

            nsvg__resetPath(p);

            nsvg__moveTo(p, x1, y1);
            nsvg__lineTo(p, x2, y2);

            nsvg__addPath(p, false);

            nsvg__addShape(p);
        }

        private static void nsvg__parsePoly(NSVGparser* p, byte** attr, bool closeFlag)
        {
            int i;
            byte* s;
            float* args = stackalloc float[2];
            int nargs, npts = 0;
            byte* item = stackalloc byte[64];

            nsvg__resetPath(p);

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("points"u8))) == 0)
                    {
                        s = attr[i + 1];
                        nargs = 0;
                        while (*s is not 0)
                        {
                            s = nsvg__getNextPathItem(s, item);
                            args[nargs++] = (float)nsvg__atof(item);
                            if (nargs >= 2)
                            {
                                if (npts == 0)
                                    nsvg__moveTo(p, args[0], args[1]);
                                else
                                    nsvg__lineTo(p, args[0], args[1]);
                                nargs = 0;
                                npts++;
                            }
                        }
                    }
                }
            }

            nsvg__addPath(p, closeFlag);

            nsvg__addShape(p);
        }

        private static void nsvg__parseSVG(NSVGparser* p, byte** attr)
        {
            int i;
            byte* buf = stackalloc byte[64];
            for (i = 0; attr[i] is not null; i += 2)
            {
                if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("width"u8))) == 0)
                    {
                        p->image->width = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, 0.0f);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("height"u8))) == 0)
                    {
                        p->image->height = nsvg__parseCoordinate(p, attr[i + 1], 0.0f, 0.0f);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("viewBox"u8))) == 0)
                    {
                        byte* s = attr[i + 1];

                        s = nsvg__parseNumber(s, buf, 64);
                        p->viewMinx = nsvg__atof(buf);
                        while (*s is not 0 && (nsvg__isspace(*s) || *s == '%' || *s == ',')) s++;
                        if (*s is 0) return;
                        s = nsvg__parseNumber(s, buf, 64);
                        p->viewMiny = nsvg__atof(buf);
                        while (*s is not 0 && (nsvg__isspace(*s) || *s == '%' || *s == ',')) s++;
                        if (*s is 0) return;
                        s = nsvg__parseNumber(s, buf, 64);
                        p->viewWidth = nsvg__atof(buf);
                        while (*s is not 0 && (nsvg__isspace(*s) || *s == '%' || *s == ',')) s++;
                        if (*s is 0) return;
                        s = nsvg__parseNumber(s, buf, 64);
                        p->viewHeight = nsvg__atof(buf);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("preserveAspectRatio"u8))) == 0)
                    {
                        if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("none"u8))) != 0)
                        {
                            // No uniform scaling
                            p->alignType = NSVG_ALIGN_NONE;
                        }
                        else
                        {
                            // Parse X align
                            if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("xMin"u8))) != 0)
                                p->alignX = NSVG_ALIGN_MIN;
                            else if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("xMid"u8))) != 0)
                                p->alignX = NSVG_ALIGN_MID;
                            else if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("xMax"u8))) != 0)
                                p->alignX = NSVG_ALIGN_MAX;
                            // Parse X align
                            if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("yMin"u8))) != 0)
                                p->alignY = NSVG_ALIGN_MIN;
                            else if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("yMid"u8))) != 0)
                                p->alignY = NSVG_ALIGN_MID;
                            else if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("yMax"u8))) != 0)
                                p->alignY = NSVG_ALIGN_MAX;
                            // Parse meet/slice
                            p->alignType = NSVG_ALIGN_MEET;
                            if (strstr(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("slice"u8))) != 0)
                                p->alignType = NSVG_ALIGN_SLICE;
                        }
                    }
                }
            }
        }

        private static void nsvg__parseGradient(NSVGparser* p, byte** attr, NSVGpaintType type)
        {
            int i;
            NSVGgradientData* grad = (NSVGgradientData*)malloc(sizeof(NSVGgradientData));
            if (grad == null) return;
            memset(grad, 0, sizeof(NSVGgradientData));
            grad->units = (byte)NSVGgradientUnits.NSVG_OBJECT_SPACE;
            grad->type = type;
            if (grad->type == NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT)
            {
                grad->linear.x1 = nsvg__coord(0.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
                grad->linear.y1 = nsvg__coord(0.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
                grad->linear.x2 = nsvg__coord(100.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
                grad->linear.y2 = nsvg__coord(0.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
            }
            else if (grad->type == NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT)
            {
                grad->radial.cx = nsvg__coord(50.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
                grad->radial.cy = nsvg__coord(50.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
                grad->radial.r = nsvg__coord(50.0f, (byte)NSVGunits.NSVG_UNITS_PERCENT);
            }

            nsvg__xformIdentity(grad->xform);

            for (i = 0; attr[i] is not null; i += 2)
            {
                if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("id"u8))) == 0)
                {
                    strncpy(grad->id, attr[i + 1], 63);
                    grad->id[63] = (byte)'\0';
                }
                else if (!nsvg__parseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("gradientUnits"u8))) == 0)
                    {
                        if (strcmp(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("objectBoundingBox"u8))) == 0)
                            grad->units = (byte)NSVGgradientUnits.NSVG_OBJECT_SPACE;
                        else
                            grad->units = (byte)NSVGgradientUnits.NSVG_USER_SPACE;
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("gradientTransform"u8))) == 0)
                    {
                        nsvg__parseTransform(grad->xform, attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cx"u8))) == 0)
                    {
                        grad->radial.cx = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("cy"u8))) == 0)
                    {
                        grad->radial.cy = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("r"u8))) == 0)
                    {
                        grad->radial.r = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fx"u8))) == 0)
                    {
                        grad->radial.fx = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("fy"u8))) == 0)
                    {
                        grad->radial.fy = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x1"u8))) == 0)
                    {
                        grad->linear.x1 = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y1"u8))) == 0)
                    {
                        grad->linear.y1 = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("x2"u8))) == 0)
                    {
                        grad->linear.x2 = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("y2"u8))) == 0)
                    {
                        grad->linear.y2 = nsvg__parseCoordinateRaw(attr[i + 1]);
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("spreadMethod"u8))) == 0)
                    {
                        if (strcmp(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("pad"u8))) == 0)
                            grad->spread = NSVGspreadType.NSVG_SPREAD_PAD;
                        else if (strcmp(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("reflect"u8))) == 0)
                            grad->spread = NSVGspreadType.NSVG_SPREAD_REFLECT;
                        else if (strcmp(attr[i + 1], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("repeat"u8))) == 0)
                            grad->spread = NSVGspreadType.NSVG_SPREAD_REPEAT;
                    }
                    else if (strcmp(attr[i], (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("xlink:href"u8))) == 0)
                    {
                        byte* href = attr[i + 1];
                        strncpy(grad->@ref, href + 1, 62);
                        grad->@ref[62] = (byte)'\0';
                    }
                }
            }

            grad->next = p->gradients;
            p->gradients = grad;
        }

        private static void nsvg__parseGradientStop(NSVGparser* p, byte** attr)
        {
            NSVGattrib* curAttr = nsvg__getAttr(p);
            NSVGgradientData* grad;
            NSVGgradientStop* stop;
            int i, idx;

            curAttr->stopOffset = 0;
            curAttr->stopColor = 0;
            curAttr->stopOpacity = 1.0f;

            for (i = 0; attr[i] is not null; i += 2)
            {
                nsvg__parseAttr(p, attr[i], attr[i + 1]);
            }

            // Add stop to the last gradient.
            grad = p->gradients;
            if (grad == null) return;

            grad->nstops++;
            grad->stops = (NSVGgradientStop*)realloc(grad->stops, sizeof(NSVGgradientStop) * grad->nstops);
            if (grad->stops == null) return;

            // Insert
            idx = grad->nstops - 1;
            for (i = 0; i < grad->nstops - 1; i++)
            {
                if (curAttr->stopOffset < grad->stops[i].offset)
                {
                    idx = i;
                    break;
                }
            }
            if (idx != grad->nstops - 1)
            {
                for (i = grad->nstops - 1; i > idx; i--)
                    grad->stops[i] = grad->stops[i - 1];
            }

            stop = &grad->stops[idx];
            stop->color = curAttr->stopColor;
            stop->color |= (uint)(curAttr->stopOpacity * 255) << 24;
            stop->offset = curAttr->stopOffset;
        }

        private static void nsvg__startElement(void* ud, byte* el, byte** attr)
        {
            NSVGparser* p = (NSVGparser*)ud;

            if (p->defsFlag)
            {
                // Skip everything but gradients in defs
                if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("linearGradient"u8))) == 0)
                {
                    nsvg__parseGradient(p, attr, NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT);
                }
                else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("radialGradient"u8))) == 0)
                {
                    nsvg__parseGradient(p, attr, NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT);
                }
                else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stop"u8))) == 0)
                {
                    nsvg__parseGradientStop(p, attr);
                }
                return;
            }

            if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("g"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parseAttribs(p, attr);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("path"u8))) == 0)
            {
                if (p->pathFlag)    // Do not allow nested paths.
                    return;
                nsvg__pushAttr(p);
                nsvg__parsePath(p, attr);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("rect"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parseRect(p, attr);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("circle"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parseCircle(p, attr);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("ellipse"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parseEllipse(p, attr);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("line"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parseLine(p, attr);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("polyline"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parsePoly(p, attr, false);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("polygon"u8))) == 0)
            {
                nsvg__pushAttr(p);
                nsvg__parsePoly(p, attr, true);
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("linearGradient"u8))) == 0)
            {
                nsvg__parseGradient(p, attr, NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("radialGradient"u8))) == 0)
            {
                nsvg__parseGradient(p, attr, NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("stop"u8))) == 0)
            {
                nsvg__parseGradientStop(p, attr);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("defs"u8))) == 0)
            {
                p->defsFlag = true;
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("svg"u8))) == 0)
            {
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/131
                nsvg__pushAttr(p);
#endif
                nsvg__parseSVG(p, attr);
            }
        }

        private static void nsvg__endElement(void* ud, byte* el)
        {
            NSVGparser* p = (NSVGparser*)ud;

            if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("g"u8))) == 0)
            {
                nsvg__popAttr(p);
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("path"u8))) == 0)
            {
                p->pathFlag = false;
            }
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("defs"u8))) == 0)
            {
                p->defsFlag = false;
            }
#if !NANOSVG_PORT_DISABLE_PATCHES // https://github.com/memononen/nanosvg/pull/131
            else if (strcmp(el, (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("svg"u8))) == 0)
            {
                nsvg__popAttr(p);
            }
#endif
        }

        private static void nsvg__content(void* ud, byte* s)
        {
            //NSVG_NOTUSED(ud);
            //NSVG_NOTUSED(s);
            // empty
        }

        private static void nsvg__imageBounds(NSVGparser* p, float* bounds)
        {
            NSVGshape* shape;
            shape = p->image->shapes;
            if (shape == null)
            {
                bounds[0] = bounds[1] = bounds[2] = bounds[3] = 0.0f;
                return;
            }
            bounds[0] = shape->bounds[0];
            bounds[1] = shape->bounds[1];
            bounds[2] = shape->bounds[2];
            bounds[3] = shape->bounds[3];
            for (shape = shape->next; shape != null; shape = shape->next)
            {
                bounds[0] = MathF.Min(bounds[0], shape->bounds[0]);
                bounds[1] = MathF.Min(bounds[1], shape->bounds[1]);
                bounds[2] = MathF.Max(bounds[2], shape->bounds[2]);
                bounds[3] = MathF.Max(bounds[3], shape->bounds[3]);
            }
        }

        private static float nsvg__viewAlign(float content, float container, int type)
        {
            if (type == NSVG_ALIGN_MIN)
                return 0;
            else if (type == NSVG_ALIGN_MAX)
                return container - content;
            // mid
            return (container - content) * 0.5f;
        }

        private static void nsvg__scaleGradient(NSVGgradient* grad, float tx, float ty, float sx, float sy)
        {
            float* t = stackalloc float[6];
            nsvg__xformSetTranslation(t, tx, ty);
            nsvg__xformMultiply(grad->xform, t);

            nsvg__xformSetScale(t, sx, sy);
            nsvg__xformMultiply(grad->xform, t);
        }

        private static void nsvg__scaleToViewbox(NSVGparser* p, byte* units)
        {
            NSVGshape* shape;
            NSVGpath* path;
            float tx, ty, sx, sy, us, avgs;
            float* bounds = stackalloc float[4], t = stackalloc float[6];
            int i;
            float* pt;

            // Guess image size if not set completely.
            nsvg__imageBounds(p, bounds);

            if (p->viewWidth == 0)
            {
                if (p->image->width > 0)
                {
                    p->viewWidth = p->image->width;
                }
                else
                {
                    p->viewMinx = bounds[0];
                    p->viewWidth = bounds[2] - bounds[0];
                }
            }
            if (p->viewHeight == 0)
            {
                if (p->image->height > 0)
                {
                    p->viewHeight = p->image->height;
                }
                else
                {
                    p->viewMiny = bounds[1];
                    p->viewHeight = bounds[3] - bounds[1];
                }
            }
            if (p->image->width == 0)
                p->image->width = p->viewWidth;
            if (p->image->height == 0)
                p->image->height = p->viewHeight;

            tx = -p->viewMinx;
            ty = -p->viewMiny;
            sx = p->viewWidth > 0 ? p->image->width / p->viewWidth : 0;
            sy = p->viewHeight > 0 ? p->image->height / p->viewHeight : 0;
            // Unit scaling
            us = 1.0f / nsvg__convertToPixels(p, nsvg__coord(1.0f, nsvg__parseUnits(units)), 0.0f, 1.0f);

            // Fix aspect ratio
            if (p->alignType == NSVG_ALIGN_MEET)
            {
                // fit whole image into viewbox
                sx = sy = MathF.Min(sx, sy);
                tx += nsvg__viewAlign(p->viewWidth * sx, p->image->width, p->alignX) / sx;
                ty += nsvg__viewAlign(p->viewHeight * sy, p->image->height, p->alignY) / sy;
            }
            else if (p->alignType == NSVG_ALIGN_SLICE)
            {
                // fill whole viewbox with image
                sx = sy = MathF.Max(sx, sy);
                tx += nsvg__viewAlign(p->viewWidth * sx, p->image->width, p->alignX) / sx;
                ty += nsvg__viewAlign(p->viewHeight * sy, p->image->height, p->alignY) / sy;
            }

            // Transform
            sx *= us;
            sy *= us;
            avgs = (sx + sy) / 2.0f;
            for (shape = p->image->shapes; shape != null; shape = shape->next)
            {
                shape->bounds[0] = (shape->bounds[0] + tx) * sx;
                shape->bounds[1] = (shape->bounds[1] + ty) * sy;
                shape->bounds[2] = (shape->bounds[2] + tx) * sx;
                shape->bounds[3] = (shape->bounds[3] + ty) * sy;
                for (path = shape->paths; path != null; path = path->next)
                {
                    path->bounds[0] = (path->bounds[0] + tx) * sx;
                    path->bounds[1] = (path->bounds[1] + ty) * sy;
                    path->bounds[2] = (path->bounds[2] + tx) * sx;
                    path->bounds[3] = (path->bounds[3] + ty) * sy;
                    for (i = 0; i < path->npts; i++)
                    {
                        pt = &path->pts[i * 2];
                        pt[0] = (pt[0] + tx) * sx;
                        pt[1] = (pt[1] + ty) * sy;
                    }
                }

                if (shape->fill.type == NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT || shape->fill.type == NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT)
                {
                    nsvg__scaleGradient(shape->fill.union.gradient, tx, ty, sx, sy);
                    memcpy(t, shape->fill.union.gradient->xform, sizeof(float) * 6);
                    nsvg__xformInverse(shape->fill.union.gradient->xform, t);
                }
                if (shape->stroke.type == NSVGpaintType.NSVG_PAINT_LINEAR_GRADIENT || shape->stroke.type == NSVGpaintType.NSVG_PAINT_RADIAL_GRADIENT)
                {
                    nsvg__scaleGradient(shape->stroke.union.gradient, tx, ty, sx, sy);
                    memcpy(t, shape->stroke.union.gradient->xform, sizeof(float) * 6);
                    nsvg__xformInverse(shape->stroke.union.gradient->xform, t);
                }

                shape->strokeWidth *= avgs;
                shape->strokeDashOffset *= avgs;
                for (i = 0; i < shape->strokeDashCount; i++)
                    shape->strokeDashArray[i] *= avgs;
            }
        }

        private static void nsvg__createGradients(NSVGparser* p)
        {
            NSVGshape* shape;
            float* inv = stackalloc float[6];
            float* localBounds = stackalloc float[4];

            for (shape = p->image->shapes; shape != null; shape = shape->next)
            {
                if (shape->fill.type == NSVGpaintType.NSVG_PAINT_UNDEF)
                {
                    if (shape->fillGradient[0] != '\0')
                    {
                        nsvg__xformInverse(inv, shape->xform);
                        nsvg__getLocalBounds(localBounds, shape, inv);
                        shape->fill.union.gradient = nsvg__createGradient(p, shape->fillGradient, localBounds, shape->xform, &shape->fill.type);
                    }
                    if (shape->fill.type == NSVGpaintType.NSVG_PAINT_UNDEF)
                    {
                        shape->fill.type = NSVGpaintType.NSVG_PAINT_NONE;
                    }
                }
                if (shape->stroke.type == NSVGpaintType.NSVG_PAINT_UNDEF)
                {
                    if (shape->strokeGradient[0] != '\0')
                    {
                        nsvg__xformInverse(inv, shape->xform);
                        nsvg__getLocalBounds(localBounds, shape, inv);
                        shape->stroke.union.gradient = nsvg__createGradient(p, shape->strokeGradient, localBounds, shape->xform, &shape->stroke.type);
                    }
                    if (shape->stroke.type == NSVGpaintType.NSVG_PAINT_UNDEF)
                    {
                        shape->stroke.type = NSVGpaintType.NSVG_PAINT_NONE;
                    }
                }
            }
        }

        public static NSVGimage* nsvgParse(byte* input, byte* units, float dpi)
        {
            NSVGparser* p;
            NSVGimage* ret = null;

            p = nsvg__createParser();
            if (p == null)
            {
                return null;
            }
            p->dpi = dpi;

            nsvg__parseXML(input, &nsvg__startElement, &nsvg__endElement, &nsvg__content, p);

            // Create gradients after all definitions have been parsed
            nsvg__createGradients(p);

            // Scale to viewBox
            nsvg__scaleToViewbox(p, units);

            ret = p->image;
            p->image = null;

            nsvg__deleteParser(p);

            return ret;
        }

        public static NSVGimage* nsvgParseFromFile(ReadOnlySpan<byte> filename, byte* units, float dpi)
        {
            int size;
            byte* data = null;
            NSVGimage* image = null;

            var name = Encoding.UTF8.GetString(filename);

            FileStream fs;
            try { fs = File.OpenRead(name); } catch { return null; }
            using var stream = fs;

            size = (int)stream.Length;
            data = (byte*)malloc(size + 1);
            if (data == null) goto error;
            if (stream.Read(new(data, size)) != size) goto error;
            data[size] = (byte)'\0';  // Must be null terminated.
            image = nsvgParse(data, units, dpi);
            free(data);

            return image;

        error:
            if (data is not null) free(data);
            if (image is not null) nsvgDelete(image);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NSVGimage* nsvgParseFromFile(byte* filename, byte* units, float dpi)
        {
            return nsvgParseFromFile(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(filename), units, dpi);
        }

        public static NSVGpath* nsvgDuplicatePath(NSVGpath* p)
        {
            NSVGpath* res = null;

            if (p == null)
                return null;

            res = (NSVGpath*)malloc(sizeof(NSVGpath));
            if (res == null) goto error;
            memset(res, 0, sizeof(NSVGpath));

            res->pts = (float*)malloc(p->npts * 2 * sizeof(float));
            if (res->pts == null) goto error;
            memcpy(res->pts, p->pts, p->npts * sizeof(float) * 2);
            res->npts = p->npts;

            memcpy(res->bounds, p->bounds, sizeof(float) * 4);

            res->closed = p->closed;

            return res;

        error:
            if (res != null)
            {
                free(res->pts);
                free(res);
            }
            return null;
        }

        public static void nsvgDelete(NSVGimage* image)
        {
            NSVGshape* snext, shape;
            if (image == null) return;
            shape = image->shapes;
            while (shape != null)
            {
                snext = shape->next;
                nsvg__deletePaths(shape->paths);
                nsvg__deletePaint(&shape->fill);
                nsvg__deletePaint(&shape->stroke);
                free(shape);
                shape = snext;
            }
            free(image);
        }
    }
}
