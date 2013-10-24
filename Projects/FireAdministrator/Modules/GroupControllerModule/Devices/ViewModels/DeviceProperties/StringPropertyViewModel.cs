using System;
using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(XDriverProperty driverProperty, XDevice device)
			: base(driverProperty, device)
		{
		    var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
		    if (property != null)
            {
		        if (driverProperty.Name == "IPAddress")
		            _text = property.StringValue;
		        else
                    _text = Convert.ToString(property.Value);
            }
            else
		    {
                if (driverProperty.Name == "IPAddress")
                    _text = driverProperty.StringDefault;
                else
                    _text = Convert.ToString(driverProperty.Default);
		    }
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
                if (DriverProperty.Name == "IPAddress")
                    Save(value);
                else
                    Save(Convert.ToUInt16(value));
			}
		}
	}
}