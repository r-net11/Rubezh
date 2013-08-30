using System;
using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
			: base(xDriverProperty, xDevice)
		{
			var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
			if (property != null)
				_text = Convert.ToString(property.Value);
			else
				_text = Convert.ToString(xDriverProperty.Default);
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
				_xDevice.OnChanged();
			}
		}
	}
}