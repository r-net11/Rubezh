using System.Windows;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Events;

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

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<NotificationItemEvent>().Publish(null);
        }

        private void ConflagrationButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ConflagrationItemEvent>().Publish(null);
        }
    }
}