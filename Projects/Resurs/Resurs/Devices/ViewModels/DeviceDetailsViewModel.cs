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
		public ObservableCollection<DetailsParameterViewModel> Parameters { get; private set; }

		public DeviceDetailsViewModel(Device device)
		{
			Title = "Редактирование устройства " + device.Name + " " + device.FullAddress;
			Initialize(device);
		}

		public DeviceDetailsViewModel(DriverType driverType, Device parent)
		{
			Initialize(new Device(driverType, parent));
		}

		void Initialize(Device device)
		{
			Device = device;
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
			Device.IsActive = IsActive;
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
			if(ResursDAL.DBCash.SaveDevice(Device))
				return base.Save();
			return false;
		}
	}
}