using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;

namespace ClientFS2.ViewModels
{
	public class EnumPropertyViewModel : BasePropertyViewModel
	{
		public EnumPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
			if (driverProperty == null)
			{
				Logger.Error("EnumPropertyViewModel driverProperty = null");
				return;
			}
			if (device == null)
			{
				Logger.Error("EnumPropertyViewModel device = null");
				return;
			}
			if (driverProperty.Name == null)
			{
				Logger.Error("EnumPropertyViewModel driverProperty.Name = null");
				return;
			}
			if (device.Properties == null)
			{
				Logger.Error("EnumPropertyViewModel device.Properties = null");
				return;
			}
			if (driverProperty.Parameters == null)
			{
				Logger.Error("EnumPropertyViewModel driverProperty.Parameters = null");
				return;
			}

			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
			{
				if (property.Value == null)
				{
					Logger.Error("EnumPropertyViewModel property.Value = null " + device.PresentationAddressAndName + " - " + driverProperty.Name);
					return;
				}
				var driverPropertyParameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
				if (driverPropertyParameter != null)
				{
					_selectedParameter = driverPropertyParameter;
				}
			}
			else
			{
				if (driverProperty.Default == null)
				{
					Logger.Error("EnumPropertyViewModel driverProperty.Default = null " + device.Driver.ShortName + "." + driverProperty.Name);
					return;
				}
				var enumdriverProperty = driverProperty.Parameters.FirstOrDefault(x => x.Value == driverProperty.Default);
				if (enumdriverProperty != null)
					_selectedParameter = enumdriverProperty;
				
			}
		}

		public List<DriverPropertyParameter> Parameters
		{
			get
			{
				return DriverProperty.Parameters;
			}
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

		//public List<string> Values
		//{
		//    get
		//    {
		//        var values = new List<string>();
		//        foreach (var propertyParameter in _driverProperty.Parameters)
		//        {
		//            values.Add(propertyParameter.Name);
		//        }
		//        return values;
		//    }
		//}

		//string _selectedValue;
		//public string SelectedValue
		//{
		//    get { return _selectedValue; }
		//    set
		//    {
		//        _selectedValue = value;
		//        OnPropertyChanged("SelectedValue");
		//        Save(value);
		//    }
		//}
	}
}