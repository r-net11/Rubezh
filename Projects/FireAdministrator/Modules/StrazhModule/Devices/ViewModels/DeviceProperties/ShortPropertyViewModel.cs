using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;

namespace StrazhModule.ViewModels
{
	public class ShortPropertyViewModel : BasePropertyViewModel
	{
		public ShortPropertyViewModel(SKDDriverProperty driverProperty, SKDDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_text = property.Value;
			else
				_text = driverProperty.Default;
		}

		int _text;
		public int Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
				Save(value);
			}
		}
	}
}