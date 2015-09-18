using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public Device Device { get; private set; }

		public DeviceDetailsViewModel(Device device = null)
		{
			if (device == null)
			{
				device = new Device();
				Title = "Создание устройства";
			}
			else
			{
				Title = "Редактирование устройства";
			}

			Name = device.Name;
			Description = device.Description;
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

		protected override bool Save()
		{
			Device.Name = Name;
			Device.Description = Description;
			return base.Save();
		}
	}
}