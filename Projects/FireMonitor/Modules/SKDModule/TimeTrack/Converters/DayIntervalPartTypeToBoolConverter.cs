using System;
using System.Globalization;
using System.Windows.Data;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	/// <summary>
	/// Конвертер из DayIntervalPartType в bool и обратно.
	/// Применим пока DayIntervalPartType имеет возможные значения {Work, Break}
	/// </summary>
	public class DayIntervalPartTypeToBoolConverter : IValueConverter
	{
		/// <summary>
		/// Возвращает false для DayIntervalPartType.Work
		/// Возвращает true для DayIntervalPartType.Break
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (DayIntervalPartType) value == DayIntervalPartType.Break;
		}

		/// <summary>
		/// Возвращает DayIntervalPartType.Work для false
		/// Возвращает DayIntervalPartType.Break для true
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? DayIntervalPartType.Break : DayIntervalPartType.Work;
		}
	}
}