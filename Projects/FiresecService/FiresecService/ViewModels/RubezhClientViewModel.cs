using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiresecIntegrationClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class RubezhClientViewModel : BaseViewModel
	{
		private readonly HttpClient _client;

		private bool _isClientEnabled;
		public bool IsClientEnabled
		{
			get { return _isClientEnabled; }
			set
			{
				if (_isClientEnabled == value) return;
				_isClientEnabled = value;
				OnPropertyChanged(() => IsClientEnabled);
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				if (_port == value) return;
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		private string _ipAddress;
		public string IPAddress
		{
			get { return _ipAddress; }
			set
			{
				if(string.Equals(_ipAddress, value)) return;
				_ipAddress = value;
				OnPropertyChanged(() => IPAddress);
			}
		}

		public RelayCommand PingCommand { get; set; }
		public RelayCommand ApplyCommand { get; set; }

		public RubezhClientViewModel()
		{
			_client = new HttpClient();
			PingCommand = new RelayCommand(OnPing);
			ApplyCommand = new RelayCommand(OnApply);
		}

		public void OnPing()
		{

		}

		public void OnApply()
		{
			if(IsClientEnabled)
				Task.Factory.StartNew(_client.Start);
			else
				_client.Stop();
		}
	}
}
