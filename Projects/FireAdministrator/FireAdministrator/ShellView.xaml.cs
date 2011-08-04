using System.ComponentModel;
using System.Windows;
using Infrastructure.Common;
using FiresecClient;

namespace FireAdministrator
{
    public partial class ShellView : Window, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public IViewPart MainContent
        {
            get { return _mainRegionHost.Content as IViewPart; }
            set { _mainRegionHost.DataContext = _mainRegionHost.Content = value; }
        }

        public object Menu
        {
            get { return _menu.Content; }
            set { _menu.DataContext = _menu.Content = value; }
        }

        public object ValidatoinArea
        {
            get { return _validationArea.Content; }
            set { _validationArea.DataContext = _validationArea.Content = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DevicesModule.DevicesModule.HasChanges == false)
            {
                return;
            }

            var result = MessageBox.Show("Сохранить конфигурацию?", "Выход", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (result == MessageBoxResult.Yes)
            {
                DevicesModule.DevicesModule.HasChanges = true;
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}
