using System;
using System.Linq;
using RubezhAPI.GK;
using System.Globalization;

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
			get
			{
				if (DriverProperty.Multiplier != 0)
				{
					double doubleValue = -1;
					if (double.TryParse(_text.Replace(".", ","),
									NumberStyles.Number,
									CultureInfo.CreateSpecificCulture("ru-RU"),
									out doubleValue))
					{
						return (doubleValue / DriverProperty.Multiplier).ToString();
					}
				}
				return _text;
			}
			set
			{

                if (DriverProperty.Multiplier != 0)
				{
					double doubleValue = -1;
					if (double.TryParse(value.Replace(".", ","),
									NumberStyles.Number,
									CultureInfo.CreateSpecificCulture("ru-RU"),
									out doubleValue))
					{
						value = (doubleValue * DriverProperty.Multiplier).ToString();
					}
				}

				UInt16 result = DriverProperty.Min;
                if (UInt16.TryParse(value, out result))
                {
                    if (result <= DriverProperty.Max)
                    {
                        if (result < DriverProperty.Min)
                        {
                            result = DriverProperty.Min;
                        }
                    }
                    else
                    {
                        result = DriverProperty.Max;
                    }                    
                }
                else
                {
                    result = DriverProperty.Max;
                }
                Save(result);
                _text = result.ToString();
                OnPropertyChanged(() => Text);
            }
		}
	}
}