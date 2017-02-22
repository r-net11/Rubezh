using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Common;
using MediaSourcePlayer;
using RVI.MediaSource;
using RviClient.UI.VideoCell;

namespace RviClient.UI
{
	/// <summary>
	/// Interaction logic for VideoCellControl.xaml
	/// </summary>
	public partial class VideoCellControl : IVideoCellControl, IDisposable
	{
		private readonly Grid _topToolbarGrid;
		private readonly Grid _bottomToolbarGrid;

		public static readonly DependencyProperty AlwaysShowToolbarsProperty = DependencyProperty.Register("AlwaysShowToolbars", typeof(bool), typeof(VideoCellControl),
			new PropertyMetadata(false, (o, args) =>
			{
				var videoCellControl = o as IVideoCellControl;
				if (videoCellControl == null)
					return;
				videoCellControl.PlayerPanel.AlwaysShowTollbars = (bool) args.NewValue;
			}));
		public static readonly DependencyProperty TopToolbarTitleProperty = DependencyProperty.Register("TopToolbarTitle", typeof (string), typeof (VideoCellControl));
		public static readonly DependencyProperty BottomToolbarTitleProperty = DependencyProperty.Register("BottomToolbarTitle", typeof(string), typeof(VideoCellControl));
		public static readonly DependencyProperty ShowReconnectButtonProperty = DependencyProperty.Register("ShowReconnectButton", typeof (bool), typeof (VideoCellControl));

		public VideoCellControl()
		{
			InitializeComponent();
			
			MediaPlayer = mediaPlayer;
			_topToolbarGrid = topToolbarGrid;
			_bottomToolbarGrid = bottomToolbarGrid;

			topGrid.Children.Remove(MediaPlayer);
			topGrid.Children.Remove(_topToolbarGrid);
			topGrid.Children.Remove(_bottomToolbarGrid);

			playerPanel.HostedElement = MediaPlayer;
			playerPanel.TopToolbarContent = _topToolbarGrid;
			playerPanel.BottomToolbarContent = _bottomToolbarGrid;

			// При ошибке воспроизведения потока, скрыть ячейку и вместо нее показать кнопку "Перезапустить"
			MediaPlayer.MediaFailed += (sender, args) =>
			{
#if DEBUG
				Logger.Info("Ошибка получения потока воспроизведения");
#endif
				RaiseReconnectEvent();
			};

			MediaPlayer.MediaOpened += (sender, args) =>
			{
#if DEBUG
				Logger.Info("Открыт источник для потока воспроизведения");
#endif
				ShowReconnectButton = false;
			};

			// Выполняем обработчик нажатия на клавижу "Перезапустить"
			reconnectButton.Click += (sender, args) =>
			{
				ShowReconnectButton = false;
				RaiseReconnectEvent();
			};
		}

		private void RaiseReconnectEvent()
		{
			if (ReconnectEvent != null)
			{
#if DEBUG
				Logger.Info("Попытка перезапустить видео ячейку");
#endif
				ReconnectEvent(null, EventArgs.Empty);
			}
		}

		#region <Реализация интерфейса IDisposable>

		public void Dispose()
		{
			MediaPlayer.Dispose();
		}

		#endregion </Реализация интерфейса IDisposable>

		#region <Реализация интерфейса IVideoCellControl>

		/// <summary>
		/// Панель, на которой размещен плеер
		/// </summary>
		public PlayerPanel PlayerPanel
		{
			get { return playerPanel; }
		}

		/// <summary>
		/// Проигрыватель
		/// </summary>
		public MediaPlayer MediaPlayer { get; private set; }


		/// <summary>
		/// Всегда показывать инструментальные панели
		/// </summary>
		public bool AlwaysShowToolbars
		{
			get { return (bool) GetValue(AlwaysShowToolbarsProperty); }
			set { SetValue(AlwaysShowToolbarsProperty, value); }
		}

		/// <summary>
		/// Заголовок, отображаемый на верхней всплывающей панели
		/// </summary>
		public string TopToolbarTitle {
			get { return (string) GetValue(TopToolbarTitleProperty); }
			set { SetValue(TopToolbarTitleProperty, value); }
		}

		/// <summary>
		/// Заголовок, отображаемый на нижней всплывающей панели
		/// </summary>
		public string BottomToolbarTitle
		{
			get { return (string)GetValue(BottomToolbarTitleProperty); }
			set { SetValue(BottomToolbarTitleProperty, value); }
		}

		/// <summary>
		/// Показать кнопку "Перазапустить"
		/// </summary>
		public bool ShowReconnectButton
		{
			get { return (bool) GetValue(ShowReconnectButtonProperty); }
			set { SetValue(ShowReconnectButtonProperty, value); }
		}

		/// <summary>
		/// Событие возникает, когда нажата клавиша "Перезапустить"
		/// </summary>
		public event EventHandler ReconnectEvent;

		#endregion </Реализация интерфейса IVideoCellControl>

		private void SoundOnOfToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			MediaPlayer.IsSoundEnabled = true;
		}

		private void SoundOnOfToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			MediaPlayer.IsSoundEnabled = false;
		}

		private void VideoAspectRatioToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			MediaPlayer.OriginalAspectRatio = true;
		}

		private void VideoAspectRatioToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			MediaPlayer.OriginalAspectRatio = false;
		}
	}
}
