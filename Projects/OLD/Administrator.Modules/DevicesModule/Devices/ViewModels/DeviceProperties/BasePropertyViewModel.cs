using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.DeviceProperties
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected Device Device;
		protected DriverProperty DriverProperty;

		public BasePropertyViewModel(DriverProperty driverProperty, Device device)
		{
			DriverProperty = driverProperty;
			Device = device;
			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}
		}

		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		public bool IsAUParameter
		{
			get { return DriverProperty.IsAUParameter; }
		}
		public bool IsControl
		{
			get { return DriverProperty.IsControl; }
		}

		protected void Save(string value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.FSChanged = true;
			}

			if (Device.Properties == null)
				Device.Properties = new List<Property>();

			var property = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (property != null)
			{
				property.Name = DriverProperty.Name;
				property.Value = value;
			}
			else
			{
				var newProperty = new Property()
				{
					Name = DriverProperty.Name,
					Value = value
				};
				Device.Properties.Add(newProperty);
			}
			Device.OnChanged();
		}
	}
}