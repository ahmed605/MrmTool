using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace MrmTool.Behaviors
{
    internal partial class PointerExitedBehavior : Trigger<UIElement>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject is { } element)
            {
                element.PointerExited += OnPointerExited;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject is { } element)
            {
                element.PointerExited -= OnPointerExited;
            }
        }

        private void OnPointerExited(object sender, RoutedEventArgs args)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, args);
        }
    }
}
