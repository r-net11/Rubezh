﻿using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Consumers;
using Resurs.Reports.Templates;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public partial class ConsumerDetailsViewModel
	{
		void InitializeBill(Consumer consumer)
		{
			ShowReceiptCommand = new RelayCommand(OnShowReceipt, CanShowReceipt);

			Receipts = ReceiptHelper.GetAllTemplate();
			SelectedReceipt = Receipts.FirstOrDefault();
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand<DeviceViewModel>(OnRemoveDevice);
			SelectDeviceCommand = new RelayCommand<Guid>(OnSelectDevice);
		}
		string _number;
		public string Number
		{
			get { return _number; }
			set
			{
				_number = value;
				OnPropertyChanged(() => Number);
			}
		}

		Decimal _balance;
		public Decimal Balance
		{
			get { return _balance; }
			set
			{
				_balance = value;
				OnPropertyChanged(() => Balance);
			}
		}

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices 
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		public List<ReceiptTemplate> Receipts { get; private set; }
		ReceiptTemplate _selectedReceipt;
		public ReceiptTemplate SelectedReceipt
		{
			get { return _selectedReceipt; }
			set
			{
				_selectedReceipt = value;
				OnPropertyChanged(() => SelectedReceipt);
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var exceptDeviceUids = new List<Guid>();
			exceptDeviceUids.AddRange(Devices.Select(x => x.Device.UID));
			exceptDeviceUids.AddRange(DBCash.GetAllChildren(DBCash.RootDevice).Where(x => x.ConsumerUID != null).Select(x => x.UID));
			var selectDeviceViewModel = new SelectDeviceViewModel(exceptDeviceUids);
			if (DialogService.ShowModalWindow(selectDeviceViewModel) && selectDeviceViewModel.SelectedDevice != null)
				Devices.Add(new DeviceViewModel(selectDeviceViewModel.SelectedDevice.Device));
		}

		public RelayCommand<DeviceViewModel> RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice(DeviceViewModel device)
		{
			Devices.Remove(device);
		}

		public RelayCommand<Guid> SelectDeviceCommand { get; private set; }
		void OnSelectDevice(Guid deviceUid)
		{
			Bootstrapper.MainViewModel.DevicesViewModel.Select(deviceUid);
		}
				
		public RelayCommand ShowReceiptCommand { get; private set; }
		void OnShowReceipt()
		{
			//var receiptViewModel = new ReceiptViewModel(SelectedReceipt, Bill);
			//Infrastructure.Common.Windows.DialogService.ShowModalWindow(receiptViewModel);
		}
		bool CanShowReceipt()
		{
			return SelectedReceipt != null;
		}
	}
}