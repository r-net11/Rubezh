using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DevicePropertiesViewModel : BaseViewModel
	{
		public DevicePropertiesViewModel(GKDevice device)
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
							case GKDriverPropertyTypeEnum.EnumType:
								var parameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
								if (parameter != null)
									deviceProperty.Value = parameter.Name;
								break;

							case GKDriverPropertyTypeEnum.IntType:
								double doubleValue = property.Value;
								if (driverProperty.Multiplier != 0)
									doubleValue /= driverProperty.Multiplier;
								deviceProperty.Value = doubleValue.ToString();
								break;

							case GKDriverPropertyTypeEnum.BoolType:
								var isTrue = property.Value > 0;
								deviceProperty.Value = isTrue ? "Есть" : "Нет";
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