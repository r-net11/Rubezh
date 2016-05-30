using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	/// <summary>
	/// Описывает настройки параметров запуска Сервера приложений
	/// </summary>
	public class AppServerSettingsViewModel : BaseViewModel
	{
		private string _serviceAddress;
		private int _servicePort;
		private int _reportServicePort;
		private bool _isRemoteConnectionsEnabled;
		private string _attachmentsFolder;

		/// <summary>
		/// IP-адрес сервера
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

		/// <summary>
		/// Порт сервиса отчетов
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

		/// <summary>
		/// Разрешены ли удаленные соединения?
		/// </summary>
		public bool IsRemoteConnectionsEnabled
		{
			get { return _isRemoteConnectionsEnabled; }
			set
			{
				if (_isRemoteConnectionsEnabled == value)
					return;
				_isRemoteConnectionsEnabled = value;
				OnPropertyChanged(() => IsRemoteConnectionsEnabled);
				InvalidateServiceAddress();
			}
		}

		public string AttachmentsFolder
		{
			get { return _attachmentsFolder; }
			set
			{
				if (_attachmentsFolder == value)
					return;
				_attachmentsFolder = value;
				OnPropertyChanged(() => AttachmentsFolder);
			}
		}

		/// <summary>
		/// Список доступных ip-адресов для хоста, где размещен сервер приложений
		/// </summary>
		public ObservableCollection<string> AvailableIpAddresses { get; private set; }

		public RelayCommand ApplyCommand { get; private set; }

		public RelayCommand SelectAttachmentsFolderCommand { get; private set; }

		public AppServerSettingsViewModel()
		{
			ApplyCommand = new RelayCommand(OnApply);
			SelectAttachmentsFolderCommand = new RelayCommand(OnSelectAttachmentsFolder);
			InitializeAvailableIpAddresses();
			ReadFromModel();
		}

		private void OnApply()
		{
			WriteToModel();
			if (ServiceRepository.Instance.ServiceStateHolder.State != ServiceState.Stoped)
				MessageBoxService.ShowWarning("Параметры вступят в силу после перезапуска сервера приложений");
		}

		private void OnSelectAttachmentsFolder()
		{
			var dlg = new FolderBrowserDialog
			{
				//RootFolder = Environment.SpecialFolder.MyComputer,
				SelectedPath = AttachmentsFolder
			};
			var result = dlg.ShowDialog();
			if (result != DialogResult.OK)
				return;
			AttachmentsFolder = dlg.SelectedPath;
		}

		/// <summary>
		/// Получаем адреса хоста, на котором запущен Монитор.
		/// Считаем что Сервер размещен на этом же хосте!!!
		/// </summary>
		private void InitializeAvailableIpAddresses()
		{
			var hostIpAdresses = NetworkHelper.GetHostIpAddresses();
			hostIpAdresses.Add(NetworkHelper.Localhost);
			AvailableIpAddresses = new ObservableCollection<string>(hostIpAdresses);
		}

		private void InvalidateServiceAddress()
		{
			if (!IsRemoteConnectionsEnabled)
				ServiceAddress = "localhost";
		}

		private void ReadFromModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;

			ServiceAddress = settings.ServiceAddress;
			ServicePort = settings.ServicePort;
			ReportServicePort = settings.ReportServicePort;
			IsRemoteConnectionsEnabled = settings.EnableRemoteConnections;
			AttachmentsFolder = settings.AttachmentsFolder;
		}

		private void WriteToModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;

			settings.ServiceAddress = ServiceAddress;
			settings.ServicePort = ServicePort;
			settings.ReportServicePort = ReportServicePort;
			settings.EnableRemoteConnections = IsRemoteConnectionsEnabled;
			settings.AttachmentsFolder = AttachmentsFolder;
			AppServerSettingsHelper.Save();
		}
	}
}