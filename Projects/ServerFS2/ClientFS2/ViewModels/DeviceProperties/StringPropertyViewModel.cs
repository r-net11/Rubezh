using System.Linq;
using FiresecAPI.Models;

namespace ClientFS2.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(DriverProperty driverProperty, Device device)
			: base(driverProperty, device)
		{
		    var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
		    _text = property != null ? property.Value : driverProperty.Default;
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
	}
}