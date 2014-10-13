using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Составная часть графика работ
	/// </summary>
	[DataContract]
	public class GKIntervalPart : ModelBase
	{
		public GKIntervalPart()
		{
			BeginTime = new TimeSpan(0, 0, 0);
			EndTime = new TimeSpan(0, 0, 0);
		}

		/// <summary>
		/// Идентификатор
		/// </summary>
		[DataMember]
		public Guid DayIntervalUID { get; set; }

		/// <summary>
		/// Время начала
		/// </summary>
		[DataMember]
		public TimeSpan BeginTime { get; set; }

		/// <summary>
		/// Время окончания
		/// </summary>
		[DataMember]
		public TimeSpan EndTime { get; set; }
	}
}