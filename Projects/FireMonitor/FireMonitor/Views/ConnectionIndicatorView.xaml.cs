using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;

namespace FireMonitor.Views
{
	public partial class ConnectionIndicatorView : UserControl, INotifyPropertyChanged
	{
		public ConnectionIndicatorView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			IsServiceConnected = true;
			_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			SafeFiresecService.ConnectionLost += new Action(OnConnectionLost);
			SafeFiresecService.ConnectionAppeared += new Action(OnConnectionAppeared);

			OnDeviceStateChanged(Guid.Empty);

			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Unsubscribe(OnDeviceStateChanged);
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
		}

		void OnDeviceStateChanged(Guid deviceUID)
		{
			IsDeviceConnected = !HasLostDevices();
			_deviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsDeviceConnected));
		}

		bool HasLostDevices()
		{
			foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
			{
				foreach (var state in deviceState.States)
				{
					if (state.DriverState.Name.Contains("Потеря связи") || state.DriverState.Name.Contains("Связь с панелью потеряна"))
						return true;

				}
			}
			return false;
		}

		bool _isServiceConnected;
		public bool IsServiceConnected
		{
			get { return _isServiceConnected; }
			set
			{
				_isServiceConnected = value;
				OnPropertyChanged("IsServiceConnected");
				if (value)
				{
					_serviceConnectionGrid.ToolTip = "Связь с сервером в норме";
				}
				else
				{
					_serviceConnectionGrid.ToolTip = "Связь с сервером потеряна";
				}
			}
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
					_deviceConnectionGrid.ToolTip = "Связь с устройствами в норме";
				}
				else
				{
					_deviceConnectionGrid.ToolTip = "Связь с устройствами потеряна";
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

		void OnConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServiceConnected = false;
				_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			}));
		}

		void OnConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServiceConnected = true;
				_serviceConnectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsServiceConnected));
			}));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}