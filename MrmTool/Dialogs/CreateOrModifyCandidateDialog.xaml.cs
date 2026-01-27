using MrmLib;
using MrmTool.Common;
using MrmTool.Models;
using MrmTool.Scintilla;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRT;
using static System.Net.Mime.MediaTypeNames;

namespace MrmTool.Dialogs
{
    public sealed partial class CreateOrModifyCandidateDialog : ContentDialog
    {
        private readonly CandidateItem? _candidate;
        private readonly ResourceItem _resourceItem;
        private readonly bool _isText;

        private IBuffer? _sourceBuffer;
        private StorageFile? _fileToEmbed;

        private bool _sourceChangedOnce = false;
        private bool _qualifierGridLoaded = false;
        private bool _isCreatingNewQualifier = false;

        private readonly ObservableCollection<Qualifier> Qualifiers = [];

        public CreateOrModifyCandidateDialog(ResourceItem resourceItem, CandidateItem? candidate = null)
        {
            _candidate = candidate;
            _resourceItem = resourceItem;
            _isText = resourceItem.Type.IsText;

            this.InitializeComponent();

            if (candidate is not null)
            {
                Title = "Modify Candidate";
                PrimaryButtonText = "Modify";
            }
        }

        internal new async Task<CandidateItem?> ShowAsync()
        {
            var result = await base.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                int qualifiersCount = Qualifiers.Count;
                var candidates = _resourceItem.Candidates;

                foreach (var candidate in candidates)
                {
                    if (candidate == _candidate)
                        continue;

                    var candidateQualifiers = candidate.CandidateQualifiers;

                    if (candidateQualifiers.Count != qualifiersCount)
                        continue;

                    bool isMatch = true;
                    foreach (var qualifier in Qualifiers)
                    {
                        bool found = false;
                        foreach (var candidateQualifier in candidateQualifiers)
                        {
                            if (qualifier.Matches(candidateQualifier))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        ContentDialog dialog = new()
                        {
                            Title = "Error",
                            Content = "A candidate with the same qualifiers already exists for this resource.",
                            CloseButtonText = "OK",
                            DefaultButton = ContentDialogButton.Close
                        };

                        await dialog.ShowAsync();
                        return null;
                    }
                }

                if (_candidate is not null)
                {
                    switch ((ResourceValueType)typeBox.SelectedIndex)
                    {
                        case ResourceValueType.String:
                            _candidate.SetValue(ResourceValueType.String, editorControl.Text);
                            break;

                        case ResourceValueType.Path:
                            _candidate.SetValue(ResourceValueType.Path, pathBox.Text);
                            break;

                        case ResourceValueType.EmbeddedData:
                            var idx = sourceBox.SelectedIndex;

                            if (idx is 0)
                            {
                                _candidate.DataValueBuffer = _sourceBuffer ?? await FileIO.ReadBufferAsync(_fileToEmbed);
                            }
                            else
                            {
                                using var data = idx is 1 ?
                                    Encoding.UTF8.GetBuffer(editorControl.Text) :
                                    Encoding.Unicode.GetBuffer(editorControl.Text);

                                _candidate.DataValueBuffer = data;
                            }

                            break;
                    }

                    _candidate.CandidateQualifiers = Qualifiers;
                    return _candidate;
                }
                else
                {
                    var name = _resourceItem.Name;

                    switch ((ResourceValueType)typeBox.SelectedIndex)
                    {
                        case ResourceValueType.String:
                            return ResourceCandidate.Create(name, ResourceValueType.String, editorControl.Text, Qualifiers);

                        case ResourceValueType.Path:
                            return ResourceCandidate.Create(name, ResourceValueType.Path, pathBox.Text, Qualifiers);

                        case ResourceValueType.EmbeddedData:
                            var idx = sourceBox.SelectedIndex;

                            if (idx is 0)
                            {
                                return ResourceCandidate.Create(name, _sourceBuffer ??
                                    await FileIO.ReadBufferAsync(_fileToEmbed), Qualifiers);
                            }
                            else
                            {
                                using var data = idx is 1 ?
                                    Encoding.UTF8.GetBuffer(editorControl.Text) :
                                    Encoding.Unicode.GetBuffer(editorControl.Text);

                                return ResourceCandidate.Create(name, data, Qualifiers);
                            }
                    }
                }
            }

            return null;
        }

        private void RestoreCandidateUI()
        {
            UnloadObject(qualifierGrid);
            candidateGrid.Visibility = Visibility.Visible;
            Title = _candidate is not null ? "Modify Candidate" : "Create New Candidate";
            PrimaryButtonText = _candidate is not null ? "Modify" : "Create";
            CloseButtonText = "Cancel";
            DefaultButton = ContentDialogButton.Primary;

            ValidateInputs();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_isCreatingNewQualifier)
            {
                Qualifiers.Add(Qualifier.Create(
                    (QualifierAttribute)attributeBox.SelectedIndex,
                    (QualifierOperator)operatorBox.SelectedIndex,
                    valueBox.Text,
                    int.Parse(priorityBox.Text),
                    double.Parse(fallbackBox.Text)));

                RestoreCandidateUI();
            }
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_isCreatingNewQualifier)
            {
                RestoreCandidateUI();
            }
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (_isCreatingNewQualifier)
            {
                args.Cancel = true;
                _isCreatingNewQualifier = false;
            }
        }

        private void ValidateInputs()
        {
            bool isContentValid = true;

            var type = (ResourceValueType)typeBox.SelectedIndex;
            if (type is ResourceValueType.Path)
            {
                isContentValid = !string.IsNullOrWhiteSpace(pathBox.Text);
            }
            else if (type is ResourceValueType.EmbeddedData)
            {
                isContentValid = sourceBox.SelectedIndex is 0 ?
                    (_fileToEmbed is not null || _sourceBuffer is not null) :
                    !string.IsNullOrEmpty(editorControl?.Text);
            }

            IsPrimaryButtonEnabled = isContentValid;
        }

        private void LoadEditor()
        {
            if (FindName(nameof(editorControl)) is not null)
            {
                var editor = editorControl.Editor;

                editor.WrapMode = WinUIEditor.Wrap.Word;
                editor.Modified += Editor_Modified;
            }
        }

        private void UnloadEditor()
        {
            editorControl?.Editor.Modified -= Editor_Modified;
            UnloadObject(editorControl);
        }

        private void Editor_Modified(WinUIEditor.Editor sender, WinUIEditor.ModifiedEventArgs args)
        {
            if ((args.ModificationType &
                (int)(WinUIEditor.ModificationFlags.InsertText | WinUIEditor.ModificationFlags.DeleteText)) > 0)
            {
                ValidateInputs();
            }
        }

        private void LoadControls(ResourceValueType type)
        {
            switch (type)
            {
                case ResourceValueType.String:
                    UnloadObject(pathContainer);
                    UnloadObject(dataContainer);
                    UnloadObject(browseButton);
                    FindName(nameof(stringContainer));
                    LoadEditor();

                    break;
                case ResourceValueType.Path:
                    UnloadObject(dataContainer);
                    UnloadObject(stringContainer);
                    UnloadObject(browseButton);
                    UnloadEditor();
                    FindName(nameof(pathContainer));

                    break;
                case ResourceValueType.EmbeddedData:
                    UnloadObject(pathContainer);
                    UnloadObject(stringContainer);
                    UnloadEditor();
                    FindName(nameof(dataContainer));
                    FindName(nameof(browseButton));

                    break;
            }
        }

        private void typeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadControls((ResourceValueType)typeBox.SelectedIndex);
            ValidateInputs();
        }

        private void sourceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sourceBox.SelectedIndex is 0) // File
            {
                UnloadEditor();
                FindName(nameof(browseButton));
            }
            else // Text String
            {
                UnloadObject(browseButton);
                LoadEditor();
            }

            _fileToEmbed = null;

            if (!_sourceChangedOnce)
            {
                _sourceBuffer = null;
                _sourceChangedOnce = true;
            }

            ValidateInputs();
        }

        private void pathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInputs();
        }

        private async void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            picker.Initialize();

            _fileToEmbed = await picker.PickSingleFileAsync();

            if (_fileToEmbed is not null)
            {
                browseBlock.Text = _fileToEmbed.Name;
            }

            _sourceBuffer = null;

            ValidateInputs();
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            picker.Initialize();

            if (await picker.PickSingleFileAsync() is { } file)
            {
                editorControl.Text = await FileIO.ReadTextAsync(file);
            }
        }

        private unsafe void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (_candidate is not null)
            {
                var type = _candidate.ValueType;
                var oldIndex = typeBox.SelectedIndex;

                typeBox.SelectedIndex = (int)type;

                if ((int)type == oldIndex)
                {
                    LoadControls(type);
                }

                switch (type)
                {
                    case ResourceValueType.String:
                        editorControl.Text = _candidate.StringValue;

                        break;
                    case ResourceValueType.Path:
                        pathBox.Text = _candidate.StringValue;

                        break;
                    case ResourceValueType.EmbeddedData:
                        if (_isText)
                        {
                            sourceBox.SelectedIndex = 1;
                            var data = _candidate.Candidate.DataValueReference;
                            editorControl.Text = Encoding.UTF8.GetString(data.GetData(), (int)data.Length);
                        }
                        else
                        {
                            sourceBox.SelectedIndex = 0;
                            _sourceBuffer = _candidate.Candidate.DataValueReference;
                            browseBlock.Text = "(Embedded Data)";
                        }

                        break;
                }

                var qualifiers = _candidate.Candidate.Qualifiers;
                foreach (var qualifier in qualifiers)
                {
                    Qualifiers.Add(qualifier);
                }
            }
            else if (_resourceItem.Candidates.Count > 0)
            {
                var type = (int)_resourceItem.Candidates[0].ValueType;
                var oldIndex = typeBox.SelectedIndex;
                typeBox.SelectedIndex = type;

                if (type == oldIndex)
                {
                    LoadControls((ResourceValueType)type);
                }
            }
            else
            {
                LoadControls((ResourceValueType)typeBox.SelectedIndex);
            }

            ValidateInputs();
        }

        [DynamicWindowsRuntimeCast(typeof(Qualifier))]
        private void deleteQualifierButton_Click(object sender, RoutedEventArgs e)
        {
            Qualifiers.Remove((Qualifier)qualifiersList.SelectedItem);
        }

        private void addQualifierButton_Click(object sender, RoutedEventArgs e)
        {
            _qualifierGridLoaded = false;
            _isCreatingNewQualifier = true;
            candidateGrid.Visibility = Visibility.Collapsed;
            FindName(nameof(qualifierGrid));

            Title = "Add New Qualifier";
            PrimaryButtonText = "Add Qualifier";
            CloseButtonText = "Go Back";
            DefaultButton = ContentDialogButton.Primary;

            ValidateQualifierInputs();
        }

        private void qualifiersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                deleteQualifierButton.IsEnabled = true;
            }
            else
            {
                deleteQualifierButton.IsEnabled = false;
            }
        }

        private void ValidateQualifierInputs()
        {
            string text;
            IsPrimaryButtonEnabled = _qualifierGridLoaded &&
                                     !string.IsNullOrWhiteSpace(text = valueBox.Text) &&
                                     int.TryParse(priorityBox.Text, out var priority) &&
                                     double.TryParse(fallbackBox.Text, out var fallback) &&
                                     !Qualifiers.Any(x => x.Matches(
                                         (QualifierAttribute)attributeBox.SelectedIndex,
                                         (QualifierOperator)operatorBox.SelectedIndex,
                                         text,
                                         priority,
                                         fallback));
        }

        private void qualifierBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateQualifierInputs();
        }

        private void qualifierComboBox_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateQualifierInputs();
        }

        private void qualifierGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _qualifierGridLoaded = true;
            ValidateQualifierInputs();
        }
    }
}
