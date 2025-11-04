using MrmLib;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace MrmTool.Models
{
    public class ResourceItem(string name)
    {
        public string Name { get; } = name;

        public string DisplayName { get; } = name.GetDisplayName();

        public ObservableCollection<ResourceItem> Children { get; } = [];

        public List<ResourceCandidate> Candidates { get; } = [];

        public BitmapImage? Icon { get; private set; }

        internal ResourceType Type { get; private set; } = ResourceType.Unknown;

        private void DetermineType()
        {
            if (Children.Count > 0)
            {
                Type = ResourceType.Folder;
            }
            else
            {
                var lower = DisplayName.ToLowerInvariant();

                if (lower.Length > 4 && lower[^4..] is { } last4)
                {
                    if (last4[0] is '.')
                    {
                        if (last4 is ".xbf")
                        {
                            Type = ResourceType.Xaml;
                        }
                        else if (last4 is ".txt" or ".xml" or ".csv" or ".ini")
                        {
                            Type = ResourceType.Text;
                        }
                        else if (last4 is ".png" or ".jpg" or ".gif" or ".bmp" or ".svg")
                        {
                            Type = ResourceType.Image;
                        }
                        else if (last4 is ".mp3" or ".wav" or ".wma" or ".ogg")
                        {
                            Type = ResourceType.Audio;
                        }
                        else if (last4 is ".mp4" or ".avi" or ".mov" or ".wmv" or ".mkv")
                        {
                            Type = ResourceType.Video;
                        }
                        else if (last4 is ".ttf" or ".otf" or ".ttc")
                        {
                            Type = ResourceType.Font;
                        }
                    }
                    else if (lower.Length > 5 && lower[^5..] is { } last5 && last5[0] is '.')
                    {
                        if (last5 is ".xaml")
                        {
                            Type = ResourceType.Xaml;
                        }
                        else if (last5 is ".json")
                        {
                            Type = ResourceType.Text;
                        }
                        else if (last5 is ".jpeg" or ".webp" or ".heif" or ".tiff")
                        {
                            Type = ResourceType.Image;
                        }
                        else if (last5 is ".flac" or ".opus")
                        {
                            Type = ResourceType.Audio;
                        }
                    }
                }

                if (Type is ResourceType.Unknown &&
                    Candidates.Count > 0 &&
                    Candidates[0].ValueType == ResourceValueType.String)
                {
                    Type = ResourceType.Text;
                }
            }
        }

        internal void EnsureIconAndType()
        {
            if (Icon is null || (Type is not ResourceType.Folder && Children.Count > 0))
            {
                DetermineType();

                Icon = Type switch
                {
                    ResourceType.Folder => Icons.Folder.Value,
                    ResourceType.Text => Icons.Text.Value,
                    ResourceType.Image => Icons.Image.Value,
                    ResourceType.Audio => Icons.Audio.Value,
                    ResourceType.Video => Icons.Video.Value,
                    ResourceType.Xaml => Icons.Xaml.Value,
                    ResourceType.Font => Icons.Font.Value,
                    _ => Icons.Unknown.Value,
                };
            }
        }
    }
}
