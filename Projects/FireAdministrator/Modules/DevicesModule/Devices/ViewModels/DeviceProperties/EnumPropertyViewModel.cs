using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace DevicesModule.DeviceProperties
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
			{
				_selectedValue = property.Value;// driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value).Name;
			}
			else
			{
				var enumdriverProperty = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default);
				if (enumdriverProperty != null)
					_selectedValue = enumdriverProperty.Name;
			}
		}

		public List<string> Values
		{
			get
			{
				var values = new List<string>();
				foreach (var propertyParameter in _driverProperty.Parameters)
				{
					values.Add(propertyParameter.Name);
				}
				return values;
			}
		}

		string _selectedValue;
		public string SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				_selectedValue = value;
				OnPropertyChanged("SelectedValue");
				Save(value);
			}
		}
	}
}