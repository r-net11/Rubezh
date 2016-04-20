using System.Linq;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class ShortPropertyViewModel : BasePropertyViewModel
	{
		public ShortPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
			{
				Text = DriverProperty.Multiplier != 0 ? (property.Value / DriverProperty.Multiplier).ToString() : property.Value.ToString();
			}
			else
				Text = driverProperty.Default.ToString();
		}

		double _text;
		public string Text
		{
			get
			{
				double result = _text;
				return result.ToString();
			}
			set
			{
				double doubleValue;
				if (double.TryParse(value, out doubleValue))
				{
					_text = doubleValue;
					Save(DriverProperty.Multiplier != 0 ? (ushort)(doubleValue * DriverProperty.Multiplier) : (ushort)doubleValue);
				}
				OnPropertyChanged(() => Text);
			}
		}
	}
}