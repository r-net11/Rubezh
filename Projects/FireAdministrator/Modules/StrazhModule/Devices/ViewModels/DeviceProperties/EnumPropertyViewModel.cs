using System.Collections.Generic;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;

namespace StrazhModule.ViewModels
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(SKDDriverProperty driverProperty, SKDDevice device)
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

		public List<SKDDriverPropertyParameter> Parameters
		{
			get { return DriverProperty.Parameters; }
		}

		SKDDriverPropertyParameter _selectedParameter;
		public SKDDriverPropertyParameter SelectedParameter
		{
			get { return _selectedParameter; }
			set
			{
				_selectedParameter = value;
				OnPropertyChanged(() => SelectedParameter);
				Save(value.Value);
			}
		}
	}
}