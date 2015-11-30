using System;
using System.Linq;
using RubezhAPI.GK;
using System.Globalization;

namespace GKModule.ViewModels
{
	public class ShortPropertyViewModel : BasePropertyViewModel
	{
		public ShortPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_text = property.Value;
			else
				_text = driverProperty.Default;
		}

		ushort _text;
		public string Text
		{
			get
			{
				double result = _text;
				if (DriverProperty.Multiplier != 0)
					result /= DriverProperty.Multiplier;
				return result.ToString();
			}
			set
			{
				double doubleValue = -1;
				if (double.TryParse(value.Replace(".", ","),
									NumberStyles.Number,
									CultureInfo.CreateSpecificCulture("ru-RU"),
									out doubleValue))
				{
					if (DriverProperty.Multiplier != 0)
						doubleValue *= DriverProperty.Multiplier;
					if (doubleValue < DriverProperty.Min)
					{
						doubleValue = DriverProperty.Min;
					}
					doubleValue = Math.Min(DriverProperty.Max, doubleValue);
					_text = (ushort)doubleValue;
					Save(_text);
				}
				OnPropertyChanged(() => Text);
			}
		}
	}
}