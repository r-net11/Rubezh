using Infrastructure;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GKModule.Views
{
	public partial class GKConnectionIndicatorView : INotifyPropertyChanged
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
			SafeFiresecService.OnConnectionLost += OnService_ConnectionLost;
			SafeFiresecService.OnConnectionAppeared += OnService_ConnectionAppeared;
		}

		void OnService_ConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServerConnected = false;
				IsConnected = IsAllConnected;
			}));
		}

		void OnService_ConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsServerConnected = true;
				IsConnected = IsAllConnected;
			}));
		}

		void OnGKObjectsStateChangedEvent(object obj)
		{
			IsGKDeviceConnected = !GKManager.Devices.Any(x => x.State.StateClasses.Contains(XStateClass.ConnectionLost));
			IsGKDBEqual = !GKManager.Devices.Any(x => x.State.StateClasses.Contains(XStateClass.DBMissmatch));
			IsConnected = IsAllConnected;
		}

		bool IsServerConnected = true;
		bool IsGKDeviceConnected = true;
		bool IsGKDBEqual = true;
		bool IsAllConnected
		{
			get { return IsServerConnected && IsGKDeviceConnected && IsGKDBEqual; }
		}

		bool _isConnected = true;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				OnPropertyChanged("IsConnected");
				if (value)
				{
					_border.ToolTip = "Связь в норме";
					_border.Background = Brushes.Transparent;
					_image.Opacity = 0.4;
					_image.Visibility = Visibility.Visible;
				}
				else
				{
					var strings = new List<string>();
					if (!IsServerConnected)
					{
						strings.Add("Связь с сервером потеряна");
					}
					if (!IsGKDeviceConnected)
					{
						strings.Add("Связь с ГК потеряна");
					}
					if (!IsGKDBEqual)
					{
						strings.Add("База данных прибора не соответствует базе данных ПК");
					}
					var text = string.Join("\n", strings);

					_border.ToolTip = text;
					_border.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_border.Background = Brushes.DarkOrange;
					_image.Opacity = 1;
					BalloonHelper.ShowFromMonitor(text);
				}
				_image.BeginAnimation(VisibilityProperty, GetAnimation(value));
			}
		}

		ObjectAnimationUsingKeyFrames GetAnimation(bool start)
		{
			var animation = new ObjectAnimationUsingKeyFrames();
			if (!start)
			{
				animation.Duration = TimeSpan.FromSeconds(1.5);
				animation.RepeatBehavior = RepeatBehavior.Forever;
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.0))));
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));
			}
			else
			{
				animation.Duration = TimeSpan.FromSeconds(0.5);
				animation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible));
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