using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace DevicesModule.DeviceProperties
{
	public class EnumAUPropertyViewModel : BaseAUPropertyViewModel
	{
		public EnumAUPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
			var property = device.SystemAUProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
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

		public List<DriverPropertyParameter> Parameters
		{
			get { return DriverProperty.Parameters; }
		}

		DriverPropertyParameter _selectedParameter;
		public DriverPropertyParameter SelectedParameter
		{
			get { return _selectedParameter; }
			set
			{
				_selectedParameter = value;
				OnPropertyChanged("SelectedParameter");
				Save(value.Value);
			}
		}

		//public override string DeviceAUParameterValue
		//{
		//    get
		//    {
		//        var driverPropertyParameter = DriverProperty.Parameters.FirstOrDefault(x => x.Value == base.DeviceAUParameterValue);
		//        if (driverPropertyParameter != null)
		//            return driverPropertyParameter.Name;
		//        return "Неизвестно";
		//    }
		//}
	}
}