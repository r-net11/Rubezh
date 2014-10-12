using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Устройство МПТ
	/// </summary>
	[DataContract]
	public class GKMPTDevice
	{
		public GKMPTDevice()
		{
			MPTDeviceType = GKMPTDeviceType.Unknown;
		}

		[XmlIgnore]
		public GKDevice Device { get; set; }

		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		[DataMember]
		public Guid DeviceUID { get; set; }

		/// <summary>
		/// Тип устройства МПТ
		/// </summary>
		[DataMember]
		public GKMPTDeviceType MPTDeviceType { get; set; }

		public static List<GKDriverType> GetAvailableMPTDriverTypes(GKMPTDeviceType mptDeviceType)
		{
			var result = new List<GKDriverType>();
			switch (mptDeviceType)
			{
				case GKMPTDeviceType.DoNotEnterBoard:
				case GKMPTDeviceType.ExitBoard:
				case GKMPTDeviceType.AutomaticOffBoard:
					result.Add(GKDriverType.RSR2_OPS);
					result.Add(GKDriverType.RSR2_OPK);
					result.Add(GKDriverType.RSR2_RM_1);
					result.Add(GKDriverType.RSR2_MVK8);
					break;

				case GKMPTDeviceType.Speaker:
					result.Add(GKDriverType.RSR2_OPZ);
					result.Add(GKDriverType.RSR2_OPK);
					break;

				case GKMPTDeviceType.Door:
				case GKMPTDeviceType.HandStart:
				case GKMPTDeviceType.HandStop:
				case GKMPTDeviceType.HandAutomatic:
					result.Add(GKDriverType.RSR2_AM_1);
					break;

				case GKMPTDeviceType.Bomb:
					result.Add(GKDriverType.RSR2_RM_1);
					result.Add(GKDriverType.RSR2_MVK8);
					break;
			}
			return result;
		}
	}
}