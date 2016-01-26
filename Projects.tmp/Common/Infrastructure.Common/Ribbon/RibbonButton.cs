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

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(RibbonButton));
		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
	}
}
