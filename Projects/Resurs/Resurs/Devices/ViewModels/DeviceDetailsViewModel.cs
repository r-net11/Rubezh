using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public Device Device { get; private set; }
		Device _oldDevice;
		Device _parent;
		bool _isSaved;
		public ObservableCollection<DetailsParameterViewModel> Parameters { get; private set; }
		
		public DeviceDetailsViewModel(Device device)
		{
			Title = "Редактирование устройства " + device.Name + " " + device.FullAddress;
			_parent = device.Parent;
			Address = device.Address;
			Initialize(device);
			SelectBillCommand = new RelayCommand<Guid?>(OnSelectBill);
			RemoveBillLinkCommand = new RelayCommand(OnRemoveBillLink);
		}

		public DeviceDetailsViewModel(DriverType driverType, Device parent)
		{
			_parent = parent;
			Address = parent.Children.Count + 1;
			var device = new Device(driverType);
			Title = "Создание устройства " + device.Name;
			Initialize(device);
		}

		void Initialize(Device device)
		{
			Device = device;
			_oldDevice = device;
			Name = device.Name;
			Description = device.Description;
			IsActive = Device.IsActive;
			Bill = device.Bill;
			if (!IsActive)
			{
				IsEditComPort = device.DeviceType == DeviceType.Network;
				if (IsEditComPort)
				{
					ComPorts = new ObservableCollection<string>(GetComPorts());
					ComPort = ComPorts.FirstOrDefault(x => x == device.ComPort);
				}
				IsEditAddress = !IsEditComPort;
			}
			else
			{
				AddressString = device.DeviceType == DeviceType.Network ? device.ComPort : device.Address.ToString();
			}
			Parameters = new ObservableCollection<DetailsParameterViewModel>(Device.Parameters.Select(x => new DetailsParameterViewModel(x, this)));
			TariffTypes = new ObservableCollection<TariffType>();
			foreach (TariffType item in Enum.GetValues(typeof(TariffType)))
			{
				TariffTypes.Add(item);
			}
			TariffType = device.TariffType;
			HasTariffType = device.DeviceType == DeviceType.Counter;
			CanEditTariffType = device.Driver.CanEditTariffType && HasTariffType;
			HasReadOnlyTariffType = !device.Driver.CanEditTariffType && HasTariffType;
		}

		public bool CanEditTariffType { get; private set; }
		public bool HasReadOnlyTariffType { get; private set; }
		public bool HasTariffType { get; private set; }
		
		public ObservableCollection<TariffType> TariffTypes { get; private set; }

		TariffType _tariffType;
		public TariffType TariffType
		{
			get { return _tariffType; }
			set
			{
				_tariffType = value;
				OnPropertyChanged(() => TariffType);
			}
		}

		int _address;
		public int Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		public bool IsEditComPort { get; private set; }
		public bool IsEditAddress { get; private set; }
		public string AddressString { get; private set; }
		public ObservableCollection<string> ComPorts { get; private set; }

		string _comPort;
		public string ComPort
		{
			get { return _comPort; }
			set
			{
				_comPort = value;
				OnPropertyChanged(() => ComPort);
			}
		}

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

		public bool IsActive { get; private set; }
		
		Bill _bill;
		public Bill Bill
		{
			get { return _bill; }
			set
			{
				_bill = value;
				OnPropertyChanged(() => Bill);
				OnPropertyChanged(() => BillLinkName);
			}
		}

		public string BillLinkName
		{
			get 
			{
				return Bill == null ? "Нажмите для привязки счета" : Bill.Name;
			}
		}

		public RelayCommand<Guid?> SelectBillCommand { get; private set; }
		void OnSelectBill(Guid? billUid)
		{
			if (billUid.HasValue)
				Bootstrapper.MainViewModel.ConsumersViewModel.Select(billUid.Value);
			else
			{
				var selectBillViewModel = new SelectBillViewModel();
				if (DialogService.ShowModalWindow(selectBillViewModel))
				{
					var bill = selectBillViewModel.Bills.GetCheckedBill();
					if (bill != null)
						Bill = bill.GetBill();
				}
			}
		}

		public RelayCommand RemoveBillLinkCommand { get; private set; }
		void OnRemoveBillLink()
		{
			Bill = null;
		}

		protected override bool Save()
		{
			if(IsEditComPort)
			{
				if(ComPort == null)
				{
					MessageBoxService.Show("Не задан адрес COM порта");
					return false;
				}
				if (_parent.Children.Any(x => x.ComPort == ComPort && x.UID != Device.UID))
				{
					MessageBoxService.Show("Невозможно добавить устройство с повторяющимся адресом");
					return false;
				}
				else
					Address = int.Parse(ComPort.Substring(3));
					Device.ComPort = ComPort;
			}
			if (_parent.Children.Any(x => x.Address == Address && x.UID != Device.UID))
			{
				MessageBoxService.Show("Невозможно добавить устройство с повторяющимся адресом");
				return false;
			}
			else
				Device.Address = Address;
			Device.SetFullAddress();
			foreach (var item in Device.Children)
			{
				item.Parent = Device;
				item.SetFullAddress();
			}

			foreach (var item in Parameters)
			{
				var saveResult = item.Save();
				if (!saveResult)
					return false;
			}
			Device.Parameters = new List<Parameter>(Parameters.Select(x => x.Model));
			foreach (var item in Device.Parameters)
			{
				if (!item.DriverParameter.IsReadOnly)
				{
					var validateResult = item.Validate();
					if (validateResult != null)
					{
						MessageBoxService.Show("Ошибка в параметре " + item.DriverParameter.Description + ": " + validateResult);
						return false;
					}
				}
			}
			Device.Name = Name;
			Device.Description = Description;
			if (CanEditTariffType)
				Device.TariffType = TariffType;
			Device.Bill = Bill;
			Device.BillUID = Bill == null ? null : (Guid?)Bill.UID;
			var isDbMissmatch = false;
			if (IsActive)
			{
				foreach (var newParameter in Device.Parameters.Where(x => x.DriverParameter.IsWriteToDevice && x.DriverParameter.CanWriteInActive))
				{
					var oldParameter = _oldDevice.Parameters.FirstOrDefault(x => x.UID == newParameter.UID);
					switch (oldParameter.DriverParameter.ParameterType)
					{
						case ParameterType.Enum:
							if (oldParameter.IntValue != newParameter.IntValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, intValue: newParameter.IntValue);
							break;
						case ParameterType.String:
							if (oldParameter.StringValue != newParameter.StringValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, stringValue: newParameter.StringValue);
							break;
						case ParameterType.Int:
							if (oldParameter.IntValue != newParameter.IntValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, intValue: newParameter.IntValue);
							break;
						case ParameterType.Double:
							if (oldParameter.DoubleValue != newParameter.DoubleValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, doubleValue: newParameter.DoubleValue);
							break;
						case ParameterType.Bool:
							if (oldParameter.BoolValue != newParameter.BoolValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, boolValue: newParameter.BoolValue);
							break;
						case ParameterType.DateTime:
							if (oldParameter.DateTimeValue != newParameter.DateTimeValue)
								isDbMissmatch = isDbMissmatch || 
									!WriteParameter(Device.UID, newParameter.DriverParameter.Name, dateTimeValue: newParameter.DateTimeValue);
							break;
						default:
							break;
					}
				}
				Device.IsDbMissmatch = isDbMissmatch;
			}
			else
			{
				Device.SetParent(_parent, Address);
			}
			if (ResursDAL.DBCash.SaveDevice(Device))
			{
				_isSaved = true;
				return base.Save();
			}
			return false;
		}

		public override bool OnClosing(bool isCanceled)
		{
			if(_isSaved)
				return base.OnClosing(isCanceled);
			var isConfirmed = MessageBoxService.ShowConfirmation("Вы уверены, что хотите закрыть окно без сохранения инменений?");
			if (!isConfirmed)
				return true;
			return base.OnClosing(isCanceled);
		}

		IEnumerable<string> GetComPorts()
		{
#if DEBUG
			var allPorts = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5" };
#else
			var allPorts = SerialPort.GetPortNames();
#endif
			return allPorts.Except(_parent.Children.Where(x => x.UID != Device.UID).Select(x => x.ComPort));
		}

		public bool SetStatus(Guid deviceUID, bool isActive)
		{
			return true;
		}

		public bool WriteParameter(Guid deviceUID, 
			string parameterName, 
			string stringValue = null, 
			int? intValue = null, 
			double? doubleValue = null, bool? 
			boolValue = null, 
			DateTime? dateTimeValue = null) 
		{ 
			return true; 
		}
	}
}