using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FiresecAPI.Models;
using XFiresecAPI;

namespace DeviceControls
{
	public class FramesControl : Decorator
	{
		private DispatcherTimer _timer;
		private List<FrameworkElement> _canvases;
		private List<TimeSpan> _times;
		private int _index;
		public FramesControl(List<LibraryFrame> frames)
		{
			_index = -1;
			if (frames.Count == 0)
				Child = DevicePictureCache.EmptyPicture;
			else if (frames.Count == 1)
				Child = Helper.GetVisual(frames[0].Image);
			else if (frames.Count > 1)
			{
				_canvases = new List<FrameworkElement>();
				_times = new List<TimeSpan>();
				foreach (var frame in frames)
				{
					_canvases.Add(Helper.GetVisual(frame.Image));
					_times.Add(TimeSpan.FromMilliseconds(frame.Duration));
				}
				_timer = new DispatcherTimer();
				_timer.Tick += (s, e) => UpdateTimer();
				UpdateTimer();
			}
		}
		public FramesControl(List<LibraryXFrame> frames)
		{
			_index = -1;
			if (frames.Count == 0)
				Child = DevicePictureCache.EmptyPicture;
			else if (frames.Count == 1)
				Child = Helper.GetVisual(frames[0].Image);
			else if (frames.Count > 1)
			{
				_canvases = new List<FrameworkElement>();
				_times = new List<TimeSpan>();
				foreach (var frame in frames)
				{
					_canvases.Add(Helper.GetVisual(frame.Image));
					_times.Add(TimeSpan.FromMilliseconds(frame.Duration));
				}
				_timer = new DispatcherTimer();
				_timer.Tick += (s, e) => UpdateTimer();
				UpdateTimer();
			}
		}
		public FramesControl(List<SKDLibraryFrame> frames)
		{
			_index = -1;
			if (frames.Count == 0)
				Child = DevicePictureCache.EmptyPicture;
			else if (frames.Count == 1)
				Child = Helper.GetVisual(frames[0].Image);
			else if (frames.Count > 1)
			{
				_canvases = new List<FrameworkElement>();
				_times = new List<TimeSpan>();
				foreach (var frame in frames)
				{
					_canvases.Add(Helper.GetVisual(frame.Image));
					_times.Add(TimeSpan.FromMilliseconds(frame.Duration));
				}
				_timer = new DispatcherTimer();
				_timer.Tick += (s, e) => UpdateTimer();
				UpdateTimer();
			}
		}

		private void UpdateTimer()
		{
			_timer.Stop();
			_index = (_index + 1) % _times.Count;
			Child = _canvases[_index];
			_timer.Interval = _times[_index];
			_timer.Start();
		}
	}
}
