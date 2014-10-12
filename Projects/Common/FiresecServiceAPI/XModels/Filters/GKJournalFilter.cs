using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Фильтр журнала событий ГК
	/// </summary>
	[DataContract]
	public class GKJournalFilter
	{
		public GKJournalFilter()
		{
			LastRecordsCount = 100;
			StateClasses = new List<XStateClass>();
			EventNames = new List<string>();
		}

		/// <summary>
		/// Название
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Количество последних зиписей
		/// </summary>
		[DataMember]
		public int LastRecordsCount { get; set; }

		/// <summary>
		/// Классы состояний
		/// </summary>
		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		/// <summary>
		/// Названия событий
		/// </summary>
		[DataMember]
		public List<string> EventNames { get; set; }
	}
}