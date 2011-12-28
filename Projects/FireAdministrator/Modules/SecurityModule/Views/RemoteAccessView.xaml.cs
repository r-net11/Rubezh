using System.Windows.Controls;
using FiresecAPI.Models;
using SecurityModule.ViewModels;

namespace SecurityModule.Views
{
    public partial class RemoteAccessView : UserControl
    {
        public RemoteAccessView()
        {
            InitializeComponent();
            Update();
        }

        void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            var remoteAccessViewModel = DataContext as RemoteAccessViewModel;
            if (remoteAccessViewModel == null)
                return;

            if (remoteAccessViewModel.RemoteAccessTypes.Find(x => x.IsActive).RemoteAccessType == RemoteAccessType.SelectivelyAllowed)
                _remoteMachinesGrid.Visibility = System.Windows.Visibility.Visible;
            else
                _remoteMachinesGrid.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}