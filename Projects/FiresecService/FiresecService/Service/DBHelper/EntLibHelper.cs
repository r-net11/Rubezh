using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using StrazhDAL;
using System;
using System.Data;
using System.Diagnostics;

namespace FiresecService
{
	public static class EntLibHelper
	{
		public static void EntLibTest()
		{
			string myConnectionString = @"Database=SKD;Server=(local)\SQLEXPRESS;Integrated Security=True;Language='English'";
			SqlDatabase sqlDatabase = new SqlDatabase(myConnectionString);
			using (IDataReader reader = sqlDatabase.ExecuteReader(CommandType.Text,
					"SELECT UID FROM Employee"))
			{
				DisplayRowValues(reader);
			}
		}

		public static void DisplayRowValues(IDataReader reader)
		{
			int i = 0;
			string s;
			while (reader.Read())
			{
				i++;
				s = string.Format("{0} {1}", reader[0].ToString(), i);
			}
		}

		public static void Test()
		{
			Trace.WriteLine("EntLib");
			var total1 = new TimeSpan();
			for (int i = 0; i < 10; i++)
			{
				var stopWatch = new Stopwatch();
				stopWatch.Start();
				EntLibTest();
				stopWatch.Stop();
				Trace.WriteLine(stopWatch.Elapsed);
				total1 += stopWatch.Elapsed;
			}
			total1 = new TimeSpan(total1.Ticks / 10);

			Trace.WriteLine("Linq");
			var total2 = new TimeSpan();
			for (int i = 0; i < 10; i++)
			{
				var stopWatch = new Stopwatch();
				stopWatch.Start();
				using (var skdDatabaseService = new SKDDatabaseService())
				{
					skdDatabaseService.EmployeeTranslator.TestGet();
				}
				stopWatch.Stop();
				Trace.WriteLine(stopWatch.Elapsed);
				total2 += stopWatch.Elapsed;
			}

			Trace.WriteLine("Linq Single DatabaseService");
			var total3 = new TimeSpan();
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				for (int i = 0; i < 10; i++)
				{
					var stopWatch = new Stopwatch();
					stopWatch.Start();
					skdDatabaseService.EmployeeTranslator.TestGet();
					stopWatch.Stop();
					Trace.WriteLine(stopWatch.Elapsed);
					total3 += stopWatch.Elapsed;
				}
			}
			total3 = new TimeSpan(total3.Ticks / 10);

			Trace.WriteLine(string.Format("{0} EntLib", total1));
			Trace.WriteLine(string.Format("{0} Linq", total2));
			Trace.WriteLine(string.Format("{0} Linq Single DatabaseService", total3));
		}
	}
}