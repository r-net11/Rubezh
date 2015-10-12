using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace GKModule.DeviceProperties
{
	public class EnumAUPropertyViewModel : BaseAUPropertyViewModel
	{
		public EnumAUPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
			{
				var driverPropertyParameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
				if (driverPropertyParameter != null)
				{
					_selectedParameter = driverPropertyParameter;
				}
			}
			else
			{
				var enumdriverProperty = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default);
				if (enumdriverProperty != null)
					_selectedParameter = enumdriverProperty;
			}
		}

		public List<GKDriverPropertyParameter> Parameters
		{
			get { return DriverProperty.Parameters; }
		}

		GKDriverPropertyParameter _selectedParameter;
		public GKDriverPropertyParameter SelectedParameter
		{
			get { return _selectedParameter; }
			set
			{
				_selectedParameter = value;
				OnPropertyChanged(() => SelectedParameter);
				Save(value.Value);
			}
		}

		public override string DeviceAUParameterValue
		{
			get
			{
				var deviceProperty = Device.DeviceProperties.FirstOrDefault(x => x.Name == DriverProperty.Name);
				if (deviceProperty == null)
					return "Неизвестно";
				var driverPropertyParameter = DriverProperty.Parameters.FirstOrDefault(x => x.Value == deviceProperty.Value);
				if (driverPropertyParameter != null)
					return driverPropertyParameter.Name;
				return "Неизвестно";
			}
		}
	}
}