using System.Diagnostics;
using System.Windows.Controls;

namespace DiagnosticsModule.Views
{
    /// <summary>
    /// Логика взаимодействия для DeviceIconTestView.xaml
    /// </summary>
    public partial class DeviceIconTestView : UserControl
    {
        public DeviceIconTestView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DiagnosticsModule.swd.Stop();
            Trace.WriteLine("driver" + DiagnosticsModule.swd.ElapsedMilliseconds.ToString());
        }
    }

    //[ValueConversion(typeof(string), typeof(Geometry))]
    //public class StringToGeometryConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        Geometry res = Geometry.Parse((string)value);
    //        return res;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}