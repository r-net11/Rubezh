using System;
using System.Runtime.Serialization;
using FiresecAPI.Journal;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Запись журнала событий
	/// </summary>
	[DataContract]
	public class GKJournalItem
	{
		public GKJournalItem()
		{
			DeviceDateTime = DateTime.Now;
			SystemDateTime = DateTime.Now;
			ObjectStateClass = XStateClass.Norm;
			JournalObjectType = GKJournalObjectType.System;
		}

		/// <summary>
		/// Тип объекта записи ГК
		/// </summary>
		[DataMember]
		public GKJournalObjectType JournalObjectType { get; set; }
		
		/// <summary>
		/// Дата/время устройства
		/// </summary>
		[DataMember]
		public DateTime DeviceDateTime { get; set; }
		
		/// <summary>
		/// Дата/время системы
		/// </summary>
		[DataMember]
		public DateTime SystemDateTime { get; set; }
		
		/// <summary>
		/// Номер записи в ГК
		/// </summary>
		[DataMember]
		public int? GKJournalRecordNo { get; set; }

		/// <summary>
		/// Тип события
		/// </summary>
		[DataMember]
		public JournalEventNameType JournalEventNameType { get; set; }

		/// <summary>
		/// Тип уточнения
		/// </summary>
		[DataMember]
		public JournalEventDescriptionType JournalEventDescriptionType { get; set; }

		/// <summary>
		/// Текст названия
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Текст уточнения
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Класс состояния
		/// </summary>
		[DataMember]
		public XStateClass StateClass { get; set; }

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
		/// Состояние обхекта
		/// </summary>
		[DataMember]
		public int ObjectState { get; set; }

		/// <summary>
		/// Номер объекта на ГК
		/// </summary>
		[DataMember]
		public ushort GKObjectNo { get; set; }

		/// <summary>
		/// IP-адрес ГК
		/// </summary>
		[DataMember]
		public string GKIpAddress { get; set; }

		/// <summary>
		/// Состояние объекта
		/// </summary>
		[DataMember]
		public XStateClass ObjectStateClass { get; set; }

		/// <summary>
		/// Адрес контроллера
		/// </summary>
		[DataMember]
		public ushort ControllerAddress { get; set; }

		/// <summary>
		/// Дополнительное примечание
		/// </summary>
		[DataMember]
		public string AdditionalDescription { get; set; }

		/// <summary>
		/// Тип дескриптора
		/// </summary>
		[DataMember]
		public ushort DescriptorType { get; set; }

		/// <summary>
		/// Адрес дескриптора
		/// </summary>
		[DataMember]
		public ushort DescriptorAddress { get; set; }

		/// <summary>
		/// Имя пользователя
		/// </summary>
		[DataMember]
		public string UserName { get; set; }

		/// <summary>
		/// Тип подсистемы ГК
		/// </summary>
		[DataMember]
		public GKSubsystemType SubsystemType { get; set; }
	}
}