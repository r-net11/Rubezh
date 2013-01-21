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
            //DiagnosticsModule.stopWatch.Stop();
            //Trace.WriteLine("drivers " + DiagnosticsModule.stopWatch.ElapsedMilliseconds.ToString());
            ////Trace.WriteLine(this.Resources.Source.ToString());
        }
    }
}