using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiresecService
{
	public static class AutomationProcessor
	{
		public static void Start()
		{
			var thread = new Thread(OnRun);
			thread.Start();
		}

		static void OnRun()
		{
			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}
	}
}