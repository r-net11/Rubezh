using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class AppServerViewModel : BaseViewModel
	{
		/// <summary>
		/// Список доступных ip-адресов для хоста, где размещен сервер приложений
		/// </summary>
		public ObservableCollection<string> AvailableIpAddresses { get; private set; }

		private void InitializeAvailableIpAddresses()
		{
			AvailableIpAddresses = new ObservableCollection<string>(NetworkHelper.GetHostIpAddresses());
		}

		private string _serviceAddress;
		/// <summary>
		/// Ip-адрес сервера приложений
		/// </summary>
		public string ServiceAddress
		{
			get { return _serviceAddress; }
			set
			{
				if (_serviceAddress == value)
					return;
				_serviceAddress = value;
				OnPropertyChanged(() => ServiceAddress);
			}
		}

		private int _servicePort;
		/// <summary>
		/// Порт
		/// </summary>
		public int ServicePort
		{
			get { return _servicePort; }
			set
			{
				if (_servicePort == value)
					return;
				_servicePort = value;
				OnPropertyChanged(() => ServicePort);
			}
		}

		private int _reportServicePort;
		/// <summary>
		/// Порт сервиса генерации отчетов
		/// </summary>
		public int ReportServicePort
		{
			get { return _reportServicePort; }
			set
			{
				if (_reportServicePort == value)
					return;
				_reportServicePort = value;
				OnPropertyChanged(() => ReportServicePort);
			}
		}

		private bool _enableRemoteConnections;
		public bool EnableRemoteConnections
		{
			get { return _enableRemoteConnections; }
			set
			{
				if (_enableRemoteConnections == value)
					return;
				_enableRemoteConnections = value;
				OnPropertyChanged(() => EnableRemoteConnections);

				InvalidateServiceAddress();
			}
		}

		private void InvalidateServiceAddress()
		{
			if (!EnableRemoteConnections)
				ServiceAddress = "localhost";
		}

		private bool _useHasp;

		public bool UseHasp
		{
			get { return _useHasp; }
			set
			{
				if (_useHasp == value)
					return;
				_useHasp = value;
				OnPropertyChanged(() => UseHasp);
			}
		}

		private string _dbServerName;

		public string DBServerName
		{
			get { return _dbServerName; }
			set
			{
				if (_dbServerName == value)
					return;
				_dbServerName = value;
				OnPropertyChanged(() => DBServerName);
			}
		}

		private bool _createNewDBOnOversize;

		public bool CreateNewDBOnOversize
		{
			get { return _createNewDBOnOversize; }
			set
			{
				if (_createNewDBOnOversize == value)
					return;
				_createNewDBOnOversize = value;
				OnPropertyChanged(() => CreateNewDBOnOversize);
			}
		}

		public AppServerViewModel()
		{
			InitializeAvailableIpAddresses();
			ReadFromModel();
		}

		private void ReadFromModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;

			ServiceAddress = settings.ServiceAddress;
			ServicePort = settings.ServicePort;
			ReportServicePort = settings.ReportServicePort;
			EnableRemoteConnections = settings.EnableRemoteConnections;
			UseHasp = settings.UseHasp;
			DBServerName = settings.DBServerName;
			CreateNewDBOnOversize = settings.CreateNewDBOnOversize;
		}

		private void WriteToModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;

			settings.ServiceAddress = ServiceAddress;
			settings.ServicePort = ServicePort;
			settings.ReportServicePort = ReportServicePort;
			settings.EnableRemoteConnections = EnableRemoteConnections;
			settings.UseHasp = UseHasp;
			settings.DBServerName = DBServerName;
			settings.CreateNewDBOnOversize = CreateNewDBOnOversize;
		}

		public void Save()
		{
			WriteToModel();
		}
	}
}
