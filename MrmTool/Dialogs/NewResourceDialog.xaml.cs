using MrmLib;
using MrmTool.Common;
using MrmTool.Models;
using MrmTool.Scintilla;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Dialogs
{
    public sealed partial class NewResourceDialog : ContentDialog
    {
        private readonly PriFile _priFile;
        private readonly string? _parentName;

        private StorageFile? _fileToEmbed;

        public NewResourceDialog(PriFile priFile, string? parentName = null)
        {
            _priFile = priFile;
            _parentName = parentName;

            this.InitializeComponent();
        }

        internal new async Task<CandidateItem?> ShowAsync()
        {
            var result = await base.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var name = nameBox.Text;

                foreach (var candidate in _priFile.ResourceCandidates)
                {
                    if (candidate.ResourceName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        ContentDialog dialog = new()
                        {
                            Title = "Error",
                            Content = "A resource with the same name already exists",
                            CloseButtonText = "OK",
                            DefaultButton = ContentDialogButton.Close
                        };

                        await dialog.ShowAsync();
                        return null;
                    }
                }

                switch ((ResourceValueType)typeBox.SelectedIndex)
                {
                    case ResourceValueType.String:
                        return ResourceCandidate.Create(name, ResourceValueType.String, editorControl.Text);

                    case ResourceValueType.Path:
                        return ResourceCandidate.Create(name, ResourceValueType.Path, pathBox.Text);

                    case ResourceValueType.EmbeddedData:
                        var idx = sourceBox.SelectedIndex;

                        if (idx is 0)
                        {
                            return ResourceCandidate.Create(name, await FileIO.ReadBufferAsync(_fileToEmbed));
                        }
                        else
                        {
                            using var data = idx is 1 ?
                                Encoding.UTF8.GetBuffer(editorControl.Text) :
                                Encoding.Unicode.GetBuffer(editorControl.Text);

                            return ResourceCandidate.Create(name, data);
                        }
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        private void ValidateInputs(string name)
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
                    _fileToEmbed is not null :
                    !string.IsNullOrEmpty(editorControl?.Text);
            }

            IsPrimaryButtonEnabled = isContentValid &&
                                     !string.IsNullOrWhiteSpace(name) &&
                                     !name.StartsWith('/') &&
                                     !name.EndsWith('/');
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = nameBox.Text;
            var type = text.DetermineResourceType();
            var largeIcon = type.GetCorrespondingLargeIcon();

            if (icon.Source != largeIcon)
            {
                icon.Source = largeIcon;
            }

            ValidateInputs(text);

            if (editorControl is not null && type.IsText)
            {
                editorControl.HighlightingLanguage = type is ResourceType.Xaml ?
                    "xml" :
                    text.GetExtensionAfterPeriod().ToScintillaLanguage();
            }
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
                ValidateInputs(nameBox.Text);
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
            ValidateInputs(nameBox.Text);
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
            ValidateInputs(nameBox.Text);
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls((ResourceValueType)typeBox.SelectedIndex);

            if (_parentName is not null)
            {
                nameBox.Text = $"{_parentName}/NewResource";
            }
        }

        private void pathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInputs(nameBox.Text);
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

            ValidateInputs(nameBox.Text);
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
    }
}
