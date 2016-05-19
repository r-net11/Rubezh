using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Infrastructure.Common;

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
					var isRunning = RegistrySettingsHelper.GetBool("RubezhMonitor.IsRunning");
					var processes = Process.GetProcessesByName("RubezhMonitor");
					var processes2 = Process.GetProcessesByName("RubezhMonitor.vshost");

					if (isRunning && processes.Count() == 0)
					{
						var RubezhMonitorpath = (string)RegistrySettingsHelper.GetString("RubezhMonitorPath");
						if (!String.IsNullOrEmpty(RubezhMonitorpath))
						{
							RegistrySettingsHelper.SetBool("isAutoConnect", true);
							Process.Start(RubezhMonitorpath);
						}
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				if (StopLifetimeEvent.WaitOne(TimeSpan.FromMinutes(1)))
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
				LifetimeThread.Name = "Revisor Lifetime";
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