using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagnosticsModule.Views
{
	public partial class DiagnosticsView : UserControl
	{
		public DiagnosticsView()
		{
			InitializeComponent();
		}
		public void TakeSnapshot(object o, RoutedEventArgs e)
		{
		
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)videoCapElement.ActualWidth, (int)videoCapElement.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(videoCapElement);
			var encoder = new PngBitmapEncoder();
			var outputFrame = BitmapFrame.Create(rtb);
			encoder.Frames.Add(outputFrame);

			using (var file = File.OpenWrite("C:\\TestImage.png"))
			{
				encoder.Save(file);
			}
		}
		
	}
}