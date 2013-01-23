using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using XFiresecAPI;

namespace DeviceControls
{
	public class XStateViewModel : IDisposable
	{
		readonly List<LibraryXFrame> _frames;
		readonly List<Canvas> _canvases;
		int _currentFrameNo;
		int _currentFrameShownTime;
		public static DispatcherTimer Timer { get; set; }

		public XStateViewModel(LibraryXState state, ICollection<Canvas> stateCanvases)
		{
			_canvases = new List<Canvas>();
			_frames = state.XFrames;
			foreach (var xframe in _frames)
			{
				var canvas = Helper.Xml2Canvas(xframe.Image);
				if (canvas.Children.Count == 0)
				{
					canvas.Background = Brushes.White;
					canvas.Opacity = 0.01;
				}
				canvas.Visibility = Visibility.Collapsed;
				_canvases.Add(canvas);
				stateCanvases.Add(canvas);
			}
			if (_canvases.Count > 0)
				_canvases[0].Visibility = Visibility.Visible;
			if (_frames.Count <= 1)
				return;
			Timer.Tick += OnTick;
		}

		static XStateViewModel()
		{
			Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromMilliseconds(94);
			Timer.Start();
		}

		void OnTick(object sender, EventArgs e)
		{
			try
			{
				_currentFrameShownTime += 100;
				if (_currentFrameShownTime < _frames[_currentFrameNo].Duration)
					return;
				_currentFrameShownTime = 0;

				_currentFrameNo = (_currentFrameNo + 1) % _frames.Count;
				foreach (var canvas in _canvases)
				{
					canvas.Visibility = Visibility.Collapsed;
				}
				if (_canvases.Count > _currentFrameNo)
					_canvases[_currentFrameNo].Visibility = Visibility.Visible;

			}
			catch { return; }
		}

		public void Dispose()
		{
			Timer.Tick -= OnTick;
		}
	}
}