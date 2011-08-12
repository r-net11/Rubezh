using System.Windows.Controls;
using System;
using SoundsModule.ViewModels;

namespace SoundsModule.Views
{
    public partial class SoundsView : UserControl
    {
        public SoundsView()
        {
            InitializeComponent();
            SoundsViewModel.ButtonContentChanged += new Action<bool>(ButtonContentChanged);
        }

        public void ButtonContentChanged(bool isNowPlaying)
        {
            if (isNowPlaying)
            {
                SoundButton.Content = "Остановить";
            }
            else
            {
                SoundButton.Content = "Проверка";
            }
        }
    }
}