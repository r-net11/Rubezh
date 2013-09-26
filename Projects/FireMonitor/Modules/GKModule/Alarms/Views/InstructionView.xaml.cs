using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace GKModule.Views
{
	public partial class InstructionView : UserControl
	{
		public InstructionView()
		{
			InitializeComponent();
			mediaElement.Play();
			UpdatePlaying();
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Play();
			UpdatePlaying();
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Stop();
			UpdateNotPlaying();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Pause();
			UpdateNotPlaying();
		}

		private void mediaElement_Unloaded(object sender, RoutedEventArgs e)
		{
			mediaElement.Play();
			UpdatePlaying();
		}

		private void MuteButton_Click(object sender, RoutedEventArgs e)
		{
			if (mediaElement.IsMuted == true)
			{
				mediaElement.IsMuted = false;
				((Image)MuteButton.Content).Source = (ImageSource)new ImageSourceConverter().ConvertFromString("pack://application:,,,/Controls;component/Images/mute.png");
			}
			else
			{
				mediaElement.IsMuted = true;
				((Image)MuteButton.Content).Source = (ImageSource)new ImageSourceConverter().ConvertFromString("pack://application:,,,/Controls;component/Images/sound.png");
			}

		}

		private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
		{
			if (!mediaElement.HasVideo)
				MuteButton.Visibility = Visibility.Collapsed;
		}

		void UpdatePlaying()
		{
			PlayButton.Visibility = Visibility.Collapsed;
			StopButton.Visibility = Visibility.Visible;
			PauseButton.Visibility = Visibility.Visible;
		}

		void UpdateNotPlaying()
		{
			PlayButton.Visibility = Visibility.Visible;
			StopButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Collapsed;
		}
	}
}