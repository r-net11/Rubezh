using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Common;
using FiresecService;

namespace StrazhService.WS
{
	public partial class StrazhWindowsService : ServiceBase
	{
		private ServiceStarter _serviceStarter;

		public StrazhWindowsService()
		{
			InitializeComponent();
			CanStop = true;
		}

		protected override void OnStart(string[] args)
		{
			Logger.Info("Запускаем службу 'StrazhService'");
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Logger.Info(string.Format("Текущий рабочий каталог службы '{0}'", Environment.CurrentDirectory));
			DoStart();
		}

		protected override void OnStop()
		{
			Logger.Info("Останавливаем службу 'StrazhService'");
			DoStop();
		}

		public void DoStart()
		{
			_serviceStarter = new ServiceStarter();
			Task.Factory.StartNew(_serviceStarter.Start);
		}

		public void DoStop()
		{
			_serviceStarter.Stop();
		}
	}
}
