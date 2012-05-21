using System.Linq;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ShortPropertyViewModel : BasePropertyViewModel
	{
		public ShortPropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
			: base(xDriverProperty, xDevice)
		{
			var property = xDevice.Properties.FirstOrDefault(x => x.Name == xDriverProperty.Name);
			if (property != null)
				_text = property.Value;
			else
				_text = xDriverProperty.Default;
		}

		short _text;
		public short Text
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