using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Infrastructure;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Events;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Views
{
	public partial class GKConnectionIndicatorView : UserControl, INotifyPropertyChanged
	{
		public GKConnectionIndicatorView()
		{
			InitializeComponent();
			DataContext = this;
		}

		void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			OnGKObjectsStateChangedEvent(true);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnGKObjectsStateChangedEvent);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnGKObjectsStateChangedEvent);
		}

		void OnGKObjectsStateChangedEvent(object obj)
		{
			var isConnected = !XManager.Devices.Any(x => x.State.StateClasses.Contains(XStateClass.ConnectionLost));
			IsDeviceConnected = isConnected;
			_connectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsDeviceConnected));
			if (IsDeviceConnected)
				_connectionIndicator.Visibility = Visibility.Visible;
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
					_deviceConnectionControl.ToolTip = "Связь с ГК в норме";
					_deviceConnectionControl.Background = Brushes.Transparent;
					_connectionIndicator.Opacity = 0.4;
				}
				else
				{
					_deviceConnectionControl.ToolTip = "Связь с ГК потеряна";
					_deviceConnectionControl.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_connectionIndicator.Opacity = 1;
					BalloonHelper.ShowFromMonitor("Связь с ГК потеряна");
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
				animation.Duration = TimeSpan.FromSeconds(0.5);
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