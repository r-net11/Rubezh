using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Infrastructure;
using Infrastructure.Events;

namespace GKModule.Views
{
	public partial class GKConnectionIndicatorView : UserControl, INotifyPropertyChanged
	{
		public GKConnectionIndicatorView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			OnGKConnectionChanged(true);

			ServiceFactory.Events.GetEvent<GKConnectionChanged>().Unsubscribe(OnGKConnectionChanged);
			ServiceFactory.Events.GetEvent<GKConnectionChanged>().Subscribe(OnGKConnectionChanged);
		}

		void OnGKConnectionChanged(bool isConnected)
		{
			IsDeviceConnected = isConnected;
			_connectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsDeviceConnected));
		}

		bool _isDeviceConnected;
		public bool IsDeviceConnected
		{
			get { return _isDeviceConnected; }
			set
			{
				_isDeviceConnected = value;
				OnPropertyChanged("IsDeviceConnected");
				if (value)
				{
					_deviceConnectionGrid.ToolTip = "Связь с устройствами ГК в норме";
				}
				else
				{
					_deviceConnectionGrid.ToolTip = "Связь с устройствами ГК потеряна";
				}
			}
		}

		ObjectAnimationUsingKeyFrames GetAnimation(bool start)
		{
			var animation = new ObjectAnimationUsingKeyFrames();
			if (!start)
			{

				animation.Duration = TimeSpan.FromSeconds(1.5);
				animation.RepeatBehavior = RepeatBehavior.Forever;
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));
			}
			else
			{
				animation.Duration = Duration.Forever;
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible));
			}
			return animation;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}