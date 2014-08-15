using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Common;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyInterval
	{
		public SKDWeeklyInterval()
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
		public ReadOnlyCollection<SKDWeeklyIntervalPart> WeeklyIntervalParts { get; set; }

		public static ReadOnlyCollection<SKDWeeklyIntervalPart> CreateParts()
		{
			var list = new List<SKDWeeklyIntervalPart>();
			for (int i = 1; i <= 7; i++)
			{
				list.Add(new SKDWeeklyIntervalPart() { No = i, TimeIntervalID = 0 });
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
			var ids = SKDManager.TimeIntervalsConfiguration.TimeIntervals.Select(item => item.ID).ToList();
			WeeklyIntervalParts.ForEach(x =>
			{
				if (!ids.Contains(x.TimeIntervalID))
					x.TimeIntervalID = 0;
			});
		}
	}
}