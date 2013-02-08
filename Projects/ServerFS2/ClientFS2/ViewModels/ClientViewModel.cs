using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using Device = FiresecAPI.Models.Device;
using ServerFS2.DataBase;
using System;
using FiresecAPI.Models;
using FiresecAPI;

namespace ClientFS2.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }

		public ClientViewModel()
		{
			ReadJournalCommand = new RelayCommand(OnReadJournal);
			SendRequestCommand = new RelayCommand(OnSendRequest);
			AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
			DevicesViewModel = new DevicesViewModel();
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

		public RelayCommand SendRequestCommand { get; private set; }
		private void OnSendRequest()
		{
			var bytes = TextBoxRequest.Split()
				   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			var inbytes = ServerHelper.SendRequest(bytes);
			foreach (var b in inbytes)
				TextBoxResponse += b.ToString("X2") + " ";
		}

		public RelayCommand ReadJournalCommand { get; private set; }
		private void OnReadJournal()
		{
			var journalItems = ServerHelper.GetJournalItems(DevicesViewModel.SelectedDevice.Device);
			var journalViewModel = new JournalViewModel(journalItems);
			DialogService.ShowModalWindow(journalViewModel);
		}

		public RelayCommand AutoDetectDeviceCommand { get; private set; }
		private void OnAutoDetectDevice()
		{
			var devices = ServerHelper.AutoDetectDevice();
			var autoDetectDevicesViewModel = new AutoDetectDevicesViewModel(devices);
			DialogService.ShowModalWindow(autoDetectDevicesViewModel);
		}		
	}
}