using System.Linq;
using FiresecAPI.Models;

namespace DevicesModule.DeviceProperties
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_text = property.Value;
			else
				_text = driverProperty.Default;
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
				Save(value);
			}
		}

		public string DeviceText
		{
			get { return DeviceValue; }
		}

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}
	}
}