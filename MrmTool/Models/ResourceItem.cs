using MrmLib;
using MrmTool.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace MrmTool.Models
{
    public partial class ResourceItem(string name) : INotifyPropertyChanged
    {
        public string Name { get; private set; } = name;

        public string DisplayName 
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;

                    var newName = Name.SetDisplayName(value);
                    Name = newName;

                    PropertyChanged?.Invoke(this, new(nameof(DisplayName)));
                    PropertyChanged?.Invoke(this, new(nameof(Name)));

                    EnsureIconAndType(true);

                    foreach (var candidate in Candidates)
                    {
                        candidate.Candidate.ResourceName = newName;
                    }
                }
            }
        } = name.GetDisplayName();

        public ObservableCollection<ResourceItem> Children { get; } = [];

        public ObservableCollection<CandidateItem> Candidates { get; } = [];

        public BitmapImage? Icon { get; private set; }

        internal ResourceType Type { get; private set; } = ResourceType.Unknown;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void DetermineType()
        {
            if (Children.Count > 0)
            {
                Type = ResourceType.Folder;
            }
            else
            {
                Type = Path.GetExtension(DisplayName).ToLower() switch
                {
                    ".xbf"
                        => ResourceType.Xbf,

                    ".png" or ".jpg" or ".gif" or ".bmp" or ".svg" or ".jpeg" or ".webp" or ".heif" or ".tiff"
                        => ResourceType.Image,

                    ".mp3" or ".wav" or ".wma" or ".ogg" or ".flac" or ".opus"
                        => ResourceType.Audio,

                    ".txt" or ".xml" or ".csv" or ".ini" or ".json" or ".html" or ".css" or ".js"
                        => ResourceType.Text,

                    ".mp4" or ".avi" or ".mov" or ".wmv" or ".mkv"
                        => ResourceType.Video,

                    ".ttf" or ".otf" or ".ttc"
                        => ResourceType.Font,

                    ".xaml"
                        => ResourceType.Xaml,

                    _
                        => ResourceType.Unknown
                };

                if (Type is ResourceType.Unknown &&
                    Candidates.Count > 0 &&
                    Candidates[0].Candidate.ValueType is ResourceValueType.String)
                {
                    Type = ResourceType.Text;
                }
            }
        }

        internal void EnsureIconAndType(bool changed = false)
        {
            if (changed is true || Icon is null || (Type is not ResourceType.Folder && Children.Count > 0))
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

                PropertyChanged?.Invoke(this, new(nameof(Icon)));
            }
        }
    }
}
