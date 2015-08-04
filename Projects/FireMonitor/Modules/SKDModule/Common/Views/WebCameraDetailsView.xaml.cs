using System;
using System.Collections.Generic;
using System.IO;
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
using WPFMediaKit.DirectShow.Interop;
using WPFMediaKit.DirectShow.Controls;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	/// <summary>
	/// Interaction logic for WebCamView.xaml
	/// </summary>
	public partial class WebCameraDetailsView : UserControl
	{
		public WebCameraDetailsView()
		{
			InitializeComponent();
		}

		public void TakeSnapshot(object o, RoutedEventArgs e)
		{

			RenderTargetBitmap rtb = new RenderTargetBitmap((int)videoCapElement.ActualWidth, (int)videoCapElement.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(videoCapElement);
			var encoder = new JpegBitmapEncoder();
			var outputFrame = BitmapFrame.Create(rtb);
			encoder.Frames.Add(outputFrame);

			using (var file = File.OpenWrite(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "webcam_tmp_img.jpg")))
			{
				encoder.Save(file);
			}
			videoCapElement.Pause();
		}

		public void ResumePreview(object o, RoutedEventArgs e)
		{
			videoCapElement.Play();
		}
		public void SavePicture(object o, RoutedEventArgs e)
		{
			if (File.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "webcam_tmp_img.jpg")))
			{
				File.Copy(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "webcam_tmp_img.jpg"), "C:\\target.jpg");
			} 
		}
	}
}

