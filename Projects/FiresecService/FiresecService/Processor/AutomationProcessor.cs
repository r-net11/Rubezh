using System;
using System.Threading;
using FiresecAPI.Models;
using FiresecService.Processor;

namespace FiresecService
{
	public static class AutomationProcessor
	{
		public static SystemConfiguration SystemConfiguration { get; private set; }

		public static void Start()
		{
			SystemConfiguration = ZipConfigurationHelper.GetSystemConfiguration();

			var thread = new Thread(OnRun);
			thread.Start();
		}

		static void OnRun()
		{
			while (true)
			{
				var shedules = SystemConfiguration.AutomationSchedules;

				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}
	}
}