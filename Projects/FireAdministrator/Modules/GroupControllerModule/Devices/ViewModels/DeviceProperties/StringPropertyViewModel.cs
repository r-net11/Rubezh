using System;
using System.Linq;
using Infrastructure;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(XDriverProperty driverProperty, XDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			_text = property != null ? property.StringValue : driverProperty.StringDefault;
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
                //if (DriverProperty.Name == "IPAddress")
				Save(value);
				//else
				//    Save(Convert.ToUInt16(value));
			}
		}

		protected void Save(string value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.GKChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.StringValue = value;
			}
			else
			{
				var newProperty = new XProperty()
				{
					Name = DriverProperty.Name,
					StringValue = value
				};
				Device.Properties.Add(newProperty);
			}
			UpdateDeviceParameterMissmatchType();
			Device.OnChanged();
		}
	}
}