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
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => type is ResourceType.Text or ResourceType.Xaml;
            }

            internal bool IsPreviewedAsText
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => type is ResourceType.Text or ResourceType.Xaml or ResourceType.Xbf;
            }

            internal bool IsPreviewable // WIP
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => type is ResourceType.Text or ResourceType.Xaml or ResourceType.Xbf or ResourceType.Image;
            }
        }
    }
}
