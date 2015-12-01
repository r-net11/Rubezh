﻿using System.Collections.Generic;
using Common;
using DevExpress.Office.Utils;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Service;
using FiresecAPI;
using Infrastructure.Common;
using System;
using System.Linq;
using System.ServiceModel;

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
						IsRunning = _instance.Open();
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
				//Address = AppServerConnectionManager.ReportServerUri;
				//var binding = BindingHelper.CreateBindingFromAddress(Address);
				//_serviceHost = new ServiceHost(typeof(FiresecReportService));
				//_serviceHost.AddServiceEndpoint(typeof(IReportService), binding, Address);
				//_serviceHost.Open();
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
				Addresses.Add(String.Format("Локальный адрес: {0}", netpipeAddress));
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
				Addresses.Add(String.Format("Удаленный адрес: {0}", remoteAddress));
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