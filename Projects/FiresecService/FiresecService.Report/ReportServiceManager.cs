using System;
using System.Linq;
using System.ServiceModel;
using Common;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Service;
using FiresecAPI;
using Infrastructure.Common;

namespace FiresecService.Report
{
	public class ReportServiceManager
	{
		private const string FilterNamespace = @"FiresecAPI.SKD.ReportFilters";
		private static ReportServiceManager _instance = new ReportServiceManager();

		public static bool IsRunning { get; private set; }
		public static void Run()
		{
			if (!IsRunning)
				lock (_instance)
					if (!IsRunning)
					{
						_instance.RegisterFilters();
						_instance.Open();
						IsRunning = true;
					}
		}
		public static void Stop()
		{
			if (IsRunning)
				lock (_instance)
					if (IsRunning)
					{
						if (_instance._serviceHost != null)
						{
							_instance._serviceHost.Close();
							_instance._serviceHost = null;
						}
						IsRunning = false;
					}
		}

		private ServiceHost _serviceHost;

		private bool Open()
		{
			try
			{
				Close();
				var binding = BindingHelper.CreateBindingFromAddress(ConnectionSettingsManager.ReportServerAddress);
				_serviceHost = new ServiceHost(typeof(FiresecReportService));
				_serviceHost.AddServiceEndpoint(typeof(IReportService), binding, ConnectionSettingsManager.ReportServerAddress);
				_serviceHost.Open();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ReportServiceManager.Open");
				return false;
			}
		}
		private void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
		}
		private void RegisterFilters()
		{
			ServiceKnownTypeProvider.Register(typeof(IFiresecService).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Namespace == FilterNamespace));
		}
	}
}