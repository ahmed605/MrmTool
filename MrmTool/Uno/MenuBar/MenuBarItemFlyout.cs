#nullable disable

using Common;
using Windows.UI.Xaml.Controls;

namespace Microsoft.UI.Xaml.Controls
{
	public partial class MenuBarItemFlyout : MenuFlyout
	{
        internal Control m_presenter;

		public MenuBarItemFlyout() : base()
		{
			if (Features.IsShouldConstrainToRootBoundsAvailable)
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
