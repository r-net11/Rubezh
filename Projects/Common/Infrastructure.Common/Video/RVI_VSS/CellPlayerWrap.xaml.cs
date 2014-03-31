using System.Collections.Generic;
using System.Windows.Documents;
using Entities.DeviceOriented;
using FiresecAPI.Models;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class CellPlayerWrap
	{
		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public List<Channel> Connect(string ipAddress, int port)
		{
			return FormsPlayer.Connect(ipAddress, port);
		}

		public void Start(int channelNumber)
		{
			FormsPlayer.Start(channelNumber);
		}

		public void Start(PlayBackDeviceRecord record)
		{
			FormsPlayer.Start(record);
		}

		public void Stop()
		{
			FormsPlayer.Stop();
		}
	}
}