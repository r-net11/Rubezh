using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using GKModule.ViewModels;

namespace GKModule.Views
{
	public partial class InstructionView : UserControl
	{
		DispatcherTimer timer;

		public InstructionView()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var dataContext = (InstructionViewModel)DataContext;
			if (dataContext.Instruction.HasMedia)
			{
				mediaElement.Play();
				UpdateVisibilityPlaying();
			}
			else
			{
				videoBorder.Visibility = Visibility.Collapsed;
				videoRow.Height = new GridLength(0);
			}
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Play();
			UpdateVisibilityPlaying();
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Stop();
			UpdateVisibilityNotPlaying();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			mediaElement.Pause();
			UpdateVisibilityNotPlaying();
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
			{
				MuteButton.Visibility = Visibility.Collapsed;
				videoBorder.Visibility = Visibility.Collapsed;
				videoRow.Height = new GridLength(0);
			}

			slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(100);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			if (!slider.IsMouseOver)
				slider.Value = mediaElement.Position.TotalSeconds;
		}

		private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
		{
			mediaElement.Stop();
			UpdateVisibilityNotPlaying();
		}

		void UpdateVisibilityPlaying()
		{
			PlayButton.Visibility = Visibility.Collapsed;
			StopButton.Visibility = Visibility.Visible;
			PauseButton.Visibility = Visibility.Visible;
		}

		void UpdateVisibilityNotPlaying()
		{
			PlayButton.Visibility = Visibility.Visible;
			StopButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Collapsed;
		}

		private void slider_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
		{
			TimeSpan time = new TimeSpan(0, 0, Convert.ToInt32(Math.Round(slider.Value)));
			mediaElement.Position = time;
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			mediaElement.Stop();
		}
	}
}