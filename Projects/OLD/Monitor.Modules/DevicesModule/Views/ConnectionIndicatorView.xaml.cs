using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FiresecClient;
using FSAgentClient;
using Infrastructure.Common.BalloonTrayTip;

namespace DevicesModule.Views
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
			_connectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(IsConnected));
			SafeFiresecService.ConnectionLost += new Action(OnService_ConnectionLost);
			SafeFiresecService.ConnectionAppeared += new Action(OnService_ConnectionAppeared);

			//if (FiresecManager.IsFS2Enabled)
			//{
			//	FS2ClientContract.ConnectionLost += new Action(FS2OrAgent_ConnectionLost);
			//	FS2ClientContract.ConnectionAppeared += new Action(FS2OrAgent_ConnectionAppeared);
			//}
			//else
			{
				FSAgent.ConnectionLost += new Action(FS2OrAgent_ConnectionLost);
				FSAgent.ConnectionAppeared += new Action(FS2OrAgent_ConnectionAppeared);
			}
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
					_connectionControl.ToolTip = "Связь с сервером в норме";
					_connectionControl.Background = Brushes.Transparent;
					_connectionIndicator.Opacity = 0.4;
				}
				else
				{
					var text = "";
					if (!IsServerConnected && IsFS2OrAgentConnected)
					{
						text = "Связь с сервером потеряна";
					}
					if (IsServerConnected && !IsFS2OrAgentConnected)
					{
						text = "Связь с агентом потеряна";
					}
					if (!IsServerConnected && !IsFS2OrAgentConnected)
					{
						text = "Связь с сервером и агентом потеряна";
					}
					_connectionControl.ToolTip = text;
					_connectionControl.SetResourceReference(Border.BackgroundProperty, "HighlightedBackgoundBrush");
					_connectionControl.Background = Brushes.DarkOrange;
					_connectionIndicator.Opacity = 1;
					BalloonHelper.ShowFromMonitor(text);
				}
				_connectionIndicator.BeginAnimation(Image.VisibilityProperty, GetAnimation(value));
			}
		}

		bool IsServerConnected = true;
		bool IsFS2OrAgentConnected = true;
		bool IsAllConnected
		{
			get { return IsServerConnected && IsFS2OrAgentConnected; }
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

		void FS2OrAgent_ConnectionLost()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsFS2OrAgentConnected = false;
				IsConnected = IsAllConnected;
			}));
		}

		void FS2OrAgent_ConnectionAppeared()
		{
			Dispatcher.Invoke(new Action(() =>
			{
				IsFS2OrAgentConnected = true;
				IsConnected = IsAllConnected;
			}));
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
		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}