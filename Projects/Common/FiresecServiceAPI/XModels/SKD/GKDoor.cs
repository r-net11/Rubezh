using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Точка доступа ГК
	/// </summary>
	public class GKDoor : GKBase, IPlanPresentable
	{
		public GKDoor()
		{
			PlanElementUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public GKDevice EnterDevice { get; set; }
		[XmlIgnore]
		public GKDevice ExitDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockControlDevice { get; set; }

		/// <summary>
		/// Тип ТД
		/// </summary>
		[DataMember]
		public GKDoorType DoorType { get; set; }

		/// <summary>
		/// Задержка
		/// </summary>
		[DataMember]
		public int Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public int Hold { get; set; }

		/// <summary>
		/// Минимальный индивидуальный уровень на вход
		/// </summary>
		[DataMember]
		public int EnterLevel { get; set; }

		/// <summary>
		/// Идентификатор устройства на вход
		/// </summary>
		[DataMember]
		public Guid EnterDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства на выход
		/// </summary>
		[DataMember]
		public Guid ExitDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Замок
		/// </summary>
		[DataMember]
		public Guid LockDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Датчик контроля двери
		/// </summary>
		[DataMember]
		public Guid LockControlDeviceUID { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Door; } }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Разрещить множественную визуализация на плане
		/// </summary>
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
	}
}