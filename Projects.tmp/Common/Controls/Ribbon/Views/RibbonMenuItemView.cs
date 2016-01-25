using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Common;

namespace Controls.Ribbon.Views
{
	public class RibbonMenuItemView : TabItem
	{
		private CoerceValueCallback _callback;
		public RibbonMenuItemView()
		{
			DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(RibbonMenuItemView.IsSelectedProperty, typeof(RibbonMenuItemView));
			if (dpd != null)
			{
				_callback = dpd.DesignerCoerceValueCallback;
				dpd.DesignerCoerceValueCallback = new CoerceValueCallback(CoerceValueCallback);
				dpd.AddValueChanged(this, (s, e) =>
				{
					var item = s as RibbonMenuItemView;
					if (item != null && item.IsSelected && !item.HasContent)
						item.IsSelected = false;
				});
			}
			AddHandler(Button.ClickEvent, (RoutedEventHandler)Click);
		}

		private void Click(object sender, RoutedEventArgs e)
		{
			if (e.Source == this)
			{
				var button = e.OriginalSource as System.Windows.Controls.Primitives.ToggleButton;
				if (button != null && button.Command != null && button.Command.CanExecute(button.CommandParameter))
				{
					var popup = LogicalTreeHelper.GetParent(VisualHelper.GetRoot(this)) as RibbonPopup;
					if (popup != null)
						popup.Close();
				}
			}
		}
		private object CoerceValueCallback(DependencyObject obj, object val)
		{
			var item = obj as RibbonMenuItemView;
			var result = _callback == null ? val : _callback(obj, val);
			if (item == null)
				return result;
			return (bool)result && item.HasContent;
		}
	}
}
