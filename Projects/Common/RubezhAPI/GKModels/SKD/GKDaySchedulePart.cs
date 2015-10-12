using System;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
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
		public int StartMilliseconds { get; set; }

		/// <summary>
		/// Время окончания
		/// </summary>
		[DataMember]
		public int EndMilliseconds { get; set; }
	}
}