using SKDModule.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SKDModule.Views
{
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
			
			using (var memoryStream = new MemoryStream())
			{
				encoder.Save(memoryStream);
				var webCameraDetailsViewModel = DataContext as WebCameraDetailsViewModel;
				if (webCameraDetailsViewModel != null)
				{
					webCameraDetailsViewModel.Data = memoryStream.ToArray();
				}
			}
		}
	}
}