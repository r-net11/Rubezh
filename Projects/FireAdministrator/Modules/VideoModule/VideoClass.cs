using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace VideoPlayerTest
{
	public class VideoClass : BaseViewModel
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				return _vlcControl.VideoSource;
			}
		}
		public VideoClass()
		{
			_vlcControl = new VlcControl { Media = new LocationMedia("rtsp://admin:admin@172.16.7.88:554/cam/realmonitor?channel=1&subtype=0") };
			_vlcControl.PositionChanged += VlcControlOnPositionChanged;
		}
		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged("Image");
		}
		public void Play()
		{
			_vlcControl.Play();
		}
	}
}