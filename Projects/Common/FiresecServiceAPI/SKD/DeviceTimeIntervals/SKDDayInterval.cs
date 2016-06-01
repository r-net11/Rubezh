using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDDayInterval : ModelBase
	{
		public SKDDayInterval()
		{
			DayIntervalParts = new List<SKDDayIntervalPart>();
		}

		[DataMember]
		public List<SKDDayIntervalPart> DayIntervalParts { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is SKDDayInterval)
				return ((SKDDayInterval)obj).UID == UID;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return No.GetHashCode();
		}
	}
}