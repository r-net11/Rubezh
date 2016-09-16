using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLicenseServiceClient.ServiceReference;

namespace TestLicenseServiceClient
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var client = new LicenseServiceClient())
			{
				var result = client.GetDemoLicense("7RAT88-I83LVY-P240X-A1IJRG");

				File.WriteAllText("MyFileLicense.lic", result);
				Console.WriteLine(result);
			}

			Console.ReadLine();
		}
	}
}
