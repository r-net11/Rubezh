using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Infrastructure.Common.Windows.Views
{
	public partial class ShellView : UserControl
	{
		public static readonly DependencyProperty IsRightPanelFocusedProperty = DependencyProperty.Register("IsRightPanelFocused", typeof(bool), typeof(ShellView), new FrameworkPropertyMetadata(false));
		public bool IsRightPanelFocused
		{
			get { return (bool)GetValue(IsRightPanelFocusedProperty); }
			private set { SetValue(IsRightPanelFocusedProperty, value); }
		}

		public ShellView()
		{
			InitializeComponent();
			SetBinding(IsRightPanelFocusedProperty, new Binding("IsRightPanelFocused") { Mode = BindingMode.OneWayToSource });
		}

		private void LeftContent_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			IsRightPanelFocused = false;
		}
		private void RightContent_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			IsRightPanelFocused = true;
		}

		private void LeftContent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!LeftContent.IsKeyboardFocusWithin)
				LeftContent.Focus();
		}
		private void RightContent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!RightContent.IsKeyboardFocusWithin)
				RightContent.Focus();
		}
	}
}