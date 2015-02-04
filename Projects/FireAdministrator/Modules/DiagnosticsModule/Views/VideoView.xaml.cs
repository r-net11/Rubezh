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

namespace DiagnosticsModule.Views
{
	public partial class VideoView : UserControl
	{
		public VideoView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(VideoView_Loaded);
		}

		void VideoView_Loaded(object sender, RoutedEventArgs e)
		{
			var source = _mediaElement.Source;
			_mediaElement.Play();
		}
	}
}