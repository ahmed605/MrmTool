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
                if (lower.Length > 3 && lower.LastIndexOf('.') is int idx && idx > 0)
                {
                    switch (lower[idx..])
                    {
                        case ".xbf":
                            Type = ResourceType.Xbf;
                            break;

                        case ".png" or ".jpg" or ".gif" or ".bmp" or ".svg" or ".jpeg" or ".webp" or ".heif" or ".tiff":
                            Type = ResourceType.Image;
                            break;

                        case ".mp3" or ".wav" or ".wma" or ".ogg" or ".flac" or ".opus":
                            Type = ResourceType.Audio;
                            break;

                        case ".txt" or ".xml" or ".csv" or ".ini" or ".json" or ".html" or ".css" or ".js":
                            Type = ResourceType.Text;
                            break;

                        case ".mp4" or ".avi" or ".mov" or ".wmv" or ".mkv":
                            Type = ResourceType.Video;
                            break;

                        case ".ttf" or ".otf" or ".ttc":
                            Type = ResourceType.Font;
                            break;

                        case ".xaml":
                            Type = ResourceType.Xaml;
                            break;
                    }
                }

                if (Type is ResourceType.Unknown &&
                    Candidates.Count > 0 &&
                    Candidates[0].ValueType is ResourceValueType.String)
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
                    ResourceType.Font => Icons.Font.Value,
                    ResourceType.Xaml or ResourceType.Xbf => Icons.Xaml.Value,
                    _ => Icons.Unknown.Value,
                };
            }
        }
    }
}
