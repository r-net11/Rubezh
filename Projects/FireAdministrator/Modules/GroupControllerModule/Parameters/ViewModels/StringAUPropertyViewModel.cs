using System;
using System.Linq;
using FiresecAPI.GK;

namespace GKModule.DeviceProperties
{
	public class StringAUPropertyViewModel : BaseAUPropertyViewModel
	{
		public StringAUPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_text = Convert.ToString(property.Value);
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
				Save(Convert.ToUInt16(value));
			}
		}
	}
}