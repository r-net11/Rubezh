using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Направление ГК
	/// </summary>
	[DataContract]
	public class GKDirection : GKBase, IPlanPresentable
	{
		public GKDirection()
		{
			DelayRegime = DelayRegime.On;
			Logic = new GKLogic();

			InputDevices = new List<GKDevice>();
			InputZones = new List<GKZone>();
			OutputDevices = new List<GKDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		public override void Update(GKDevice device)
		{
			Logic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			UnLinkObject(device);
			OnChanged();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Direction; } }

		[XmlIgnore]
		public List<GKDevice> InputDevices { get; set; }
		[XmlIgnore]
		public List<GKZone> InputZones { get; set; }
		[XmlIgnore]
		public List<GKDevice> OutputDevices { get; set; }

		/// <summary>
		/// Задержка на включение
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BDirection.png"; }
		}
	}
}