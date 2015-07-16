using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// День из графика работ
	/// </summary>
	[DataContract]
	public class GKSchedulePart
	{
		/// <summary>
		/// Последовательный номер дневного графика
		/// </summary>
		[DataMember]
		public int DayNo { get; set; }

		/// <summary>
		/// Идентификатор дневного графика
		/// </summary>
		[DataMember]
		public Guid DayScheduleUID { get; set; }
	}
}