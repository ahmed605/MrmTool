using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace MrmTool.Behaviors
{
    internal partial class PointerEnteredBehavior : Trigger<UIElement>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject is { } element)
            {
                element.PointerEntered += OnPointerEntered;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject is { } element)
            {
                element.PointerEntered -= OnPointerEntered;
            }
        }

        private void OnPointerEntered(object sender, RoutedEventArgs args)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, args);
        }
    }
}
