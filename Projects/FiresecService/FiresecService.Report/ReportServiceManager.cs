using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;
using DevExpress.XtraReports.Service;
using DevExpress.DocumentServices.ServiceModel.Client;
using System.Xml;
using DevExpress.Xpf.Printing;
using FiresecAPI.Models;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common;
using FiresecAPI;

namespace FiresecService.Report
{
	public class ReportServiceManager
	{
		private const string FilterNamespace = @"FiresecAPI.SKD.ReportFilters";
		private static ReportServiceManager _instance;

		public static void Run()
		{
			_instance = new ReportServiceManager();
			_instance.RegisterFilters();
			_instance.Open();
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
#if DEBUG
				// Удалить - нужно только временно для тестов
				var remoteAddress = "http://127.0.0.1:2323/FiresecReportService/";
				var serviceHost = new ServiceHost(typeof(FiresecReportService));
				var testbinding = new BasicHttpBinding()
				{
					MaxReceivedMessageSize = 2097152,
					TransferMode = TransferMode.Streamed,
					ReaderQuotas = new XmlDictionaryReaderQuotas()
					{
						MaxArrayLength = 2097152
					}
				};
				serviceHost.AddServiceEndpoint(typeof(IReportService), testbinding, new Uri(remoteAddress));
				serviceHost.Open();
#endif
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
