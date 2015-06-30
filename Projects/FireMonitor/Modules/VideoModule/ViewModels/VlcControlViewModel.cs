using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;
using System.Threading;

namespace VideoModule.ViewModels
{
	public class VlcControlViewModel : BaseViewModel
	{
		string _rviRTSP;
		public string RviRTSP
		{
			get { return _rviRTSP; }
			set
			{
				_rviRTSP = value;
				_vlcControl = new VlcControl { Media = new LocationMedia(_rviRTSP) };
				_vlcControl.PositionChanged -= VlcControlOnPositionChanged;
				_vlcControl.PositionChanged += VlcControlOnPositionChanged;
			}
		}

		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				if (_vlcControl == null)
					return new BitmapImage();
				return _vlcControl.VideoSource;
			}
		}

		public void Start()
		{
			try
			{
				if (_vlcControl == null ||_vlcControl.IsPlaying)
					return;
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)_vlcControl.Play);
				//_vlcControl.Play();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public void Stop()
		{
			if (_vlcControl.IsPlaying)
			{
				_vlcControl.Stop();
			}
		}

		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			if (_vlcControl != null)
				OnPropertyChanged(() => Image);
		}
	}
}
