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
				_text = Convert.ToString(property.StringValue);
			else
				_text = Convert.ToString(driverProperty.Default);
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
				SaveStringValue(value);
				_device.OnChanged();
			}
		}
	}
}