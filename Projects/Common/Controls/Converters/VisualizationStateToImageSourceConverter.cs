using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infrastructure.Common;

namespace Controls.Converters
{
	public class VisualizationStateToImageSourceConverter : IValueConverter
	{
		private const string VisualizationImageSource = "pack://application:,,,/Controls;component/Images/map{0}.png";
		private static Dictionary<VisualizationState, ImageSource> _map;
		static VisualizationStateToImageSourceConverter()
		{
			_map = new Dictionary<VisualizationState, ImageSource>();
			_map.Add(VisualizationState.Multiple, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Multiple))));
			_map.Add(VisualizationState.NotPresent, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.NotPresent))));
			_map.Add(VisualizationState.Prohibit, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Prohibit))));
			_map.Add(VisualizationState.Single, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Single))));
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var state = (VisualizationState)value;
			if (_map.ContainsKey(state))
				return _map[state];
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}