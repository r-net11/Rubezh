using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Controls.Ribbon.Views
{
	public class RibbonMenuItemView : TabItem
	{
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(RibbonMenuItemView));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RibbonMenuItemView));
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RibbonMenuItemView));
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RibbonMenuItemView));

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
		}
		private object CoerceValueCallback(DependencyObject obj, object val)
		{
			var item = obj as RibbonMenuItemView;
			var result = _callback == null ? val : _callback(obj, val);
			if (item == null)
				return result;
			return (bool)result && item.HasContent;
		}

		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
	}
}
