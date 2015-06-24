using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

namespace GKImitator.ViewModels
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
				if (property != null)
				{
					var driverPropertyParameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
					if (driverPropertyParameter != null)
					{
						SelectedParameter = driverPropertyParameter;
					}
				}
			else
			{
				var enumdriverProperty = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default);
				if (enumdriverProperty != null)
					SelectedParameter = enumdriverProperty;
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
				Save(value.Value);
				OnPropertyChanged(() => SelectedParameter);
			}
		}
	}
}