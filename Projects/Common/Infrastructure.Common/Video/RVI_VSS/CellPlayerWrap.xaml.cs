using System.Collections.Generic;
using Entities.DeviceOriented;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class CellPlayerWrap
	{
		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public List<IChannel> Connect(string ipAddress, int port, string login = "admin", string password = "admin")
		{
			return FormsPlayer.Connect(ipAddress, port, login, password);
		}

		public void Start(int channelNumber)
		{
			FormsPlayer.Start(channelNumber);
		}

		public void Start(PlayBackDeviceRecord record)
		{
			FormsPlayer.Start(record);
		}

		public bool Pause(PlayBackDeviceRecord record, bool pausePlayBack)
		{
			return FormsPlayer.Pause(record, pausePlayBack);
		}

		public void Stop()
		{
			FormsPlayer.Stop();
		}

		public void Stop(PlayBackDeviceRecord record)
		{
			FormsPlayer.Stop(record);
		}

		public void Fast(PlayBackDeviceRecord record)
		{
			FormsPlayer.Fast(record);
		}

		public void Slow(PlayBackDeviceRecord record)
		{
			FormsPlayer.Slow(record);
		}
	}
}