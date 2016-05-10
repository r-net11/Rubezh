using System;
using System.Linq;
using System.ServiceModel;
using Common;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Service;
using RubezhAPI;
using Infrastructure.Common;

namespace FiresecService.Report
{
	public class ReportServiceManager
	{
		const string FilterNamespace = @"RubezhAPI.SKD.ReportFilters";
		static ReportServiceManager _instance = new ReportServiceManager();
		ServiceHost _serviceHost;
		ServiceHost _dataServiceHost = null;
		public static bool IsRunning { get; private set; }

		public static bool Run()
		{
			if (!IsRunning)
			{
				lock (_instance)
				{
					if (!IsRunning)
					{
						try
						{
							_instance.RegisterFilters();
							_instance.Open();
							IsRunning = true;
						}
						catch (Exception e)
						{
							Logger.Error(e, "Исключение при вызове ReportServiceManager.Open");
							return false;
						}
					}
				}
			}
			return true;
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
						if (_instance._dataServiceHost != null)
						{
							_instance._dataServiceHost.Close();
							_instance._dataServiceHost = null;
						}
						IsRunning = false;
					}
		}

		void Open()
		{
			Close();
			var binding = BindingHelper.CreateBindingFromAddress(ConnectionSettingsManager.ReportServerAddress);
			_serviceHost = new ServiceHost(typeof(FiresecReportService));
			_serviceHost.AddServiceEndpoint(typeof(IReportService), binding, ConnectionSettingsManager.ReportServerAddress);
			_serviceHost.Open();

			var reportDataBinding = BindingHelper.CreateBindingFromAddress(ConnectionSettingsManager.ReportDataServerAddress);
			_dataServiceHost = new ServiceHost(typeof(FiresecReportDataService));
			_dataServiceHost.AddServiceEndpoint(typeof(IReportDataService), reportDataBinding, ConnectionSettingsManager.ReportDataServerAddress);
			_dataServiceHost.Open();
		}
		void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
			if (_dataServiceHost != null && _dataServiceHost.State != CommunicationState.Closed && _dataServiceHost.State != CommunicationState.Closing)
				_dataServiceHost.Close();
		}
		void RegisterFilters()
		{
			ServiceKnownTypeProvider.Register(typeof(IFiresecService).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Namespace == FilterNamespace));
		}
	}
}