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
	}
}