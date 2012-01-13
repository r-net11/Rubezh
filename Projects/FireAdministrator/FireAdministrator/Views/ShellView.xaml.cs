using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Common;
using DevicesModule.Views;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure;
using FireAdministrator.ViewModels;

namespace FireAdministrator.Views
{
    public partial class ShellView : Window, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
            LoadZonesSelectionViewCustomLocation();
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

        void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                ChangeMaximizedState();
        }

        void ChangeMaximizedState()
        {
            if (Application.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        void OnMaximize(object sender, RoutedEventArgs e)
        {
            ChangeMaximizedState();
        }

        void OnShowHelp(object sender, RoutedEventArgs e)
        {
            var helperPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec\FS_CL_ADMIN.HLP");
            Process.Start(helperPath);
        }

        void OnShowAbout(object sender, RoutedEventArgs e)
        {
            var aboutViewModel = new AboutViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(aboutViewModel);
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

        void Window_Closing(object sender, CancelEventArgs e)
        {
            AlarmPlayerHelper.Dispose();

            if (DevicesModule.DevicesModule.HasChanges || PlansModule.PlansModule.HasChanges ||
                SoundsModule.SoundsModule.HasChanges ||
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

        void Window_Closed(object sender, System.EventArgs e)
        {
            SaveZonesSelectionViewCustomLocation();
            FiresecManager.Disconnect();
            Application.Current.Shutdown();
        }

        void LoadZonesSelectionViewCustomLocation()
        {
            try
            {
                ZonesSelectionView.CustomWidth = double.Parse(ConfigurationManager.AppSettings["ZonesSelectionViewCustomWidth"]);
                ZonesSelectionView.CustomHeight = double.Parse(ConfigurationManager.AppSettings["ZonesSelectionViewCustomHeight"]);
                ZonesSelectionView.CustomLeft = double.Parse(ConfigurationManager.AppSettings["ZonesSelectionViewCustomLeft"]);
                ZonesSelectionView.CustomTop = double.Parse(ConfigurationManager.AppSettings["ZonesSelectionViewCustomTop"]);
                //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //foreach (KeyValueConfigurationElement property in config.AppSettings.Settings)
                //{
                //    switch (property.Key)
                //    {
                //        case "ZonesSelectionViewCustomWidth":
                //            ZonesSelectionView.CustomWidth = double.Parse(property.Value);
                //            break;
                //        case "ZonesSelectionViewCustomHeight":
                //            ZonesSelectionView.CustomHeight = double.Parse(property.Value);
                //            break;
                //        case "ZonesSelectionViewCustomLeft":
                //            ZonesSelectionView.CustomLeft = double.Parse(property.Value);
                //            break;
                //        case "ZonesSelectionViewCustomTop":
                //            ZonesSelectionView.CustomTop = double.Parse(property.Value);
                //            break;
                //        default:
                //            break;
                //    }
                //}
            }
            catch { }
        }

        void SaveZonesSelectionViewCustomLocation()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                foreach (KeyValueConfigurationElement property in config.AppSettings.Settings)
                {
                    switch (property.Key)
                    {
                        case "ZonesSelectionViewCustomWidth":
                            property.Value = ZonesSelectionView.CustomWidth.ToString();
                            break;
                        case "ZonesSelectionViewCustomHeight":
                            property.Value = ZonesSelectionView.CustomHeight.ToString();
                            break;
                        case "ZonesSelectionViewCustomLeft":
                            property.Value = ZonesSelectionView.CustomLeft.ToString();
                            break;
                        case "ZonesSelectionViewCustomTop":
                            property.Value = ZonesSelectionView.CustomTop.ToString();
                            break;
                        default:
                            break;
                    }
                }

                config.Save();
            }
            catch { }
        }
    }
}