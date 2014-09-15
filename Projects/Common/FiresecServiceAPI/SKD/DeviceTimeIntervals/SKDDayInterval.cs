using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDayInterval
	{
		public SKDDayInterval()
		{
			DayIntervalParts = new List<SKDDayIntervalPart>();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<SKDDayIntervalPart> DayIntervalParts { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is SKDDayInterval)
				return ((SKDDayInterval)obj).ID == ID;
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}
	}
}