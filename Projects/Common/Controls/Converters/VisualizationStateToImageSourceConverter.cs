using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Controls.Converters
{
	public class VisualizationStateToImageSourceConverter : IValueConverter
	{
		private const string VisualizationImageSource = "pack://application:,,,/Controls;component/Images/map{0}.png";
		private static readonly Dictionary<VisualizationState, ImageSource> Map;

		static VisualizationStateToImageSourceConverter()
		{
			Map = new Dictionary<VisualizationState, ImageSource>
			{
				{
					VisualizationState.Multiple,
					new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int) VisualizationState.Multiple)))
				},
				{
					VisualizationState.NotPresent,
					new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int) VisualizationState.NotPresent)))
				},
				{
					VisualizationState.Prohibit,
					new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int) VisualizationState.Prohibit)))
				},
				{
					VisualizationState.Single,
					new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int) VisualizationState.Single)))
				}
			};
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var state = (VisualizationState)value;
			return Map.ContainsKey(state) ? Map[state] : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}