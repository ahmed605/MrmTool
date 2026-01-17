using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MrmTool
{
    internal static class StringEx
    {
        private const char DirectorySeparatorChar = '\\';
        private const char AltDirectorySeparatorChar = '/';

        [return: NotNullIfNotNull(nameof(path))]
        internal static string? GetExtensionAfterPeriod(this string path)
        {
            if (path == null)
                return null;

            return GetExtensionAfterPeriod(path.ToLowerInvariant().AsSpan()).ToString();
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
    }
}
