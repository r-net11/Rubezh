using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Охранная зона ГК
	/// </summary>
	[DataContract]
	public class GKGuardZone : GKBase, IPlanPresentable
	{
		public GKGuardZone()
		{
			PlanElementUIDs = new List<Guid>();
			GuardZoneDevices = new List<GKGuardZoneDevice>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.GuardZone; } }

		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Метод ввода
		/// </summary>
		[DataMember]
		public GKGuardZoneEnterMethod GuardZoneEnterMethod { get; set; }

		/// <summary>
		/// Минимальный уровень на постановку
		/// </summary>
		[DataMember]
		public int SetGuardLevel { get; set; }

		/// <summary>
		/// Минимальный уровень на снятие
		/// </summary>
		[DataMember]
		public int ResetGuardLevel { get; set; }

		/// <summary>
		/// Минимальный уровень на вызов тревоги
		/// </summary>
		[DataMember]
		public int SetAlarmLevel { get; set; }

		/// <summary>
		/// Задержка на постановку
		/// </summary>
		[DataMember]
		public int Delay { get; set; }

		/// <summary>
		/// Задержка на снятие
		/// </summary>
		[DataMember]
		public int ResetDelay { get; set; }

		/// <summary>
		/// Задержка на вызов тревоги
		/// </summary>
		[DataMember]
		public int AlarmDelay { get; set; }

		/// <summary>
		/// Идентификаторы устройств
		/// </summary>
		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		/// <summary>
		/// Устройства охранной зоны
		/// </summary>
		[DataMember]
		public List<GKGuardZoneDevice> GuardZoneDevices { get; set; }
	}
}