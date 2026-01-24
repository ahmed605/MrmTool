using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace MrmTool.Common
{
    internal static class ScrollViewerExtensions
    {
        internal static Task WaitForZoomFactorChangeAsync(this ScrollViewer scrollViewer, float? originalZoomFactor = null)
        {
            originalZoomFactor ??= scrollViewer.ZoomFactor;

            var tcs = new TaskCompletionSource<object?>();
            void ViewChangedHandler(object? sender, ScrollViewerViewChangedEventArgs e)
            {
                if (scrollViewer.ZoomFactor != originalZoomFactor)
                {
                    scrollViewer.ViewChanged -= ViewChangedHandler;
                    tcs.SetResult(null);
                }
            }

            scrollViewer.ViewChanged += ViewChangedHandler;
            return tcs.Task;
        }
    }
}
