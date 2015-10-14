using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class BillViewModel : BaseViewModel
	{
		public BillsViewModel BillsViewModel { get; private set; }
		public BillViewModel(Bill bill, BillsViewModel billsViewModel, bool isReadOnly)
		{
			BillsViewModel = billsViewModel;

			Uid = bill.UID;
			Consumer = bill.Consumer;
			Name = bill.Name;
			Description = bill.Description;
			Balance = bill.Balance;
			TemplatePath = bill.TemplatePath;
			IsReadOnly = isReadOnly;

			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand<DeviceViewModel>(OnRemoveDevice);
			SelectDeviceCommand = new RelayCommand<Guid>(OnSelectDevice);

			Devices = new ObservableCollection<DeviceViewModel>(bill.Devices.Select(x => new DeviceViewModel(x)));
		}

		public bool IsReadOnly { get; private set; }

		public Guid Uid { get; private set; }

		public Consumer Consumer { get; private set; }

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; set; }

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

		string _templatePath;
		public string TemplatePath
		{
			get { return _templatePath; }
			set
			{
				_templatePath = value;
				OnPropertyChanged(() => TemplatePath);
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var exceptDeviceUids = new List<Guid>();
			foreach (var billViewModel in BillsViewModel.Bills)
				exceptDeviceUids.AddRange(billViewModel.Devices.Select(x => x.Device.UID));
			exceptDeviceUids.AddRange(DBCash.GetAllChildren(DBCash.RootDevice).Where(x => x.BillUID != null).Select(x => x.UID));
			var selectDeviceViewModel = new SelectDeviceViewModel(exceptDeviceUids);
			if (DialogService.ShowModalWindow(selectDeviceViewModel) && selectDeviceViewModel.SelectedDevice != null)
			{
				Devices.Add(new DeviceViewModel(selectDeviceViewModel.SelectedDevice.Device));	
			}
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

		public Bill GetBill()
		{
			return new Bill
			{
				Balance = this.Balance,
				Consumer = this.Consumer,
				Description = this.Description,
				Name = this.Name,
				TemplatePath = this.TemplatePath,
				UID = this.Uid,
				Devices = this.Devices.Select(x => x.Device).ToList()
			};
		}
	}
}
