using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Насосная станция ГК
	/// </summary>
	[DataContract]
	public class GKPumpStation : GKBase
	{
		public GKPumpStation()
		{
			Delay = 10;
			Hold = 60;
			DelayRegime = DelayRegime.Off;
			NSPumpsCount = 1;
			NSDeltaTime = 15;
			StartLogic = new GKDeviceLogic();
			StopLogic = new GKDeviceLogic();
			AutomaticOffLogic = new GKDeviceLogic();

			NSDevices = new List<GKDevice>();
			NSDeviceUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.PumpStation; } }
		[XmlIgnore]
		public List<GKDevice> NSDevices { get; set; }

		/// <summary>
		/// Время задержки
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		/// <summary>
		/// Режим после удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Количество основных насосов
		/// </summary>
		[DataMember]
		public int NSPumpsCount { get; set; }

		/// <summary>
		/// Время разновременного пуска
		/// </summary>
		[DataMember]
		public int NSDeltaTime { get; set; }

		/// <summary>
		/// Идентификаторы устройств, входящих в НС
		/// </summary>
		[DataMember]
		public List<Guid> NSDeviceUIDs { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKDeviceLogic StartLogic { get; set; }

		/// <summary>
		/// Логика выключения
		/// </summary>
		[DataMember]
		public GKDeviceLogic StopLogic { get; set; }

		/// <summary>
		/// Логика отключения автоматики
		/// </summary>
		[DataMember]
		public GKDeviceLogic AutomaticOffLogic { get; set; }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "0" + No + "." + Name; }
		}
	}
}