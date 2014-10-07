using System;
using System.Linq;
using FiresecAPI.GK;

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
				if (double.TryParse(value, out doubleValue))
				{
					if (DriverProperty.Multiplier != 0)
						doubleValue *= DriverProperty.Multiplier;
					doubleValue = Math.Min(ushort.MaxValue, doubleValue);
					_text = (ushort)doubleValue;
					Save(_text);
				}
				OnPropertyChanged(()=>Text);
			}
		}
	}
}