using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Модуль пожаротушения ГК
	/// </summary>
	[DataContract]
	public class GKMPT : GKBase
	{
		public GKMPT()
		{
			StartLogic = new GKLogic();
			MPTDevices = new List<GKMPTDevice>();
			Delay = 10;
			Devices = new List<GKDevice>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Время задержки на включение
		/// </summary>
		[DataMember]
		public int Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public int Hold { get; set; }

		/// <summary>
		/// Режим после окончания удержания
		/// </summary>
		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Логика включения
		/// </summary>
		[DataMember]
		public GKLogic StartLogic { get; set; }

		/// <summary>
		/// Устройства МПТ
		/// </summary>
		[DataMember]
		public List<GKMPTDevice> MPTDevices { get; set; }

		/// <summary>
		/// Отключать автоматику при срабатывании датчика двери-окна
		/// </summary>
		[DataMember]
		public bool UseDoorAutomatic { get; set; }

		/// <summary>
		/// Останавливать при срабатывании датчика двери-окна
		/// </summary>
		[DataMember]
		public bool UseDoorStop { get; set; }

		/// <summary>
		/// Отключать автоматику при неисправности любого устройства
		/// </summary>
		[DataMember]
		public bool UseFailureAutomatic { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.MPT; } }

		[XmlIgnore]
		public override string PresentationName
		{
			get { return "MПТ" + "." + No + "." + Name; }
		}
	}
}