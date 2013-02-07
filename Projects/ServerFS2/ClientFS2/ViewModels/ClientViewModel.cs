using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using Device = FiresecAPI.Models.Device;

namespace ClientFS2.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public ObservableCollection<DeviceViewModel> DevicesViewModel { get; private set; }

		public ClientViewModel()
		{
			ReadJournalCommand = new RelayCommand(OnReadJournal);
			SendRequestCommand = new RelayCommand(OnSendRequest);
			AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
			ShowDevicesTreeCommand = new RelayCommand(OnShowDevicesTree);
			DevicesViewModel = new ObservableCollection<DeviceViewModel>();
		}

		private DeviceViewModel _selectedDeviceViewModel;

		public DeviceViewModel SelectedDeviceViewModel
		{
			get { return _selectedDeviceViewModel; }
			set
			{
				_selectedDeviceViewModel = value;
				OnPropertyChanged("SelectedDeviceViewModel");
			}
		}

		private string _textBoxRequest;

		public string TextBoxRequest
		{
			get { return _textBoxRequest; }
			set
			{
				_textBoxRequest = value;
				OnPropertyChanged("TextBoxRequest");
			}
		}

		private string _textBoxResponse;

		public string TextBoxResponse
		{
			get { return _textBoxResponse; }
			set
			{
				_textBoxResponse = value;
				OnPropertyChanged("TextBoxResponse");
			}
		}

		public RelayCommand ReadJournalCommand { get; private set; }

		private void OnReadJournal()
		{
			ShowJournal(ServerHelper.GetJournalItems(SelectedDeviceViewModel.Device));
		}

		private static void ShowJournal(List<JournalItem> journalItems)
		{
			DialogService.ShowModalWindow(new JournalViewModel(journalItems));
		}

		public RelayCommand ShowDevicesTreeCommand { get; private set; }

		private static void OnShowDevicesTree()
		{
			DialogService.ShowModalWindow(new DevicesViewModel());
		}

		public RelayCommand SendRequestCommand { get; private set; }

		private void OnSendRequest()
		{
			var bytes = TextBoxRequest.Split()
				   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			var inbytes = ServerHelper.SendRequest(bytes);
			foreach (var b in inbytes)
				TextBoxResponse += b.ToString("X2") + " ";
		}

		public RelayCommand AutoDetectDeviceCommand { get; private set; }

		private void OnAutoDetectDevice()
		{
			var devices = new List<Device>();
			ServerHelper.AutoDetectDevice(devices);
			foreach (var device in devices)
			{
				DevicesViewModel.Add(new DeviceViewModel(device));
			}
			OnPropertyChanged("DevicesViewModel");
		}
	}
}