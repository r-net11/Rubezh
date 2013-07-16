using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.DeviceProperties
{
	public class BaseAUPropertyViewModel : BaseViewModel
	{
		protected Device Device;
		protected DriverProperty DriverProperty;

		public BaseAUPropertyViewModel(DriverProperty driverProperty, Device device)
		{
			DriverProperty = driverProperty;
			Device = device;

			if (!Device.SystemAUProperties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}
			var deviceProperty = Device.DeviceAUProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (deviceProperty != null)
			{
				DeviceAUParameterValue = deviceProperty.Value;
			}
			else
				DeviceAUParameterValue = "Неизвестно";

			var systemProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (!DriverProperty.IsReadOnly)
			{
				IsMissmatch = (deviceProperty != null && systemProperty != null && deviceProperty.Value != systemProperty.Value);
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

		bool _isMissmatch;
		public bool IsMissmatch
		{
			get { return _isMissmatch; }
			set
			{
				_isMissmatch = value;
				OnPropertyChanged("IsMissmatch");
			}
		}

		public virtual string DeviceAUParameterValue { get; protected set; }

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}

		protected void Save(string value, bool useSaveService = true)
		{
            if (useSaveService)
            {
				ServiceFactory.SaveService.FSParametersChanged = true;
            }

			var systemProperty = Device.SystemAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
			}
			else
			{
				var newProperty = new Property()
				{
					Name = DriverProperty.Name,
					Value = value
				};
				Device.SystemAUProperties.Add(newProperty);
			}
			var deviceProperty = Device.DeviceAUProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			IsMissmatch = (deviceProperty != null && systemProperty != null && deviceProperty.Value != systemProperty.Value);
			Device.OnChanged();
		}
	}
}