using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SKDModule.PassCardDesigner.Views
{
	/// <summary>
	/// Interaction logic for ImageViewerControl.xaml
	/// </summary>
	public partial class ImageViewerControl : UserControl
	{
		public static DependencyProperty IsFrontProperty = DependencyProperty.Register("IsFront",
			typeof (bool), typeof (ImageViewerControl));


		public bool IsFront
		{
			get { return (bool)GetValue(IsFrontProperty); }
			set { SetValue(IsFrontProperty, value); }
		}

		public ImageViewerControl()
		{
			InitializeComponent();
		}
	}
}
