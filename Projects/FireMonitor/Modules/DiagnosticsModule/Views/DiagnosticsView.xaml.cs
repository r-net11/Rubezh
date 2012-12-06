using System.Windows;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.BalloonTrayTip;

namespace DiagnosticsModule.Views
{
    public partial class DiagnosticsView : UserControl
    {
        public DiagnosticsView()
        {
            InitializeComponent();
        }

        private void WarningButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<WarningItemEvent>().Publish(null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}