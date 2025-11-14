#nullable disable

using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;

namespace Microsoft.UI.Xaml.Controls
{
	public partial class MenuBarItemFlyout : MenuFlyout
	{
		private static readonly bool IsShouldConstrainToRootBoundsAvailable = ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.Primitives.FlyoutBase", "ShouldConstrainToRootBounds");

        internal Control m_presenter;

		public MenuBarItemFlyout() : base()
		{
			if (IsShouldConstrainToRootBoundsAvailable)
			{
				ShouldConstrainToRootBounds = false;
            }
        }

		protected override Control CreatePresenter()
		{
			m_presenter = base.CreatePresenter();
			return m_presenter;
		}
	}
}
