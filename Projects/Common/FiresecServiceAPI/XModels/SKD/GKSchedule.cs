using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// График работ ГК
	/// </summary>
	[DataContract]
	public class GKSchedule : ModelBase
	{
		public GKSchedule()
		{
			DayIntervalParts = new List<GKIntervalPart>();
		}

		/// <summary>
		/// Список составных частей
		/// </summary>
		[DataMember]
		public List<GKIntervalPart> DayIntervalParts { get; set; }
	}
}