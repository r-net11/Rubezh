using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonButton : ContentControl
	{
		public static readonly DependencyProperty IsOpenedProperty = DependencyProperty.Register("IsOpened", typeof(bool), typeof(RibbonButton), new UIPropertyMetadata(false));
		public bool IsOpened
		{
			get { return (bool)GetValue(IsOpenedProperty); }
			set { SetValue(IsOpenedProperty, value); }
		}
	}
}
