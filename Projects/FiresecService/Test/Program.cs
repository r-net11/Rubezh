using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiresecService.ReportPresenter;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ReadLine();
			ReportDataClient c = new ReportDataClient();
			var result = c.GetDevicesReport();
		}
	}
}
