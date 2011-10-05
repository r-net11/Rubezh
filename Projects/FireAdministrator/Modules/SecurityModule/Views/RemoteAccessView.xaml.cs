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
        }

        void ListBox_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            SetVisibility();
        }

        void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisibility();
        }

        void SetVisibility()
        {
            var remoteAccessViewModel = DataContext as RemoteAccessViewModel;
            if (remoteAccessViewModel == null)
                return;

            if (remoteAccessViewModel.RemoteAccessTypes.Find(x => x.IsActive).RemoteAccessType == RemoteAccessType.SelectivlyAllowed)
            {
                _computersList.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _computersList.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}