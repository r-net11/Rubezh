using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Common;
using CustomWindow;
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

        void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
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

            if (DevicesModule.DevicesModule.HasChanges || SoundsModule.SoundsModule.HasChanges ||
                FiltersModule.FilterModule.HasChanged || LibraryModule.LibraryModule.HasChanges)
            {
                var result = MessageBox.Show("Настройки изменены. Желаете сохранить изменения?", "Подтверждение", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SoundsModule.SoundsModule.Save();
                        FiresecManager.SetConfiguration();

                        SoundsModule.SoundsModule.HasChanges = false;
                        DevicesModule.DevicesModule.HasChanges = false;
                        FiltersModule.FilterModule.HasChanged = false;
                        LibraryModule.LibraryModule.HasChanges = false;
                        return;
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        void EssentialWindow_Closed(object sender, System.EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}