using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (App.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
                else
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            }
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
                FiltersModule.FilterModule.HasChanges || LibraryModule.LibraryModule.HasChanges ||
                InstructionsModule.InstructionsModule.HasChanges || SecurityModule.SecurityModule.HasChanges)
            {
                var result = DialogBox.DialogBox.Show("Сохранить изменения в настройках?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FiresecManager.SetConfiguration();
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