using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace MrmTool.Models
{
    internal static class Icons
    {
        static internal Lazy<BitmapImage> Unknown { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/generic.png") });

        static internal Lazy<BitmapImage> Folder { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/folder.png") });

        static internal Lazy<BitmapImage> Text { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/text.png") });

        static internal Lazy<BitmapImage> Image { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/image.png") });

        static internal Lazy<BitmapImage> Audio { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/audio.png") });

        static internal Lazy<BitmapImage> Video { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/video.png") });

        static internal Lazy<BitmapImage> Xaml { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/xaml.png") });

        static internal Lazy<BitmapImage> Font { get; } = new(() => new BitmapImage() { DecodePixelWidth = 28, DecodePixelType = DecodePixelType.Logical, UriSource = new("ms-appx:///Assets/Icons/font.png") });
    }
}
