using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.SKD;

namespace RubezhAPI.Journal
{
	/// <summary>
	/// Запись журнала событий
	/// </summary>
	[DataContract]
	public class JournalItem : SKDModelBase
	{
		public JournalItem()
			: base()
		{
			SystemDateTime = DateTime.Now;
			JournalDetalisationItems = new List<JournalDetalisationItem>();
			JournalObjectType = JournalObjectType.None;
		}

		/// <summary>
		/// Дата, когда событие было зарегистрированно системой
		/// </summary>
		[DataMember]
		public DateTime SystemDateTime { get; set; }

		/// <summary>
		/// Дата, когда событие было зарегистрированно прибором
		/// </summary>
		[DataMember]
		public DateTime? DeviceDateTime { get; set; }

		/// <summary>
		/// Тип подсистемы
		/// </summary>
		[DataMember]
		public JournalSubsystemType JournalSubsystemType { get; set; }

		/// <summary>
		/// Тип события
		/// </summary>
		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }

		/// <summary>
		/// Тип примечания
		/// </summary>
		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionType { get; set; }

		/// <summary>
		/// Текст примечания
		/// </summary>
		[DataMember]
		public string DescriptionText { get; set; }

		/// <summary>
		/// Тип объекта
		/// </summary>
		[DataMember]
		public JournalObjectType JournalObjectType { get; set; }

		/// <summary>
		/// Идентификатор объекта
		/// </summary>
		[DataMember]
		public Guid ObjectUID { get; set; }

		/// <summary>
		/// Название объекта
		/// </summary>
		[DataMember]
		public string ObjectName { get; set; }

		/// <summary>
		/// Имя оператора
		/// </summary>
		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public int CardNo { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public Guid VideoUID { get; set; }

		[DataMember]
		public Guid CameraUID { get; set; }

		[DataMember]
		public List<JournalDetalisationItem> JournalDetalisationItems { get; set; }

		/// <summary>
		/// Определяет ImageSource на основе JournalObjectType, для устройств, задержек и ПИМ, необходимо изменить на основе ObjectUid
		/// </summary>
		/// <returns></returns>
		public static string GetImageSource(JournalObjectType journalObjectType)
		{
			switch (journalObjectType)
			{
				case JournalObjectType.GKDevice:
					return  "/Controls;component/GKIcons/RSR2_RM_1.png";
				case JournalObjectType.GKZone:
					return "/Controls;component/Images/Zone.png";
				case JournalObjectType.GKDirection:
					return "/Controls;component/Images/Blue_Direction.png";
				case JournalObjectType.GKPumpStation:
					return "/Controls;component/Images/BPumpStation.png";
				case JournalObjectType.GKMPT:
					return "/Controls;component/Images/BMPT.png";
				case JournalObjectType.GKDelay:
					return "/Controls;component/Images/Delay.png";
				case JournalObjectType.GKPim:
					return "/Controls;component/Images/Pim.png";
				case JournalObjectType.GKGuardZone:
					return "/Controls;component/Images/GuardZone.png";
				case JournalObjectType.GKSKDZone:
					return "/Controls;component/Images/SKDZone.png";
				case JournalObjectType.GKDoor:
					return "/Controls;component/Images/Door.png";
				case JournalObjectType.Camera:
					return "/Controls;component/Images/Camera.png";
				case JournalObjectType.AccessTemplate:
					return "/Controls;component/Images/AccessTemplate.png";
				case JournalObjectType.AdditionalColumn:
					return "/Controls;component/Images/AdditionalColumn.png";
				case JournalObjectType.Card:
					return "/Controls;component/Images/Card.png";
				case JournalObjectType.DayInterval:
					return "/Controls;component/Images/Interval.png";
				case JournalObjectType.Department:
					return "/Controls;component/Images/Department.png";
				case JournalObjectType.Employee:
					return "/Controls;component/Images/Employee.png";
				case JournalObjectType.GKDayShedule:
					return "/Controls;component/Images/Shedule.png";
				case JournalObjectType.GKShedule:
					return "/Controls;component/Images/Shedule.png";
				case JournalObjectType.Holiday:
					return "/Controls;component/Images/Holiday.png";
				case JournalObjectType.Organisation:
					return "/Controls;component/Images/Organisation.png";
				case JournalObjectType.PassCardTemplate:
					return "/Controls;component/Images/BPassCardDesigner.png";
				case JournalObjectType.Position:
					return "/Controls;component/Images/Position.png";
				case JournalObjectType.Schedule:
					return "/Controls;component/Images/Shedule.png";
				case JournalObjectType.ScheduleScheme:
					return "/Controls;component/Images/Month.png";
				default:
					return "/Controls;component/Images/Blank.png";
			}
		}
	}
}