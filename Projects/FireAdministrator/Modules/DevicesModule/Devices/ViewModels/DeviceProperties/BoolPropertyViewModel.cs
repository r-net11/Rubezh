using System.Linq;
using FiresecAPI.Models;

namespace DevicesModule.DeviceProperties
{
	public class BoolPropertyViewModel : BasePropertyViewModel
	{
		public BoolPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_isChecked = property.Value == "1" ? true : false;
			else
				_isChecked = (driverProperty.Default == "1") ? true : false;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
				Save(value ? "1" : "0");
			}
		}
	}
}