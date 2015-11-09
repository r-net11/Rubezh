using Vlc.DotNet.Core.Medias;

namespace VideoModule.Views
{
	public partial class VlcControlView
	{
		public VlcControlView()
		{
			InitializeComponent();
		}

		private string _rtsp;
		public string Rtsp
		{
			get { return _rtsp; }
			set
			{
				_rtsp = value;
				//myVlcControl.Media = new LocationMedia(_rtsp);
			}
		}

		public void Start()
		{
			//myVlcControl.Play();
		}

		public void Stop()
		{
			//myVlcControl.Stop();
		}
	}
}
