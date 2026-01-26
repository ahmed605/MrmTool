using MrmLib;
using MrmTool.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace MrmTool.Models
{
    public partial class ResourceItem(string name, ObservableCollection<ResourceItem> parent) : INotifyPropertyChanged
    {
        private string _name = name;
        private string _displayName = name.GetDisplayName();

        internal ObservableCollection<ResourceItem> Parent = parent;

        public string Name
        {
            get => _name;
            set
            {
                if (!_name.Equals(value, StringComparison.Ordinal))
                {
                    var oldNameLength = _name.Length;

                    _name = value;
                    _displayName = value.GetDisplayName();

                    foreach (var candidate in Candidates)
                    {
                        candidate.Candidate.ResourceName = value;
                    }

                    foreach (var child in Children)
                    {
                        child.Name = value + child.Name[oldNameLength..];
                    }

                    PropertyChanged?.Invoke(this, new(nameof(Name)));
                    PropertyChanged?.Invoke(this, new(nameof(DisplayName)));

                    EnsureIconAndType(true);
                }
            }
        }

        public string DisplayName 
        {
            get => _displayName;
            set
            {
                if (!_displayName.Equals(value, StringComparison.Ordinal))
                {
                    Name = Name.SetDisplayName(value);
                }
            }
        }

        public ObservableCollection<ResourceItem> Children { get; } = [];

        public ObservableCollection<CandidateItem> Candidates { get; } = [];

        public BitmapImage? Icon { get; private set; }

        internal ResourceType Type { get; private set; } = ResourceType.Unknown;

        internal bool IsFolder => Type is ResourceType.Folder || Children.Count > 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void DetermineType()
        {
            if (Children.Count > 0)
            {
                Type = ResourceType.Folder;
            }
            else
            {
                Type = DisplayName.DetermineResourceType();

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

                Icon = Type.GetCorrespondingIcon();
                PropertyChanged?.Invoke(this, new(nameof(Icon)));
            }
        }

        internal void Delete(PriFile pri, bool isChild = false)
        {
            foreach (var candidate in Candidates)
            {
                pri.ResourceCandidates.Remove(candidate.Candidate);
            }

            Candidates.Clear();

            foreach (var child in Children)
            {
                child.Delete(pri, true);
            }

            if (!isChild)
            {
                Parent.Remove(this);
            }

            Children.Clear();
        }
    }
}
