using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Устройство МПТ
	/// </summary>
	[DataContract]
	public class GKMPTDevice
	{
		public GKMPTDevice()
		{
			CodeReaderSettings = new GKCodeReaderSettings();
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
		/// Тип действия
		/// </summary>
		[DataMember]
		public GKGuardZoneDeviceActionType ActionType { get; set; }

		/// <summary>
		/// Настройки кодонаборника в МПТ
		/// </summary>
		[DataMember]
		public GKCodeReaderSettings CodeReaderSettings { get; set; }

		/// <summary>
		/// Тип устройства МПТ
		/// </summary>
		/// 
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
					result.Add(GKDriverType.RSR2_SCOPA);
					result.Add(GKDriverType.RSR2_RM_1);
					result.Add(GKDriverType.RSR2_MVK8);
					break;

				case GKMPTDeviceType.Speaker:
					result.Add(GKDriverType.RSR2_OPZ);
					result.Add(GKDriverType.RSR2_OPK);
					result.Add(GKDriverType.RSR2_ZOV);
					result.Add(GKDriverType.RSR2_RM_1);
					result.Add(GKDriverType.RSR2_MVK8);
					break;

				case GKMPTDeviceType.HandStart:
				case GKMPTDeviceType.HandStop:
				case GKMPTDeviceType.HandAutomaticOn:
				case GKMPTDeviceType.HandAutomaticOff:
					result.Add(GKDriverType.RSR2_AM_1);
					result.Add(GKDriverType.RSR2_CardReader);
					result.Add(GKDriverType.RSR2_CodeReader);
					result.Add(GKDriverType.RSR2_CodeCardReader);
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