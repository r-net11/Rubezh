using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Resurs.Processor;
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
		bool _isCancelClosing;
		public ObservableCollection<DetailsParameterViewModel> Parameters { get; private set; }
		public bool IsNetwork { get; private set; }
		
		DeviceDetailsViewModel()
		{
			SelectConsumerCommand = new RelayCommand<Guid?>(OnSelectConsumer);
			RemoveConsumerLinkCommand = new RelayCommand(OnRemoveConsumerLink);
			SelectTariffCommand = new RelayCommand(OnSelectTariff);
			RemoveTariffCommand = new RelayCommand(OnRemoveTariff);
		}
		
		public DeviceDetailsViewModel(Device device)
			: this()
		{
			Title = "Редактирование устройства " + device.Name + " " + device.FullAddress;
			_parent = device.Parent;
			Address = device.Address;
			Initialize(device);
		}

		public DeviceDetailsViewModel(DriverType driverType, Device parent)
			: this()
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
			Consumer = device.Consumer;
			IsNetwork = device.DeviceType == DeviceType.Network;
			if (!IsActive)
			{
				IsEditComPort = IsNetwork;
				if (IsEditComPort)
				{
					ComPorts = new ObservableCollection<string>(GetComPorts());
					ComPort = ComPorts.FirstOrDefault(x => x == device.ComPort);
				}
				IsEditAddress = !IsEditComPort;
			}
			else
			{
				AddressString = IsNetwork ? device.ComPort : device.Address.ToString();
			}
			Parameters = new ObservableCollection<DetailsParameterViewModel>(Device.Parameters.Select(x => new DetailsParameterViewModel(x, this)));
			Commands = new ObservableCollection<CommandViewModel>(Device.Commands.Select(x => new CommandViewModel(Device, x)));
			TariffTypes = new ObservableCollection<TariffType>();
			foreach (TariffType item in Enum.GetValues(typeof(TariffType)))
			{
				TariffTypes.Add(item);
			}
			Tariff = device.Tariff;
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

		#region Consumer
		Consumer _consumer;
		public Consumer Consumer
		{
			get { return _consumer; }
			set
			{
				_consumer = value;
				OnPropertyChanged(() => Consumer);
				OnPropertyChanged(() => ConsumerLinkName);
			}
		}

		public string ConsumerLinkName
		{
			get
			{
				return Consumer == null ? "Нажмите для привязки счета" : "[" + Consumer.Number + "] " + Consumer.Name;
			}
		}

		public RelayCommand<Guid?> SelectConsumerCommand { get; private set; }
		void OnSelectConsumer(Guid? consumerUid)
		{
			if (consumerUid.HasValue)
				Bootstrapper.MainViewModel.ConsumersViewModel.Select(consumerUid.Value);
			else
			{
				var selectConsumerViewModel = new SelectConsumerViewModel("Выбор лицевого счета для привязки");
				if (DialogService.ShowModalWindow(selectConsumerViewModel))
					Consumer = selectConsumerViewModel.SelectedConsumer.Consumer;
			}
		}

		public RelayCommand RemoveConsumerLinkCommand { get; private set; }
		void OnRemoveConsumerLink()
		{
			Consumer = null;
		}
		#endregion

		#region Tariff
		Tariff _tariff;
		public Tariff Tariff
		{
			get { return _tariff; }
			set
			{
				_tariff = value;
				OnPropertyChanged(() => Tariff);
				OnPropertyChanged(() => HasTariff);
			}
		}

		public bool HasTariff { get { return Tariff != null; } }
		
		public RelayCommand SelectTariffCommand { get; private set; }
		void OnSelectTariff()
		{
			var tariffSelectionViewModel = new TariffSelectionViewModel(this);
			if(DialogService.ShowModalWindow(tariffSelectionViewModel))
			{
				Tariff = tariffSelectionViewModel.Tariff;
			}
		}

		public RelayCommand RemoveTariffCommand { get; private set; }
		void OnRemoveTariff()
		{
			Tariff = null;
		}
		#endregion

		public ObservableCollection<CommandViewModel> Commands { get; private set; }
		public bool HasCommands { get { return Commands.IsNotNullOrEmpty(); } }
		
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
			Device.Consumer = Consumer;
			Device.ConsumerUID = Consumer == null ? null : (Guid?)Consumer.UID;
			Device.Tariff = Tariff;
			Device.TariffUID = Tariff == null ? null : (Guid?)Tariff.UID;
			
			if (IsActive)
			{
				var isDbMissmatch = false;
				foreach (var newParameter in Device.Parameters.Where(x => x.DriverParameter.IsWriteToDevice && x.DriverParameter.CanWriteInActive))
				{
					var oldParameter = _oldDevice.Parameters.FirstOrDefault(x => x.UID == newParameter.UID);
					if (!Parameter.IsEqual(oldParameter, newParameter))
					{
						isDbMissmatch = isDbMissmatch || !DeviceProcessor.Instance.WriteParameter(Device.UID, newParameter.DriverParameter.Name, newParameter.ValueType);
					}
				}
				Device.IsDbMissmatch = isDbMissmatch;
			}
			else
			{
				Device.SetParent(_parent, Address);
			}
			if (ResursDAL.DbCache.SaveDevice(Device))
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
			if (!_isCancelClosing)
			{
				var isConfirmed = MessageBoxService.ShowConfirmation("Вы уверены, что хотите закрыть окно без сохранения изменений?");
				if (!isConfirmed)
				{
					_isCancelClosing = true;
					return true;
				}
			}
			else
			{
				_isCancelClosing = false;
				return true;
			}
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
	}
}