using System;
using System.Windows;
using System.Windows.Controls;

namespace AlarmModule.Views
{
    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _mediaElement.Position = TimeSpan.FromSeconds(11);
            _mediaElement.Play();
        }
    }
}