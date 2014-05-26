using System.Collections.Generic;
using Entities.DeviceOriented;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public static class RviVssHelper
	{
		static RviVssHelper()
		{
			Devices = new List<IDevice>();
		}
		public static List<IDevice> Devices { get; private set; }
	}
}
