using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyInterval
	{
		public SKDWeeklyInterval()
		{
		}

		public SKDWeeklyInterval(bool createdefault)
		{
			WeeklyIntervalParts = CreateParts();
		}

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		[XmlIgnore]
		public ReadOnlyCollection<SKDWeeklyIntervalPart> WeeklyIntervalParts { get; set; }

		public static ReadOnlyCollection<SKDWeeklyIntervalPart> CreateParts()
		{
			var list = new List<SKDWeeklyIntervalPart>();
			for (int i = 1; i <= 7; i++)
			{
				list.Add(new SKDWeeklyIntervalPart() { No = i, DayIntervalID = 0 });
			}
			return new ReadOnlyCollection<SKDWeeklyIntervalPart>(list);
		}

		public override bool Equals(object obj)
		{
			if (obj is SKDWeeklyInterval)
				return ((SKDWeeklyInterval)obj).ID == ID;
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}
		public void InvalidateDayIntervals()
		{
			var ids = SKDManager.TimeIntervalsConfiguration.DayIntervals.Select(item => item.ID).ToList();
			WeeklyIntervalParts.ForEach(x =>
			{
				if (!ids.Contains(x.DayIntervalID))
					x.DayIntervalID = 0;
			});
		}
	}
}