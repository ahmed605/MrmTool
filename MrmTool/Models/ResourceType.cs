using System.Runtime.CompilerServices;

namespace MrmTool.Models
{
    internal enum ResourceType
    {
        Unknown,
        Folder,
        Text,
        Image,
        Xaml,
        Xbf,
        Audio,
        Video,
        Font
    }

    internal static class ResourceTypeEx
    {
        extension(ResourceType type)
        {
            internal bool IsText
            {
                // TODO: Add XBF to IsText when we include the decompiler,
                // we probably need to remove it later when we have a proper XAML viewer/editor though

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => type is ResourceType.Text or ResourceType.Xaml;
            }
        }
    }
}
