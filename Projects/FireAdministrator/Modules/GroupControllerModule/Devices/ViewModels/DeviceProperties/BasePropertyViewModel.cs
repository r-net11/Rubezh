using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected GKDevice Device;
		protected GKDriverProperty DriverProperty;

		public BasePropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
		{
			DriverProperty = driverProperty;
			IsAUParameter = driverProperty.IsAUParameter;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}

			if (Device.DeviceProperties == null)
			{
				Device.DeviceProperties = new List<GKProperty>();
			}

			var deviceProperty = Device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (deviceProperty != null)
			{
				double value = deviceProperty.Value;
				if (DriverProperty.Multiplier != 0)
					value /= DriverProperty.Multiplier;
				DeviceAUParameterValue = value.ToString();
			}
			else
				DeviceAUParameterValue = "Неизвестно";
		}

		public bool IsAUParameter { get; set; }
		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		public virtual string DeviceAUParameterValue { get; protected set; }

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}

		protected void Save(ushort value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.GKChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
			}
			else
			{
				var newProperty = new GKProperty()
				{
					Name = DriverProperty.Name,
					Value = value,
				};
				Device.Properties.Add(newProperty);
			}
			Device.OnChanged();
		}
	}
}