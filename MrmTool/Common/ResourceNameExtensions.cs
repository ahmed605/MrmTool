using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MrmTool.Models;

namespace MrmTool.Common;

internal static class ResourceNameExtensions
{
    private const char DirectorySeparatorChar = '\\';
    private const char AltDirectorySeparatorChar = '/';

    internal static string GetDisplayName(this string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var index = name.LastIndexOf('/');
        return index >= 0 ? name[(index + 1)..] : name;
    }

    internal static string SetDisplayName(this string name, string displayName)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(displayName);

        var index = name.LastIndexOf('/');
        return index >= 0 ? $"{name[..(index + 1)]}{displayName}" : displayName;
    }

    internal static string? GetParentName(this string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var index = name.LastIndexOf('/');
        return index >= 0 ? name[..index] : null;
    }

    [return: NotNullIfNotNull(nameof(path))]
    internal static string? GetExtensionAfterPeriod(this string path)
    {
        if (path == null)
            return null;

        return path.ToLowerInvariant().AsSpan().GetExtensionAfterPeriod().ToString();
    }

    internal static ReadOnlySpan<char> GetExtensionAfterPeriod(this ReadOnlySpan<char> path)
    {
        for (var index = path.Length - 1; index >= 0; index--)
        {
            var character = path[index];
            if (character == '.')
            {
                return index != path.Length - 1 ? path[(index + 1)..] : ReadOnlySpan<char>.Empty;
            }

            if (IsDirectorySeparator(character))
            {
                break;
            }
        }

        return ReadOnlySpan<char>.Empty;
    }

    internal static ResourceType DetermineResourceType(this string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Path.GetExtension(name).ToLowerInvariant() switch
        {
            ".xbf" => ResourceType.Xbf,
            ".xaml" => ResourceType.Xaml,
            ".ttf" or ".otf" or ".ttc" => ResourceType.Font,
            ".mp4" or ".avi" or ".mov" or ".wmv" or ".mkv" or ".webm" => ResourceType.Video,
            ".mp3" or ".wav" or ".wma" or ".ogg" or ".flac" or ".opus" => ResourceType.Audio,
            ".png" or ".jpg" or ".gif" or ".bmp" or ".jpeg" or ".webp" or ".heif" or ".tiff" => ResourceType.Image,
            ".svg" => ResourceType.Svg,
            ".txt" or ".xml" or ".xsl" or ".csv" or ".ini" or ".inf" or ".json" or ".html" or ".htm" or
                ".css" or ".scss" or ".less" or ".hss" or ".js" or ".cs" or ".resw" or ".resx" => ResourceType.Text,
            _ => ResourceType.Unknown
        };
    }

    internal static string[] SplitIntoResourceNames(this string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        var separatorCount = resourceName.Count('/');
        if (separatorCount == 0)
        {
            return [resourceName];
        }

        var result = new string[separatorCount + 1];
        var resultIndex = 0;
        var currentIndex = -1;

        while ((currentIndex = resourceName.IndexOf('/', currentIndex + 1)) >= 0)
        {
            result[resultIndex++] = resourceName[..currentIndex];
        }

        result[resultIndex] = resourceName;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDirectorySeparator(char character)
    {
        return character is DirectorySeparatorChar or AltDirectorySeparatorChar;
    }
}
