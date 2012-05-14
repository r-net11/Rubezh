using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
			: base(xDriverProperty, xDevice)
		{
			var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
			if (property != null)
			{
				//_selectedValue = xDriverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value).Name;
			}
			else
			{
				var enumdriverProperty = xDriverProperty.Parameters.FirstOrDefault(x => x.Value.ToString() == xDriverProperty.Default.ToString());
				if (enumdriverProperty != null)
					_selectedValue = enumdriverProperty.Name;
			}
		}

		public List<string> Values
		{
			get
			{
				var values = new List<string>();
				foreach (var propertyParameter in _xDriverProperty.Parameters)
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