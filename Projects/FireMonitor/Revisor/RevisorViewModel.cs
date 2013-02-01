using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Infrastructure.Common;
using Microsoft.Win32;

namespace Revisor
{
	public class RevisorViewModel
	{
		public RevisorViewModel()
		{
			var path = System.Reflection.Assembly.GetExecutingAssembly();
			RegistrySettingsHelper.SetString("RevisorPath", path.Location);
			StartLifetimeThread();
			StartCommand = new RelayCommand(OnStart);
			StopCommand = new RelayCommand(OnStop);
		}
		public void Inspect()
		{
			while (true)
			{
				try
				{
					var firemonitorpath = (string)RegistrySettingsHelper.GetString("FireMonitorPath");
					var isException = RegistrySettingsHelper.GetBool("isException");

					if (!String.IsNullOrEmpty(firemonitorpath))
					{
						var processes = Process.GetProcessesByName("FireMonitor");
						var processes2 = Process.GetProcessesByName("FireMonitor.vshost");
						if (isException != null)
							if ((processes.Count() == 0) && (isException.Equals("True")))// && (processes2.Count() == 0))
							{
								RegistrySettingsHelper.SetBool("isAutoConnect", true);
								Process.Start(firemonitorpath);
							}
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				if (StopLifetimeEvent.WaitOne(20000))
					break;
			}
		}

		public RelayCommand StartCommand { get; private set; }
		public void OnStart()
		{
			StartLifetimeThread();
		}

		public RelayCommand StopCommand { get; private set; }
		public void OnStop()
		{
			LifetimeThread.Abort();
			LifetimeThread = null;
		}

		static AutoResetEvent StopLifetimeEvent;
		Thread LifetimeThread;

		void StartLifetimeThread()
		{
			if (LifetimeThread == null)
			{
				StopLifetimeEvent = new AutoResetEvent(false);
				LifetimeThread = new Thread(Inspect);
				LifetimeThread.Start();
			}
		}

		public void StopLifetimeThread()
		{
			if (StopLifetimeEvent != null)
			{
				StopLifetimeEvent.Set();
			}
			if (LifetimeThread != null)
			{
				LifetimeThread.Join(TimeSpan.FromSeconds(1));
			}
		}
	}
}