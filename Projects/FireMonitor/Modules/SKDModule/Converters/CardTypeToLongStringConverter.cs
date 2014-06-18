using System.Windows.Data;
using FiresecAPI.SKD;

namespace SKDModule.Converters
{
	public class CardTypeToLongStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch((CardType)value)
			{
				case CardType.Blocked:
					return "Пропуск заблокирован. С таким пропуском невозможно пройти через турникет";
				case CardType.Constant:
					return "Постоянный пропуск сотрудника с неограниченным действия";
				case CardType.OneTime:
					return "Одноразовый пропуск посетителя со сроком действия в течение одного дня";
				case CardType.Temporary:
					return "Временный пропуск сотрудника с ограниченным сроком действия";
			}
			return null;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}