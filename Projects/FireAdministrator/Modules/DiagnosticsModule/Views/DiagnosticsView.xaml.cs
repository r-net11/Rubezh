using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FiresecClient;

namespace DiagnosticsModule.Views
{
	public partial class DiagnosticsView : UserControl
	{
		public DiagnosticsView()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			MediaElement.Source = new Uri("rtsp://admin:admin@172.16.2.23:554/cam/realmonitor?channel=1&subtype=0");
			MediaElement.LoadedBehavior = MediaState.Manual;
			MediaElement.UnloadedBehavior = MediaState.Manual;
			MediaElement.Play();
		}
	}
}