using Localization.FiresecService.Report.Common;
using System.Collections.Generic;
using Common;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Service;
using StrazhAPI;
using Infrastructure.Common;
using System;
using System.Linq;
using System.ServiceModel;

namespace FiresecService.Report
{
	public class ReportServiceManager
	{
		private const string FilterNamespace = @"StrazhAPI.SKD.ReportFilters";
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

		public static string Address { get; private set; }

		public static List<string> Addresses { get; private set; }

		static ReportServiceManager()
		{
			Addresses = new List<string>();
		}

		private bool Open()
		{
			Addresses.Clear();

			try
			{
				Close();
				_serviceHost = new ServiceHost(typeof(FiresecReportService));
				if (AppServerSettingsHelper.AppServerSettings.EnableRemoteConnections && UACHelper.IsAdministrator)
				{
					CreateTcpEndpoint();
				}
				CreateNetPipesEndpoint();
				_serviceHost.Open();

				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ReportServiceManager.Open");
                return false;
			}
		}

		private void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = AppServerConnectionManager.ReportServerNamedPipesUri;
				_serviceHost.AddServiceEndpoint(typeof(IReportService), BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
                Addresses.Add(String.Format(CommonResources.CreateNetPipesEndpoint_Address, netpipeAddress));
			}
			catch (Exception e)
			{
				Logger.Error(e, "ReportServiceManager.CreateNetPipesEndpoint");
			}
		}

		private void CreateTcpEndpoint()
		{
			try
			{
				var remoteAddress = AppServerConnectionManager.ReportServerTcpUri;
				_serviceHost.AddServiceEndpoint(typeof(IReportService), BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
                Addresses.Add(String.Format(CommonResources.CreateTcpEndpoint_Address, remoteAddress));
			}
			catch (Exception e)
			{
				Logger.Error(e, "ReportServiceManager.CreateTcpEndpoint");
			}
		}

		private void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
			Addresses.Clear();
		}

		private void RegisterFilters()
		{
			ServiceKnownTypeProvider.Register(typeof(IFiresecService).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Namespace == FilterNamespace));
		}
	}
}