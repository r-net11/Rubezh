using System;
using System.Windows;
using System.Windows.Controls;

namespace PlansModule.Views
{
    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            InitializeComponent();
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _mediaElement.Position = TimeSpan.FromSeconds(11);
            _mediaElement.Play();
        }
    }
}