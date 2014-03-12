using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Entities.DeviceOriented;
using RVI_VSS.ViewModels;
using VideoCellLayoutControl;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Infrastructure.Common.Video.RVI_VSS
{
	/// <summary>
	/// Логика взаимодействия для CellPlayerWrap.xaml
	/// </summary>

	[ContentProperty("VideoCell")]
	public partial class CellPlayerWrap : UserControl
	{

		public VideoCell VideoCell
		{
			get { return (VideoCell)GetValue(VideoCellProperty); }
			set { SetValue(VideoCellProperty, value); }
		}

		// Using a DependencyProperty as the backing store for VideoCell.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoCellProperty =
			DependencyProperty.Register("VideoCell", typeof(VideoCell), typeof(CellPlayerWrap), new PropertyMetadata(null, PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var wrap = dependencyObject as CellPlayerWrap;
			if (wrap != null)
			{
				VideoCellPanel.SetCell(wrap, wrap.VideoCell.CellNumber);
				//пробросить WinForms контролу
				wrap.FormsPlayer.VideoCell = wrap.VideoCell;
			}
		}

		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public void InitializeCamera(EventHandler fullScreenSizeNewDelegate)
		{
			//var hwnd = FormsPlayer.Handle;
			var perimeter = SystemPerimeter.Instance;

			var deviceSearchInfo = new DeviceSearchInfo("172.16.2.36", 37777);

			var device = perimeter.AddDevice(deviceSearchInfo);

			device.Authorize();

			var firstChannel = device.Channels.First(channel => channel.ChannelNumber == 0);
			var videoCell = new VideoCell
                {
					Channel = new ChannelViewModel(firstChannel)
                };
			FormsPlayer.VideoCell = videoCell;
			FormsPlayer.MouseDoubleClick += (s,e) => fullScreenSizeNewDelegate (this, null);
		}
	}
}

