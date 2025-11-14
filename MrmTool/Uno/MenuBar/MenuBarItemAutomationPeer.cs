#nullable disable

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation.Peers;

namespace Microsoft.UI.Xaml.Automation.Peers
{
	public partial class MenuBarItemAutomationPeer : FrameworkElementAutomationPeer
	{
		public MenuBarItemAutomationPeer(Controls.MenuBarItem owner) : base(owner)
		{
		}
	}
}
