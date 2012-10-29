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
        readonly List<LibraryXFrame> _xframes;
        readonly List<Canvas> _canvases;
        int _currentFrameNo;
        int _currentFrameShownTime;
        public static DispatcherTimer Timer { get; set; }

        public XStateViewModel(LibraryXState xstate, ICollection<Canvas> stateCanvases)
        {
            _canvases = new List<Canvas>();
            _xframes = xstate.XFrames;
            foreach (var xframe in _xframes)
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
            _canvases[0].Visibility = Visibility.Visible;
            if (_xframes.Count <= 1)
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
                if (_currentFrameShownTime < _xframes[_currentFrameNo].Duration)
                    return;
                _currentFrameShownTime = 0;

                _currentFrameNo = (_currentFrameNo + 1) % _xframes.Count;
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