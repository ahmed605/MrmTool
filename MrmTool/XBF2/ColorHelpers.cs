using System.Runtime.CompilerServices;

namespace XbfAnalyzer.Xbf
{
    internal static class ColorHelpers
    {
        private static ReadOnlySpan<uint> KnownColors
        {
            get =>
            [
                0xFFF0F8FF, // AliceBlue
                0xFFFAEBD7, // AntiqueWhite
                0xFF00FFFF, // Aqua
                0xFF7FFFD4, // Aquamarine
                0xFFF0FFFF, // Azure
                0xFFF5F5DC, // Beige
                0xFFFFE4C4, // Bisque
                0xFF000000, // Black
                0xFFFFEBCD, // BlanchedAlmond
                0xFF0000FF, // Blue
                0xFF8A2BE2, // BlueViolet
                0xFFA52A2A, // Brown
                0xFFDEB887, // BurlyWood
                0xFF5F9EA0, // CadetBlue
                0xFF7FFF00, // Chartreuse
                0xFFD2691E, // Chocolate
                0xFFFF7F50, // Coral
                0xFF6495ED, // CornflowerBlue
                0xFFFFF8DC, // Cornsilk
                0xFFDC143C, // Crimson
                0xFF00FFFF, // Cyan
                0xFF00008B, // DarkBlue
                0xFF008B8B, // DarkCyan
                0xFFB8860B, // DarkGoldenrod
                0xFFA9A9A9, // DarkGray
                0xFF006400, // DarkGreen
                0xFFBDB76B, // DarkKhaki
                0xFF8B008B, // DarkMagenta
                0xFF556B2F, // DarkOliveGreen
                0xFFFF8C00, // DarkOrange
                0xFF9932CC, // DarkOrchid
                0xFF8B0000, // DarkRed
                0xFFE9967A, // DarkSalmon
                0xFF8FBC8F, // DarkSeaGreen
                0xFF483D8B, // DarkSlateBlue
                0xFF2F4F4F, // DarkSlateGray
                0xFF00CED1, // DarkTurquoise
                0xFF9400D3, // DarkViolet
                0xFFFF1493, // DeepPink
                0xFF00BFFF, // DeepSkyBlue
                0xFF696969, // DimGray
                0xFF1E90FF, // DodgerBlue
                0xFFB22222, // Firebrick
                0xFFFFFAF0, // FloralWhite
                0xFF228B22, // ForestGreen
                0xFFFF00FF, // Fuchsia
                0xFFDCDCDC, // Gainsboro
                0xFFF8F8FF, // GhostWhite
                0xFFFFD700, // Gold
                0xFFDAA520, // Goldenrod
                0xFF808080, // Gray
                0xFF008000, // Green
                0xFFADFF2F, // GreenYellow
                0xFFF0FFF0, // Honeydew
                0xFFFF69B4, // HotPink
                0xFFCD5C5C, // IndianRed
                0xFF4B0082, // Indigo
                0xFFFFFFF0, // Ivory
                0xFFF0E68C, // Khaki
                0xFFE6E6FA, // Lavender
                0xFFFFF0F5, // LavenderBlush
                0xFF7CFC00, // LawnGreen
                0xFFFFFACD, // LemonChiffon
                0xFFADD8E6, // LightBlue
                0xFFF08080, // LightCoral
                0xFFE0FFFF, // LightCyan
                0xFFFAFAD2, // LightGoldenrodYellow
                0xFFD3D3D3, // LightGray
                0xFF90EE90, // LightGreen
                0xFFFFB6C1, // LightPink
                0xFFFFA07A, // LightSalmon
                0xFF20B2AA, // LightSeaGreen
                0xFF87CEFA, // LightSkyBlue
                0xFF778899, // LightSlateGray
                0xFFB0C4DE, // LightSteelBlue
                0xFFFFFFE0, // LightYellow
                0xFF00FF00, // Lime
                0xFF32CD32, // LimeGreen
                0xFFFAF0E6, // Linen
                0xFFFF00FF, // Magenta
                0xFF800000, // Maroon
                0xFF66CDAA, // MediumAquamarine
                0xFF0000CD, // MediumBlue
                0xFFBA55D3, // MediumOrchid
                0xFF9370DB, // MediumPurple
                0xFF3CB371, // MediumSeaGreen
                0xFF7B68EE, // MediumSlateBlue
                0xFF00FA9A, // MediumSpringGreen
                0xFF48D1CC, // MediumTurquoise
                0xFFC71585, // MediumVioletRed
                0xFF191970, // MidnightBlue
                0xFFF5FFFA, // MintCream
                0xFFFFE4E1, // MistyRose
                0xFFFFE4B5, // Moccasin
                0xFFFFDEAD, // NavajoWhite
                0xFF000080, // Navy
                0xFFFDF5E6, // OldLace
                0xFF808000, // Olive
                0xFF6B8E23, // OliveDrab
                0xFFFFA500, // Orange
                0xFFFF4500, // OrangeRed
                0xFFDA70D6, // Orchid
                0xFFEEE8AA, // PaleGoldenrod
                0xFF98FB98, // PaleGreen
                0xFFAFEEEE, // PaleTurquoise
                0xFFDB7093, // PaleVioletRed
                0xFFFFEFD5, // PapayaWhip
                0xFFFFDAB9, // PeachPuff
                0xFFCD853F, // Peru
                0xFFFFC0CB, // Pink
                0xFFDDA0DD, // Plum
                0xFFB0E0E6, // PowderBlue
                0xFF800080, // Purple
                0xFFFF0000, // Red
                0xFFBC8F8F, // RosyBrown
                0xFF4169E1, // RoyalBlue
                0xFF8B4513, // SaddleBrown
                0xFFFA8072, // Salmon
                0xFFF4A460, // SandyBrown
                0xFF2E8B57, // SeaGreen
                0xFFFFF5EE, // SeaShell
                0xFFA0522D, // Sienna
                0xFFC0C0C0, // Silver
                0xFF87CEEB, // SkyBlue
                0xFF6A5ACD, // SlateBlue
                0xFF708090, // SlateGray
                0xFFFFFAFA, // Snow
                0xFF00FF7F, // SpringGreen
                0xFF4682B4, // SteelBlue
                0xFFD2B48C, // Tan
                0xFF008080, // Teal
                0xFFD8BFD8, // Thistle
                0xFFFF6347, // Tomato
                0x00FFFFFF, // Transparent
                0xFF40E0D0, // Turquoise
                0xFFEE82EE, // Violet
                0xFFF5DEB3, // Wheat
                0xFFFFFFFF, // White
                0xFFF5F5F5, // WhiteSmoke
                0xFFFFFF00, // Yellow
                0xFF9ACD32, // YellowGreen
            ];
        }

        private static ReadOnlySpan<char> KnownColorNames
        {
            get =>
            [
                (char)141, // AliceBlue
                (char)151, // AntiqueWhite
                (char)164, // Aqua
                (char)169, // Aquamarine
                (char)180, // Azure
                (char)186, // Beige
                (char)192, // Bisque
                (char)199, // Black
                (char)205, // BlanchedAlmond
                (char)220, // Blue
                (char)225, // BlueViolet
                (char)236, // Brown
                (char)242, // BurlyWood
                (char)252, // CadetBlue
                (char)262, // Chartreuse
                (char)273, // Chocolate
                (char)283, // Coral
                (char)289, // CornflowerBlue
                (char)304, // Cornsilk
                (char)313, // Crimson
                (char)321, // Cyan
                (char)326, // DarkBlue
                (char)335, // DarkCyan
                (char)344, // DarkGoldenrod
                (char)358, // DarkGray
                (char)367, // DarkGreen
                (char)377, // DarkKhaki
                (char)387, // DarkMagenta
                (char)399, // DarkOliveGreen
                (char)414, // DarkOrange
                (char)425, // DarkOrchid
                (char)436, // DarkRed
                (char)444, // DarkSalmon
                (char)455, // DarkSeaGreen
                (char)468, // DarkSlateBlue
                (char)482, // DarkSlateGray
                (char)496, // DarkTurquoise
                (char)510, // DarkViolet
                (char)521, // DeepPink
                (char)530, // DeepSkyBlue
                (char)542, // DimGray
                (char)550, // DodgerBlue
                (char)561, // Firebrick
                (char)571, // FloralWhite
                (char)583, // ForestGreen
                (char)595, // Fuchsia
                (char)603, // Gainsboro
                (char)613, // GhostWhite
                (char)624, // Gold
                (char)629, // Goldenrod
                (char)639, // Gray
                (char)644, // Green
                (char)650, // GreenYellow
                (char)662, // Honeydew
                (char)671, // HotPink
                (char)679, // IndianRed
                (char)689, // Indigo
                (char)696, // Ivory
                (char)702, // Khaki
                (char)708, // Lavender
                (char)717, // LavenderBlush
                (char)731, // LawnGreen
                (char)741, // LemonChiffon
                (char)754, // LightBlue
                (char)764, // LightCoral
                (char)775, // LightCyan
                (char)785, // LightGoldenrodYellow
                (char)806, // LightGray
                (char)816, // LightGreen
                (char)827, // LightPink
                (char)837, // LightSalmon
                (char)849, // LightSeaGreen
                (char)863, // LightSkyBlue
                (char)876, // LightSlateGray
                (char)891, // LightSteelBlue
                (char)906, // LightYellow
                (char)918, // Lime
                (char)923, // LimeGreen
                (char)933, // Linen
                (char)939, // Magenta
                (char)947, // Maroon
                (char)954, // MediumAquamarine
                (char)971, // MediumBlue
                (char)982, // MediumOrchid
                (char)995, // MediumPurple
                (char)1008, // MediumSeaGreen
                (char)1023, // MediumSlateBlue
                (char)1039, // MediumSpringGreen
                (char)1057, // MediumTurquoise
                (char)1073, // MediumVioletRed
                (char)1089, // MidnightBlue
                (char)1102, // MintCream
                (char)1112, // MistyRose
                (char)1122, // Moccasin
                (char)1131, // NavajoWhite
                (char)1143, // Navy
                (char)1148, // OldLace
                (char)1156, // Olive
                (char)1162, // OliveDrab
                (char)1172, // Orange
                (char)1179, // OrangeRed
                (char)1189, // Orchid
                (char)1196, // PaleGoldenrod
                (char)1210, // PaleGreen
                (char)1220, // PaleTurquoise
                (char)1234, // PaleVioletRed
                (char)1248, // PapayaWhip
                (char)1259, // PeachPuff
                (char)1269, // Peru
                (char)1274, // Pink
                (char)1279, // Plum
                (char)1284, // PowderBlue
                (char)1295, // Purple
                (char)1302, // Red
                (char)1306, // RosyBrown
                (char)1316, // RoyalBlue
                (char)1326, // SaddleBrown
                (char)1338, // Salmon
                (char)1345, // SandyBrown
                (char)1356, // SeaGreen
                (char)1365, // SeaShell
                (char)1374, // Sienna
                (char)1381, // Silver
                (char)1388, // SkyBlue
                (char)1396, // SlateBlue
                (char)1406, // SlateGray
                (char)1416, // Snow
                (char)1421, // SpringGreen
                (char)1433, // SteelBlue
                (char)1443, // Tan
                (char)1447, // Teal
                (char)1452, // Thistle
                (char)1460, // Tomato
                (char)1467, // Transparent
                (char)1479, // Turquoise
                (char)1489, // Violet
                (char)1496, // Wheat
                (char)1502, // White
                (char)1508, // WhiteSmoke
                (char)1519, // Yellow
                (char)1526, // YellowGreen
                'A','l','i','c','e','B','l','u','e','\0',
                'A','n','t','i','q','u','e','W','h','i','t','e','\0',
                'A','q','u','a','\0',
                'A','q','u','a','m','a','r','i','n','e','\0',
                'A','z','u','r','e','\0',
                'B','e','i','g','e','\0',
                'B','i','s','q','u','e','\0',
                'B','l','a','c','k','\0',
                'B','l','a','n','c','h','e','d','A','l','m','o','n','d','\0',
                'B','l','u','e','\0',
                'B','l','u','e','V','i','o','l','e','t','\0',
                'B','r','o','w','n','\0',
                'B','u','r','l','y','W','o','o','d','\0',
                'C','a','d','e','t','B','l','u','e','\0',
                'C','h','a','r','t','r','e','u','s','e','\0',
                'C','h','o','c','o','l','a','t','e','\0',
                'C','o','r','a','l','\0',
                'C','o','r','n','f','l','o','w','e','r','B','l','u','e','\0',
                'C','o','r','n','s','i','l','k','\0',
                'C','r','i','m','s','o','n','\0',
                'C','y','a','n','\0',
                'D','a','r','k','B','l','u','e','\0',
                'D','a','r','k','C','y','a','n','\0',
                'D','a','r','k','G','o','l','d','e','n','r','o','d','\0',
                'D','a','r','k','G','r','a','y','\0',
                'D','a','r','k','G','r','e','e','n','\0',
                'D','a','r','k','K','h','a','k','i','\0',
                'D','a','r','k','M','a','g','e','n','t','a','\0',
                'D','a','r','k','O','l','i','v','e','G','r','e','e','n','\0',
                'D','a','r','k','O','r','a','n','g','e','\0',
                'D','a','r','k','O','r','c','h','i','d','\0',
                'D','a','r','k','R','e','d','\0',
                'D','a','r','k','S','a','l','m','o','n','\0',
                'D','a','r','k','S','e','a','G','r','e','e','n','\0',
                'D','a','r','k','S','l','a','t','e','B','l','u','e','\0',
                'D','a','r','k','S','l','a','t','e','G','r','a','y','\0',
                'D','a','r','k','T','u','r','q','u','o','i','s','e','\0',
                'D','a','r','k','V','i','o','l','e','t','\0',
                'D','e','e','p','P','i','n','k','\0',
                'D','e','e','p','S','k','y','B','l','u','e','\0',
                'D','i','m','G','r','a','y','\0',
                'D','o','d','g','e','r','B','l','u','e','\0',
                'F','i','r','e','b','r','i','c','k','\0',
                'F','l','o','r','a','l','W','h','i','t','e','\0',
                'F','o','r','e','s','t','G','r','e','e','n','\0',
                'F','u','c','h','s','i','a','\0',
                'G','a','i','n','s','b','o','r','o','\0',
                'G','h','o','s','t','W','h','i','t','e','\0',
                'G','o','l','d','\0',
                'G','o','l','d','e','n','r','o','d','\0',
                'G','r','a','y','\0',
                'G','r','e','e','n','\0',
                'G','r','e','e','n','Y','e','l','l','o','w','\0',
                'H','o','n','e','y','d','e','w','\0',
                'H','o','t','P','i','n','k','\0',
                'I','n','d','i','a','n','R','e','d','\0',
                'I','n','d','i','g','o','\0',
                'I','v','o','r','y','\0',
                'K','h','a','k','i','\0',
                'L','a','v','e','n','d','e','r','\0',
                'L','a','v','e','n','d','e','r','B','l','u','s','h','\0',
                'L','a','w','n','G','r','e','e','n','\0',
                'L','e','m','o','n','C','h','i','f','f','o','n','\0',
                'L','i','g','h','t','B','l','u','e','\0',
                'L','i','g','h','t','C','o','r','a','l','\0',
                'L','i','g','h','t','C','y','a','n','\0',
                'L','i','g','h','t','G','o','l','d','e','n','r','o','d','Y','e','l','l','o','w','\0',
                'L','i','g','h','t','G','r','a','y','\0',
                'L','i','g','h','t','G','r','e','e','n','\0',
                'L','i','g','h','t','P','i','n','k','\0',
                'L','i','g','h','t','S','a','l','m','o','n','\0',
                'L','i','g','h','t','S','e','a','G','r','e','e','n','\0',
                'L','i','g','h','t','S','k','y','B','l','u','e','\0',
                'L','i','g','h','t','S','l','a','t','e','G','r','a','y','\0',
                'L','i','g','h','t','S','t','e','e','l','B','l','u','e','\0',
                'L','i','g','h','t','Y','e','l','l','o','w','\0',
                'L','i','m','e','\0',
                'L','i','m','e','G','r','e','e','n','\0',
                'L','i','n','e','n','\0',
                'M','a','g','e','n','t','a','\0',
                'M','a','r','o','o','n','\0',
                'M','e','d','i','u','m','A','q','u','a','m','a','r','i','n','e','\0',
                'M','e','d','i','u','m','B','l','u','e','\0',
                'M','e','d','i','u','m','O','r','c','h','i','d','\0',
                'M','e','d','i','u','m','P','u','r','p','l','e','\0',
                'M','e','d','i','u','m','S','e','a','G','r','e','e','n','\0',
                'M','e','d','i','u','m','S','l','a','t','e','B','l','u','e','\0',
                'M','e','d','i','u','m','S','p','r','i','n','g','G','r','e','e','n','\0',
                'M','e','d','i','u','m','T','u','r','q','u','o','i','s','e','\0',
                'M','e','d','i','u','m','V','i','o','l','e','t','R','e','d','\0',
                'M','i','d','n','i','g','h','t','B','l','u','e','\0',
                'M','i','n','t','C','r','e','a','m','\0',
                'M','i','s','t','y','R','o','s','e','\0',
                'M','o','c','c','a','s','i','n','\0',
                'N','a','v','a','j','o','W','h','i','t','e','\0',
                'N','a','v','y','\0',
                'O','l','d','L','a','c','e','\0',
                'O','l','i','v','e','\0',
                'O','l','i','v','e','D','r','a','b','\0',
                'O','r','a','n','g','e','\0',
                'O','r','a','n','g','e','R','e','d','\0',
                'O','r','c','h','i','d','\0',
                'P','a','l','e','G','o','l','d','e','n','r','o','d','\0',
                'P','a','l','e','G','r','e','e','n','\0',
                'P','a','l','e','T','u','r','q','u','o','i','s','e','\0',
                'P','a','l','e','V','i','o','l','e','t','R','e','d','\0',
                'P','a','p','a','y','a','W','h','i','p','\0',
                'P','e','a','c','h','P','u','f','f','\0',
                'P','e','r','u','\0',
                'P','i','n','k','\0',
                'P','l','u','m','\0',
                'P','o','w','d','e','r','B','l','u','e','\0',
                'P','u','r','p','l','e','\0',
                'R','e','d','\0',
                'R','o','s','y','B','r','o','w','n','\0',
                'R','o','y','a','l','B','l','u','e','\0',
                'S','a','d','d','l','e','B','r','o','w','n','\0',
                'S','a','l','m','o','n','\0',
                'S','a','n','d','y','B','r','o','w','n','\0',
                'S','e','a','G','r','e','e','n','\0',
                'S','e','a','S','h','e','l','l','\0',
                'S','i','e','n','n','a','\0',
                'S','i','l','v','e','r','\0',
                'S','k','y','B','l','u','e','\0',
                'S','l','a','t','e','B','l','u','e','\0',
                'S','l','a','t','e','G','r','a','y','\0',
                'S','n','o','w','\0',
                'S','p','r','i','n','g','G','r','e','e','n','\0',
                'S','t','e','e','l','B','l','u','e','\0',
                'T','a','n','\0',
                'T','e','a','l','\0',
                'T','h','i','s','t','l','e','\0',
                'T','o','m','a','t','o','\0',
                'T','r','a','n','s','p','a','r','e','n','t','\0',
                'T','u','r','q','u','o','i','s','e','\0',
                'V','i','o','l','e','t','\0',
                'W','h','e','a','t','\0',
                'W','h','i','t','e','\0',
                'W','h','i','t','e','S','m','o','k','e','\0',
                'Y','e','l','l','o','w','\0',
                'Y','e','l','l','o','w','G','r','e','e','n','\0',
            ];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint FromArgb(byte a, byte r, byte g, byte b)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

        internal static unsafe string GetString(byte a, byte r, byte g, byte b)
        {
            int idx;
            if ((idx = KnownColors.IndexOf(FromArgb(a, r, g, b))) is not -1)
            {
                return new((char*)Unsafe.AsPointer(in KnownColorNames[KnownColorNames[idx]]));
            }
            else
            {
                return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
            }
        }
    }
}
