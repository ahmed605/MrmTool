using Windows.Storage;
using Windows.Storage.Pickers;

namespace MrmTool.Common;

internal static class OpenFileHelper
{
    internal static bool IsPri(string path)
    {
        return Path.GetExtension(path).Equals(".pri", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsXbf(string path)
    {
        return Path.GetExtension(path).Equals(".xbf", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsSupported(string path)
    {
        return IsPri(path) || IsXbf(path);
    }

    internal static async Task<StorageFile?> PickAsync()
    {
        FileOpenPicker picker = new();
        picker.FileTypeFilter.Add(".pri");
        picker.FileTypeFilter.Add(".xbf");
        picker.CommitButtonText = "Open";
        picker.Initialize();
        return await picker.PickSingleFileAsync();
    }
}
