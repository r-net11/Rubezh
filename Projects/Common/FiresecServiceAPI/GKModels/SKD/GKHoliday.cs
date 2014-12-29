using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Праздник
	/// </summary>
	[DataContract]
	public class GKHoliday : ModelBase
	{
		public GKHoliday()
		{
			Date = DateTime.Now;
		}

		/// <summary>
		/// Тип праздника
		/// </summary>
		[DataMember]
		public GKHolidayType HolidayType { get; set; }

		/// <summary>
		/// Дата
		/// </summary>
		[DataMember]
		public DateTime Date { get; set; }

		/// <summary>
		/// Дата переноса
		/// </summary>
		[DataMember]
		public DateTime? TransferDate { get; set; }

		/// <summary>
		/// Величина сокращения в часах
		/// </summary>
		[DataMember]
		public int Reduction { get; set; }
	}
}