using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public class BaseStateViewModel<TFrame> : IDisposable
		where TFrame : ILibraryFrame
	{
		private static DispatcherTimer Timer { get; set; }
		private readonly List<TFrame> _frames;
		private readonly List<Canvas> _canvases;
		private int _currentFrameNo;
		private int _currentFrameShownTime;

		static BaseStateViewModel()
		{
			Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromMilliseconds(94);
			Timer.Start();
		}

		public BaseStateViewModel(ILibraryState<TFrame> state, ICollection<Canvas> stateCanvases)
			: this(state.Frames, stateCanvases)
		{
		}
		public BaseStateViewModel(List<TFrame> frames, ICollection<Canvas> stateCanvases)
		{
			_canvases = new List<Canvas>();
			_frames = frames;
			foreach (var frame in _frames)
			{
				var canvas = Helper.Xml2Canvas(frame.Image);
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

		private void OnTick(object sender, EventArgs e)
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