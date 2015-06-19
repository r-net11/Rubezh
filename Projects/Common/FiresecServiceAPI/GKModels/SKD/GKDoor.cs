using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
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
			OpenRegimeLogic = new GKLogic();
			NormRegimeLogic = new GKLogic();
			CloseRegimeLogic = new GKLogic();
			Delay = 2;
			Hold = 2;
		}

		bool _isLogicOnKau;
		[XmlIgnore]
		public override bool IsLogicOnKau
		{
			get { return _isLogicOnKau; }
			set
			{
				_isLogicOnKau = value;
			}
		}

		[XmlIgnore]
		public override string PresentationName
		{
			get
			{
				var presentationName = No + "." + Name;
				return presentationName;
			}
		}
		[XmlIgnore]
		public GKDevice EnterDevice { get; set; }
		[XmlIgnore]
		public GKDevice ExitDevice { get; set; }
		[XmlIgnore]
		public GKDevice EnterButton { get; set; }
		[XmlIgnore]
		public GKDevice ExitButton { get; set; }
		[XmlIgnore]
		public GKDevice LockDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockDeviceExit { get; set; }
		[XmlIgnore]
		public GKDevice LockControlDevice { get; set; }
		[XmlIgnore]
		public GKDevice LockControlDeviceExit { get; set; }

		/// <summary>
		/// Тип ТД
		/// </summary>
		[DataMember]
		public GKDoorType DoorType { get; set; }

		/// <summary>
		/// Режим запрета повторного прохода
		/// </summary>
		[DataMember]
		public bool AntipassbackOn { get; set; }

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
		/// Идентификатор кнопки на вход
		/// </summary>
		[DataMember]
		public Guid EnterButtonUID { get; set; }

		/// <summary>
		/// Идентификатор кнопки на выход
		/// </summary>
		[DataMember]
		public Guid ExitButtonUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Замок
		/// </summary>
		[DataMember]
		public Guid LockDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Замок на выход
		/// </summary>
		[DataMember]
		public Guid LockDeviceExitUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Датчик контроля двери
		/// </summary>
		[DataMember]
		public Guid LockControlDeviceUID { get; set; }

		/// <summary>
		/// Идентификатор устройства Датчик контроля двери на выход
		/// </summary>
		[DataMember]
		public Guid LockControlDeviceExitUID { get; set; }

		/// <summary>
		/// Идентификатор зоны входа
		/// </summary>
		[DataMember]
		public Guid EnterZoneUID { get; set; }

		/// <summary>
		/// Идентификатор зоны выхода
		/// </summary>
		[DataMember]
		public Guid ExitZoneUID { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Всегда Открыто
		/// </summary>
		[DataMember]
		public GKLogic OpenRegimeLogic { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Норма
		/// </summary>
		[DataMember]
		public GKLogic NormRegimeLogic { get; set; }

		/// <summary>
		/// Логика перевода ТД в состояние Всегда Закрыто
		/// </summary>
		[DataMember]
		public GKLogic CloseRegimeLogic { get; set; }

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