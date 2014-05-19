using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DevicePropertiesViewModel : BaseViewModel
	{
		public DevicePropertiesViewModel(SKDDevice device)
		{
			Properties = new ObservableCollection<DeviceProperty>();

			foreach (var driverProperty in device.Driver.Properties)
			{
				if (driverProperty.IsAUParameter)
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
							case XDriverPropertyTypeEnum.EnumType:
								var parameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
								if (parameter != null)
									deviceProperty.Value = parameter.Name;
								break;

							case XDriverPropertyTypeEnum.IntType:
								deviceProperty.Value = property.Value.ToString();
								break;

							case XDriverPropertyTypeEnum.BoolType:
								var isTrue = property.Value > 0;
								deviceProperty.Value = isTrue ? "Есть" : "Нет";
								break;

							case XDriverPropertyTypeEnum.StringType:
								deviceProperty.Value = property.StringValue;
								break;
						}
					}

					Properties.Add(deviceProperty);
				}
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