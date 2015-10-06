using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public Device Device { get; private set; }
		Device _OldDevice;
		bool _IsNew;
		public ObservableCollection<DetailsParameterViewModel> Parameters { get; private set; }

		public DeviceDetailsViewModel(Device device)
		{
			Title = "Редактирование устройства " + device.Name + " " + device.FullAddress;
			Initialize(device);
		}

		public DeviceDetailsViewModel(DriverType driverType, Device parent)
		{
			_IsNew = true;
			Initialize(new Device(driverType, parent));
		}

		void Initialize(Device device)
		{
			Device = device;
			_OldDevice = device;
			Description = device.Description;
			IsActive = device.IsActive;
			Parameters = new ObservableCollection<DetailsParameterViewModel>(Device.Parameters.Select(x => new DetailsParameterViewModel(x)));
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
			foreach (var item in Parameters)
			{
				var saveResult = item.Save();
				if (!saveResult)
					return false;
			}
			Device.Description = Description;
			Device.Parameters = new List<Parameter>(Parameters.Select(x => x.Model));
			foreach (var item in Device.Parameters)
			{
				var validateResult = item.Validate();
				if (validateResult != null)
				{
					MessageBoxService.Show("Ошибка в параметре " + item.DriverParameter.Name + ": " + validateResult);
					return false;
				}
			}
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
				AddDevice(Device);
			}
			if(ResursDAL.DBCash.SaveDevice(Device))
				return base.Save();
			return false;
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