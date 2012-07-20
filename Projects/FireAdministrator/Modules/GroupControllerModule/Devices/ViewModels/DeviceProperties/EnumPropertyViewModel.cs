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
				var parameter = xDriverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
				if (parameter != null)
					_selectedValue = parameter.Name;
			}
			else
			{
				var enumDriverProperty = xDriverProperty.Parameters.FirstOrDefault(x => x.Value == xDriverProperty.Default);
				if (enumDriverProperty != null)
					_selectedValue = enumDriverProperty.Name;
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

				var shortValue = _xDriverProperty.Parameters.FirstOrDefault(x => x.Name == value).Value;
				Save(shortValue);
			}
		}
	}
}