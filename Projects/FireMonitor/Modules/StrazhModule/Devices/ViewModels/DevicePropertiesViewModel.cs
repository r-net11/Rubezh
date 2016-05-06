using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DevicePropertiesViewModel : BaseViewModel
	{
		public DevicePropertiesViewModel(SKDDevice device)
		{
			Properties = new ObservableCollection<DeviceProperty>();

			foreach (var driverProperty in device.Driver.Properties.Where(x => !String.Equals(x.Name, "login", StringComparison.InvariantCultureIgnoreCase) && !String.Equals(x.Name, "password", StringComparison.InvariantCultureIgnoreCase)))
			{
				var deviceProperty = new DeviceProperty()
				{
					Name = driverProperty.Caption
				};

				var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
				if (property != null)
				{
					switch (driverProperty.DriverPropertyType)
					{
						case SKDDriverType.EnumType:
							var parameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
							if (parameter != null)
								deviceProperty.Value = parameter.Name;
							break;

						case SKDDriverType.IntType:
							deviceProperty.Value = property.Value.ToString();
							break;

						case SKDDriverType.BoolType:
							var isTrue = property.Value > 0;
							deviceProperty.Value = isTrue ? "Есть" : "Нет";
							break;

						case SKDDriverType.StringType:
							deviceProperty.Value = property.StringValue;
							break;
					}
				}

				Properties.Add(deviceProperty);
			}
		}

		public ObservableCollection<DeviceProperty> Properties { get; private set; }

		public bool HasAUParameters
		{
			get { return Properties.Count > 0; }
		}
	}

	public class DeviceProperty
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}