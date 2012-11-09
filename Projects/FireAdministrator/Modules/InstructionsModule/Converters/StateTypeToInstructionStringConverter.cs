using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using FiresecAPI;

namespace InstructionsModule.Converters
{
	public class StateTypeToInstructionStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			StateType stateType = (StateType)value;
			switch(stateType)
			{
				case StateType.Fire:
					return "Пожар или тревога";

				case StateType.Attention:
					return "Внимание";

				case StateType.Failure:
					return "Неисправность";

				case StateType.Service:
					return "Требуется обслуживание";

				case StateType.Off:
					return "Отключено";

				case StateType.Info:
					return "Информация";
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}