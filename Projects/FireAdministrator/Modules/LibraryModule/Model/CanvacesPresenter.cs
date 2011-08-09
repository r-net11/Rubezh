using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LibraryModule.ViewModels
{
    public class CanvasesPresenter
    {
        public CanvasesPresenter(DeviceLibrary.Models.State state)
        {
            Canvases = new List<Canvas>();
            AddCanvacesFrom(state);
            Canvases[0].Visibility = Visibility.Visible;
        }

        List<DeviceLibrary.Models.Frame> _frames = new List<DeviceLibrary.Models.Frame>();
        int _tick = 0;
        int _startTick = 0;
        bool _isTimerSetted = false;

        public List<Canvas> Canvases { get; private set; }
        public DispatcherTimer Timer { get; private set; }

        public void AddCanvacesFrom(DeviceLibrary.Models.State state)
        {
            if (state != null)
            {
                _frames.AddRange(state.Frames);
                SetTimer();

                foreach (var frame in state.Frames)
                {
                    var canvas = ImageConverters.Xml2Canvas(frame.Image, frame.Layer);
                    if (canvas.Children.Count == 0)
                    {
                        canvas.Background = Brushes.White;
                        canvas.Opacity = 0.01;
                    }
                    canvas.Visibility = Visibility.Collapsed;

                    Canvases.Add(canvas);
                }
            }
        }

        void SetTimer()
        {
            if (_frames.Count > 1 && !_isTimerSetted)
            {
                Timer = new DispatcherTimer();
                Timer.Interval = TimeSpan.FromMilliseconds(94);
                Timer.Start();
                Timer.Tick += OnTick;
                _isTimerSetted = true;
            }
        }

        void OnTick(object sender, EventArgs e)
        {
            _startTick++;
            if (_startTick * 100 >= _frames[_tick].Duration)
            {
                _startTick = 0;
                _tick = (_tick + 1) % _frames.Count;
                foreach (var canvas in Canvases)
                {
                    canvas.Visibility = Visibility.Collapsed;
                }
                Canvases[_tick].Visibility = Visibility.Visible;
            }
        }
    }
}