using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(XDriverProperty driverProperty, XDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
			{
				var parameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
				if (parameter != null)
					_selectedValue = parameter.Name;
			}
			else
			{
				var enumDriverProperty = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default);
				if (enumDriverProperty != null)
					_selectedValue = enumDriverProperty.Name;
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

				var shortValue = _driverProperty.Parameters.FirstOrDefault(x => x.Name == value).Value;
				Save(shortValue);
			}
		}
	}
}