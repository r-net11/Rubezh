using System.ComponentModel;
using System.Windows;
using Infrastructure.Common;
using FiresecClient;
using CustomWindow;
using System.Windows.Controls;

namespace FireAdministrator
{
    public partial class ShellView : EssentialWindow, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override Decorator GetWindowButtonsPlaceholder()
        {
            return WindowButtonsPlaceholder;
        }

        private void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
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

        private void EssentialWindow_Closing(object sender, CancelEventArgs e)
        {
            if ((DevicesModule.DevicesModule.HasChanges == false)
                && (SoundsModule.SoundsModule.HasChanged == false))
            {
                return;
            }

            MessageBoxResult result;

            if (SoundsModule.SoundsModule.HasChanged)
            {
                result = MessageBox.Show("Настройки звуков изменены. Желаете сохранить изменения?",
                    "Подтверждение", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SoundsModule.SoundsModule.HasChanged = false;
                        SoundsModule.SoundsModule.Save();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                    default:
                        break;
                }
            }

            if (DevicesModule.DevicesModule.HasChanges)
            {
                result = MessageBox.Show("Сохранить конфигурацию?", "Выход", MessageBoxButton.YesNoCancel);
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

        }

        private void EssentialWindow_Closed(object sender, System.EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}
