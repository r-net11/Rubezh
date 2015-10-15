using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
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
		Device _OldDevice;
		Device _Parent;
		bool _IsNew;
		public ObservableCollection<DetailsParameterViewModel> Parameters { get; private set; }
		
		public DeviceDetailsViewModel(Device device)
		{
			Title = "Редактирование устройства " + device.Name + " " + device.FullAddress;
			_Parent = device.Parent;
			Address = device.Address;
			Initialize(device);
		}

		public DeviceDetailsViewModel(DriverType driverType, Device parent)
		{
			_IsNew = true;
			_Parent = parent;
			Address = parent.Children.Count + 1;
			var device = new Device(driverType);
			Title = "Создание устройства " + device.Name;
			Initialize(device);
		}

		void Initialize(Device device)
		{
			Device = device;
			_OldDevice = device;
			Description = device.Description;
			IsActive = device.IsActive;
			Parameters = new ObservableCollection<DetailsParameterViewModel>(Device.Parameters.Select(x => new DetailsParameterViewModel(x)));
			IsNetwork = device.DeviceType == DeviceType.Network;
			if(IsNetwork)
			{
				ComPorts = new ObservableCollection<string>(GetComPorts());
				ComPort = ComPorts.FirstOrDefault(x => x == device.ComPort);
			}
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

		TariffType _TariffType;
		public TariffType TariffType
		{
			get { return _TariffType; }
			set
			{
				_TariffType = value;
				OnPropertyChanged(() => TariffType);
			}
		}

		int _Address;
		public int Address
		{
			get { return _Address; }
			set
			{
				_Address = value;
				OnPropertyChanged(() => Address);
			}
		}

		public bool IsNetwork { get; private set; }
		public ObservableCollection<string> ComPorts { get; private set; }
		string _ComPort;
		public string ComPort
		{
			get { return _ComPort; }
			set
			{
				_ComPort = value;
				OnPropertyChanged(() => ComPort);
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

		bool _IsActive;
		public bool IsActive
		{
			get { return _IsActive; }
			set
			{
				_IsActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}

		protected override bool Save()
		{
			if(IsNetwork)
			{
				if (_Parent.Children.Any(x => x.ComPort == ComPort && x.UID != Device.UID))
				{
					MessageBoxService.Show("Невозможно добавить устройство с повторяющимся адресом");
					return false;
				}
				else
					Address = int.Parse(ComPort.Substring(3));
					Device.ComPort = ComPort;
			}
			if (_Parent.Children.Any(x => x.Address == Address && x.UID != Device.UID))
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
			
			Device.Parameters = new List<Parameter>(Parameters.Select(x => x.Model));
			foreach (var item in Device.Parameters)
			{
				if (!item.DriverParameter.IsReadOnly)
				{
					var validateResult = item.Validate();
					if (validateResult != null)
					{
						MessageBoxService.Show("Ошибка в параметре " + item.DriverParameter.Name + ": " + validateResult);
						return false;
					}
				}
			}
			foreach (var item in Parameters)
			{
				var saveResult = item.Save();
				if (!saveResult)
					return false;
			}
			Device.Description = Description;
			if (CanEditTariffType)
				Device.TariffType = TariffType;
			Device.IsActive = IsActive;
			if (!_IsNew)
			{
				if (Device.IsActive != _OldDevice.IsActive)
					SetStatus(Device.UID, Device.IsActive);
				var isDbMissmatch = false;
				if (Device.IsActive)
				{
					foreach (var newParameter in Device.Parameters.Where(x => x.DriverParameter.IsWriteToDevice))
					{
						var oldParameter = _OldDevice.Parameters.FirstOrDefault(x => x.UID == newParameter.UID);
						switch (oldParameter.DriverParameter.ParameterType)
						{
							case ParameterType.Enum:
								if (oldParameter.IntValue != newParameter.IntValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, intValue: newParameter.IntValue);
								break;
							case ParameterType.String:
								if (oldParameter.StringValue != newParameter.StringValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, stringValue: newParameter.StringValue);
								break;
							case ParameterType.Int:
								if (oldParameter.IntValue != newParameter.IntValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, intValue: newParameter.IntValue);
								break;
							case ParameterType.Double:
								if (oldParameter.DoubleValue != newParameter.DoubleValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, doubleValue: newParameter.DoubleValue);
								break;
							case ParameterType.Bool:
								if (oldParameter.BoolValue != newParameter.BoolValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, boolValue: newParameter.BoolValue);
								break;
							case ParameterType.DateTime:
								if (oldParameter.DateTimeValue != newParameter.DateTimeValue)
									isDbMissmatch = isDbMissmatch || !WriteParameter(Device.UID, newParameter.DriverParameter.Name, dateTimeValue: newParameter.DateTimeValue);
								break;
							default:
								break;
						}
					}
				}
				Device.IsDbMissmatch = isDbMissmatch;
			}
			else
			{
				Device.SetParent(_Parent, Address);
				AddDevice(Device);
			}
			if(ResursDAL.DBCash.SaveDevice(Device))
				return base.Save();
			return false;
		}

		IEnumerable<string> GetComPorts()
		{
#if DEBUG
			var allPorts = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5" };
#else
			var allPorts = SerialPort.GetPortNames();
#endif
			return allPorts.Except(_Parent.Children.Where(x => x.UID != Device.UID).Select(x => x.ComPort));
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
		
		public bool AddDevice(Device device)
		{
			return true;
		}

	}
}