using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DeviceControls
{
    public class StateViewModel : IDisposable
    {
        #region Private Fields

        readonly List<FiresecAPI.Models.DeviceLibrary.Frame> _frames;
        readonly List<Canvas> _canvases;
        int _tick;
        int _startTick;

        #endregion

        public StateViewModel(FiresecAPI.Models.DeviceLibrary.State state, ICollection<Canvas> stateCanvases)
        {
            _canvases = new List<Canvas>();
            _frames = state.Frames;
            foreach (var frame in _frames)
            {
                var canvas = Helper.Xml2Canvas(frame.Image, frame.Layer);
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
            if (_frames.Count <= 1) return;
            Timer.Tick += OnTick;
        }

        static StateViewModel()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(94);
            Timer.Start();
        }

        public static DispatcherTimer Timer { get; set; }
        void OnTick(object sender, EventArgs e)
        {
            _startTick++;
            if (_startTick * 100 < _frames[_tick].Duration) return;
            _startTick = 0;
            _tick = (_tick + 1) % _frames.Count;
            foreach (var canvas in _canvases)
            {
                canvas.Visibility = Visibility.Collapsed;
            }
            _canvases[_tick].Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            Timer.Tick -= OnTick;
        }
    }
}