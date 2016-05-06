using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Controls.Converters
{
	public class PlanElementUIDsToImageSourceConverter : IValueConverter
	{
		private const string VisualizationImageSource = "pack://application:,,,/Controls;component/Images/map{0}.png";
		private static Dictionary<VisualizationState, ImageSource> _map;
		static PlanElementUIDsToImageSourceConverter()
		{
			_map = new Dictionary<VisualizationState, ImageSource>();
			_map.Add(VisualizationState.Multiple, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Multiple))));
			_map.Add(VisualizationState.NotPresent, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.NotPresent))));
			_map.Add(VisualizationState.Prohibit, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Prohibit))));
			_map.Add(VisualizationState.Single, new BitmapImage(new Uri(string.Format(VisualizationImageSource, (int)VisualizationState.Single))));
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var planElementUIDs = (List<Guid>)value;
			var visualizationState = planElementUIDs.Count == 0 ? VisualizationState.NotPresent : (planElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			if (_map.ContainsKey(visualizationState))
				return _map[visualizationState];
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}