using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using Infrastructure.Common;
using System.Threading;

namespace FiresecService
{
	partial class FiresecNTService : ServiceBase
	{
		private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";
		private StreamWriter _file;
		private System.Timers.Timer _timer;

		public FiresecNTService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			base.OnStart(args);
			_file = new StreamWriter(new FileStream("FiresecNTService.log", System.IO.FileMode.OpenOrCreate));
			_file.WriteLine("FiresecNTService стартовал");
			try
			{
				using (new DoubleLaunchLocker(SignalId, WaitId, true))
					Bootstrapper.Run();
			}
			catch (Exception e)
			{
				_file.WriteLine(e.Message);
			}
			finally
			{
				_file.Flush();
			}
			_timer = new System.Timers.Timer();
			_timer.Enabled = true;
			_timer.Interval = 100000;
			_timer.Elapsed +=  new System.Timers.ElapsedEventHandler(timer_Elapsed);
			_timer.AutoReset = true;
			_timer.Start();
		}

		protected override void OnStop()
		{
			base.OnStop();
			//var processes = Process.GetProcesses();
			//foreach (var process in processes)
			//{
			//    if (process.ProcessName == "FiresecService")
			//    {
			//        process.Kill();
			//    }
			//}
		}
		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_file.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
			_file.Flush();
}
	}
}
