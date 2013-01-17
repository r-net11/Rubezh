using System.Diagnostics;
using System.Windows.Controls;

namespace DiagnosticsModule.Views
{
    /// <summary>
    /// Логика взаимодействия для ZoneTest.xaml
    /// </summary>
    public partial class ZoneTestView : UserControl
    {
        public ZoneTestView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DiagnosticsModule.swz.Stop();
            Trace.WriteLine("zone" + DiagnosticsModule.swz.ElapsedMilliseconds.ToString());
        }
    }
}