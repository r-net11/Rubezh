using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class MPTDevice
	{
		public MPTDevice()
		{

		}

		public XDevice Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public MPTDeviceType MPTDeviceType { get; set; }

		public static List<MPTDeviceType> GetAvailableMPTDeviceTypes(XDriverType driverType)
		{
			var result = new List<MPTDeviceType>();
			switch (driverType)
			{
				case XDriverType.RSR2_MVK8:
					result.Add(MPTDeviceType.Bomb);
					break;

				case XDriverType.RSR2_Table:
					result.Add(MPTDeviceType.DoNotEnterBoard);
					result.Add(MPTDeviceType.ExitBoard);
					result.Add(MPTDeviceType.AutomaticOffBoard);
					break;

				case XDriverType.RSR2_Siren:
					result.Add(MPTDeviceType.Speaker);
					break;

				case XDriverType.RSR2_AM_1:
					result.Add(MPTDeviceType.Door);
					result.Add(MPTDeviceType.HandAutomatic);
					result.Add(MPTDeviceType.HandStart);
					result.Add(MPTDeviceType.HandStop);
					break;

				default:
					result.Add(MPTDeviceType.Unknown);
					break;
			}
			return result;
		}
	}
}