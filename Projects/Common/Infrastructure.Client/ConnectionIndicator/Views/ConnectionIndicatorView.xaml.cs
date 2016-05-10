using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using StrazhAPI.Events;
using FiresecClient;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Services;

// ReSharper disable once CheckNamespace
namespace Infrastructure.Client.ConnectionIndicator.Views
{
	/// <summary>
	/// Interaction logic for ConnectionIndicatorView.xaml
	/// </summary>
	public partial class ConnectionIndicatorView : INotifyPropertyChanged
	{
		public static readonly DependencyProperty ShowBaloonProperty = DependencyProperty.Register("ShowBaloon", typeof(bool), typeof(ConnectionIndicatorView), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BaloonHeaderTextProperty = DependencyProperty.Register("BaloonHeaderText", typeof(string), typeof(ConnectionIndicatorView), new UIPropertyMetadata(null));

		public bool ShowBaloon
		{
			get { return (bool) GetValue(ShowBaloonProperty); }
			set { SetValue(ShowBaloonProperty, value);}
		}

		public string BaloonHeaderText
		{
			get { return (string) GetValue(BaloonHeaderTextProperty); }
			set { SetValue(BaloonHeaderTextProperty, value); }
		}

		public ConnectionIndicatorView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// В режиме дизайнера ничего не делаем
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			OnSKDObjectsStateChangedEvent(true);
			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Unsubscribe(OnSKDObjectsStateChangedEvent);
			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Subscribe(OnSKDObjectsStateChangedEvent);
			SafeFiresecService.ConnectionLost += OnService_ConnectionLost;
			SafeFiresecService.ConnectionAppeared += OnService_ConnectionAppeared;
		}

		void OnService_ConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				_isServerConnected = false;
				IsConnected = IsAllConnected;
			}));
		}

		void OnService_ConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				_isServerConnected = true;
				IsConnected = IsAllConnected;
			}));
		}

		void OnSKDObjectsStateChangedEvent(object obj)
		{
			IsConnected = IsAllConnected;
		}

		bool _isServerConnected = true;
		bool IsAllConnected
		{
			get { return _isServerConnected; }
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
					if (!_isServerConnected)
					{
						strings.Add("Связь с сервером потеряна");
					}
					var text = string.Join("\n", strings);

					_border.ToolTip = text;
					_border.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_border.Background = Brushes.DarkOrange;
					_image.Opacity = 1;
					if (ShowBaloon)
						BalloonHelper.Show(BaloonHeaderText, text, Brushes.Black, Brushes.Cornsilk);
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
