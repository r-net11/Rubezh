using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DiagnosticsModule
{
    [ValueConversion(typeof(string), typeof(DrawingImage))]
    public class StringToDrawingImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ResourceDictionary rd = new ResourceDictionary();
            Uri uri = new Uri("BalloonResources.xaml", UriKind.Relative);
            rd.Source = uri;
            if (!Application.Current.Resources.MergedDictionaries.Contains(rd))
                Application.Current.Resources.MergedDictionaries.Add(rd);
            DrawingImage di = (DrawingImage)Application.Current.Resources["close_cross"];
            return di;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}