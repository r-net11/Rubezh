using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Common;
using FiresecClient;
using Infrastructure.Common;

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

        void EssentialWindow_Closing(object sender, CancelEventArgs e)
        {
            AlarmPlayerHelper.Dispose();

            if (DevicesModule.DevicesModule.HasChanges || SoundsModule.SoundsModule.HasChanged ||
                FiltersModule.FilterModule.HasChanged)
            {
                MessageBoxResult result;
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
                    }
                }

                if (FiltersModule.FilterModule.HasChanged)
                {
                    result = MessageBox.Show("Фильтры журнала изменены. Желаете сохранить изменения?",
                        "Подтверждение", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            FiltersModule.FilterModule.HasChanged = false;
                            FiltersModule.FilterModule.Save();
                            break;

                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
            }
            MessageBox.Show(@"I promise I will not ship slow code. Speed is a feature I care about. Every day I will pay attention to the performance of my code. I will regularly and
            methodically measure its speed and size. I will learn, build, or buy the tools I need to do this. It's my responsibility.");
        }

        void EssentialWindow_Closed(object sender, System.EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}