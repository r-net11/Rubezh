using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static bool AddMeasure(Guid deviceUID, int tariffPartNo, double value, DateTime dateTime)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var measure = CreateMeasure(deviceUID, tariffPartNo, value, dateTime);
				context.Measures.Add(measure);
				context.SaveChanges();
			}
			return true;
		}

		public static Measure CreateMeasure(Guid deviceUID, int tariffPartNo, double value, DateTime dateTime)
		{
			var measure = new Measure();
			measure.DeviceUID = deviceUID;
			measure.TariffPartNo = tariffPartNo;
			measure.Value = value;
			measure.DateTime = dateTime;
			return measure;
		}

		public static bool AddRangeMeasures(List<Measure> measures)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Measures.AddRange(measures);
				context.SaveChanges();
			}
			return true;
		}

		public static List<Measure> GetMeasures(Guid deviceUID, DateTime startDate, DateTime endDate)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Measures.Where(x => x.DeviceUID == deviceUID && x.DateTime>=startDate && x.DateTime <= endDate).OrderBy(x=>x.DateTime).ToList();
			}
		}
	}
}
