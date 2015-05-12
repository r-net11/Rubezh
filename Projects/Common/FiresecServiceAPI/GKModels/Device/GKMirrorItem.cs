using System;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Отражение объекта ГК
	/// </summary>
	[DataContract]
	public class GKMirrorItem
	{
		/// <summary>
		/// Идентификатор объекта ГК
		/// </summary>
		[DataMember]
		public Guid UID { get; set; }
	}
}