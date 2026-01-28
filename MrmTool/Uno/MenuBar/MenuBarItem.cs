#nullable disable

using Common;
using CommunityToolkit.WinUI;
using Uno.Disposables;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using WinRT;

using MenuBarItemAutomationPeer = Microsoft.UI.Xaml.Automation.Peers.MenuBarItemAutomationPeer;

namespace Microsoft.UI.Xaml.Controls
{
	[ContentProperty(Name = nameof(Items))]
	public partial class MenuBarItem : Control
	{
        private readonly SerialDisposable _registrations = new SerialDisposable();

		private MenuBar m_menuBar;
		private MenuBarItemFlyout m_flyout;
		private Button m_button;
		private bool m_isFlyoutOpen;

		private DependencyObject m_passThroughElement;

		private CompositeDisposable _activeDisposables;

		public MenuBarItem()
		{
			DefaultStyleKey = typeof(MenuBarItem);

            var observableVector = new ObservableVector<MenuFlyoutItemBase>();

			observableVector.VectorChanged += OnItemsVectorChanged;

			SetValue(ItemsProperty, observableVector);

			Loaded += MenuBarItem_Loaded;
            //DataContextChanged += MenuBarItem_DataContextChanged;
		}

        private void MenuBarItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
			OnDataContextChanged(args);
        }

        private void MenuBarItem_Loaded(object sender, RoutedEventArgs e)
		{
			SynchronizeMenuBar();
		}

		// IUIElement / IUIElementOverridesHelper
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new MenuBarItemAutomationPeer(this);
		}

        // IFramework Override
        [DynamicWindowsRuntimeCast(typeof(Button))]
        protected override void OnApplyTemplate()
		{
			m_button = GetTemplateChild("ContentButton") as Button;
			SynchronizeMenuBar();

			m_menuBar?.RequestPassThroughElement(this);

			PopulateContent();
			AttachEventHandlers();
		}

		private void SynchronizeMenuBar()
			=> m_menuBar = VisualTreeHelper.GetParent(this).FindAscendant<MenuBar>();

		internal void OnDataContextChanged(DataContextChangedEventArgs e)
		{
			SetFlyoutDataContext();
		}

		private void SetFlyoutDataContext()
		{
			// This is present to force the dataContext to be passed to the popup of the flyout since it is not directly a child in the visual tree of the flyout.
			//m_flyout?.SetValue(
			//	MenuFlyout.DataContextProperty,
			//	this.DataContext,
			//	precedence: DependencyPropertyValuePrecedences.Inheritance
			//);
		}

		private void PopulateContent()
		{
			// Create flyout
			var flyout = new MenuBarItemFlyout();

			foreach (var flyoutItem in Items)
			{
				flyout.Items.Add(flyoutItem);
			}

			flyout.Placement = FlyoutPlacementMode.Bottom;

			if (m_passThroughElement != null)
			{
				flyout.OverlayInputPassThroughElement = m_passThroughElement;
			}

			m_flyout = flyout;

			if (m_button != null)
			{
				m_button.IsAccessKeyScope = true;
				m_button.ContextFlyout = flyout;
			}

			SetFlyoutDataContext();
		}

		private void AttachEventHandlers()
		{
			_registrations.Disposable = null;

			_activeDisposables = new CompositeDisposable();

			long token1 = -1;
			long token2 = -1;

			if (m_button != null)
			{
                token1 = m_button.RegisterPropertyChangedCallback(ButtonBase.IsPressedProperty, OnVisualPropertyChanged);
                token2 = m_button.RegisterPropertyChangedCallback(ButtonBase.IsPointerOverProperty, OnVisualPropertyChanged);
			}

			if (m_flyout != null)
			{
				m_flyout.Closed += OnFlyoutClosed;
				m_flyout.Opening += OnFlyoutOpening;

				_activeDisposables.Add(() =>
				{
					m_flyout.Closed -= OnFlyoutClosed;
					m_flyout.Opening -= OnFlyoutOpening;

					if (token2 != -1)
					{
                        m_button.UnregisterPropertyChangedCallback(ButtonBase.IsPressedProperty, token1);
                        m_button.UnregisterPropertyChangedCallback(ButtonBase.IsPointerOverProperty, token2);
                    }
                });
			}

			PointerEntered += OnMenuBarItemPointerEntered;
			_activeDisposables.Add(() => PointerEntered -= OnMenuBarItemPointerEntered);

			var pointerPressHandler = new PointerEventHandler(OnMenuBarItemPointerPressed);
			AddHandler(UIElement.PointerPressedEvent, pointerPressHandler, true);
			var keyDownHandler = new KeyEventHandler(OnMenuBarItemKeyDown);
			AddHandler(UIElement.KeyDownEvent, keyDownHandler, true);

			_activeDisposables.Add(() =>
			{
				RemoveHandler(UIElement.PointerPressedEvent, pointerPressHandler);
				RemoveHandler(UIElement.KeyDownEvent, keyDownHandler);
			});

			AccessKeyInvoked += OnMenuBarItemAccessKeyInvoked;
			_activeDisposables.Add(() => AccessKeyInvoked -= OnMenuBarItemAccessKeyInvoked);

			_registrations.Disposable = _activeDisposables;
		}

		// Event Handlers
		private void OnMenuBarItemPointerEntered(object sender, PointerRoutedEventArgs args)
		{
			if (m_menuBar != null)
			{
				if (m_menuBar.IsFlyoutOpen)
				{
					ShowMenuFlyout();
				}
			}
		}

		private void OnMenuBarItemPointerPressed(object sender, PointerRoutedEventArgs args)
		{
			if (m_menuBar != null)
			{
				if (!m_menuBar.IsFlyoutOpen)
				{
					ShowMenuFlyout();
				}
			}
		}

		private void OnMenuBarItemKeyDown(object sender, KeyRoutedEventArgs args)
		{
			var key = args.Key;
			if (key == VirtualKey.Down
				|| key == VirtualKey.Enter
				|| key == VirtualKey.Space)
			{
				ShowMenuFlyout();
			}
		}

		private void OnPresenterKeyDown(object sender, KeyRoutedEventArgs args)
		{
			var key = args.Key;
			if (key == VirtualKey.Right)
			{
				if (FlowDirection == FlowDirection.RightToLeft)
				{
					OpenFlyoutFrom(FlyoutLocation.Left);
				}
				else
				{
					OpenFlyoutFrom(FlyoutLocation.Right);
				}
			}
			else if (key == VirtualKey.Left)
			{
				if (FlowDirection == FlowDirection.RightToLeft)
				{
					OpenFlyoutFrom(FlyoutLocation.Right);
				}
				else
				{
					OpenFlyoutFrom(FlyoutLocation.Left);
				}
			}
		}

		private void OnItemsVectorChanged(IObservableVector<MenuFlyoutItemBase> sender, IVectorChangedEventArgs e)
		{
			if (m_flyout != null)
			{
				var index = e.Index;
				switch (e.CollectionChange)
				{
					case CollectionChange.ItemInserted:
						m_flyout.Items.Insert((int)index, Items[(int)index]);
						break;
					case CollectionChange.ItemRemoved:
						m_flyout.Items.RemoveAt((int)index);
						break;
					default:
						break;
				}
			}
		}

		private void OnMenuBarItemAccessKeyInvoked(DependencyObject sender, AccessKeyInvokedEventArgs args)
		{
			ShowMenuFlyout();
			args.Handled = true;
		}

		// Menu Flyout actions
		internal void ShowMenuFlyout()
		{
			if (m_button != null)
			{
				var width = m_button.ActualWidth;
				var height = m_button.ActualHeight;

				if (Features.IsFlyoutShowOptionsAvailable)
				{
                    // Sets an exclusion rect over the button that generates the flyout so that even if the menu opens upwards
                    // (which is the default in touch mode) it doesn't cover the menu bar button.
                    FlyoutShowOptions options = new()
                    {
                        Position = new Point(0, height),
                        Placement = FlyoutPlacementMode.Bottom,
                        ExclusionRect = new Rect(0, 0, width, height)
                    };

                    m_flyout.ShowAt(m_button, options);
				}
				else
				{
					m_flyout.ShowAt(m_button, new Point(0, height));
				}

				if (m_flyout?.m_presenter != null)
				{
					m_flyout.m_presenter.KeyDown += OnPresenterKeyDown;

					_activeDisposables.Add(() =>
					{
						m_flyout.m_presenter.KeyDown -= OnPresenterKeyDown;
					});
				}
			}
		}

		internal void CloseMenuFlyout()
		{
			m_flyout.Hide();
		}

		void OpenFlyoutFrom(FlyoutLocation location)
		{
			if (m_menuBar != null)
			{
				int index = m_menuBar.Items.IndexOf(this);
				CloseMenuFlyout();
				if (location == FlyoutLocation.Left)
				{
					m_menuBar.Items[((index - 1) + m_menuBar.Items.Count) % m_menuBar.Items.Count].ShowMenuFlyout();
				}
				else
				{
					m_menuBar.Items[(index + 1) % m_menuBar.Items.Count].ShowMenuFlyout();
				}
			}
		}

		internal void AddPassThroughElement(DependencyObject element)
		{
			m_passThroughElement = element;
		}

		public bool IsFlyoutOpen()
		{
			return m_isFlyoutOpen;
		}

		public void Invoke()
		{
			if (IsFlyoutOpen())
			{
				CloseMenuFlyout();
			}
			else
			{
				ShowMenuFlyout();
			}
		}

		// Menu Flyout Events
		void OnFlyoutClosed(object sender, object args)
		{
			m_isFlyoutOpen = false;

			if (m_menuBar != null)
			{
				m_menuBar.IsFlyoutOpen = false;
			}

			UpdateVisualStates();
		}

		void OnFlyoutOpening(object sender, object args)
		{
			Focus(FocusState.Pointer);

			m_isFlyoutOpen = true;

			if (m_menuBar != null)
			{
				m_menuBar.IsFlyoutOpen = true;
			}

			UpdateVisualStates();
		}

		void OnVisualPropertyChanged(DependencyObject sender, DependencyProperty args)
		{
			UpdateVisualStates();
		}

		void UpdateVisualStates()
		{
			if (m_button != null)
			{
				if (m_isFlyoutOpen)
				{
					VisualStateManager.GoToState(this, "Selected", false);
				}
				else if (m_button.IsPressed)
				{
					VisualStateManager.GoToState(this, "Pressed", false);
				}
				else if (m_button.IsPointerOver)
				{
					VisualStateManager.GoToState(this, "PointerOver", false);
				}
				else
				{
					VisualStateManager.GoToState(this, "Normal", false);
				}
			}
		}
	}
}
