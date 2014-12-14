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

namespace FiresecService.Report
{
	public class ReportServiceManager
	{
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
				var remoteAddress = "http://127.0.0.1:2323/FiresecReportService/";
				_serviceHost = new ServiceHost(typeof(FiresecReportService));
				var binding = new BasicHttpBinding() 
				{ 
					MaxReceivedMessageSize = 2097152, 
					TransferMode = TransferMode.Streamed, 
					ReaderQuotas = new XmlDictionaryReaderQuotas() 
					{ 
						MaxArrayLength = 2097152 
					} 
				};
				_serviceHost.AddServiceEndpoint(typeof(IReportService), binding, new Uri(remoteAddress));
				_serviceHost.Open();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ReportServiceManager.Open");
				//UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message);
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
			ServiceKnownTypeProvider.Register<Parameter>();
		}
	}
}
