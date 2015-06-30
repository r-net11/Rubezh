using System;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Составная часть денвного графика
	/// </summary>
	[DataContract]
	public class GKDaySchedulePart : ModelBase
	{
		public GKDaySchedulePart()
		{
			UID = Guid.NewGuid();
		}

		/// <summary>
		/// Время начала
		/// </summary>
		[DataMember]
		public double StartMilliseconds { get; set; }

		/// <summary>
		/// Время окончания
		/// </summary>
		[DataMember]
		public double EndMilliseconds { get; set; }
	}
}