using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DeviceLibrary;
using Frame = DeviceLibrary.Models.Frame;

namespace DeviceControls
{
    public class StateViewModel
    {
        private readonly List<Frame> _frames;
        private readonly List<Canvas> _canvases;
        private Canvas _canvas = new Canvas();
        private int _tick;
        private int _startTick;
        public StateViewModel(State state, ObservableCollection<Canvas> stateCanvases)
        {
            _canvases = new List<Canvas>();
            foreach (var frame in state.Frames)
            {
                var canvas = Helper.Str2Canvas(frame.Image, frame.Layer);
                _canvases.Add(canvas);
                stateCanvases.Add(canvas);
            }
            _frames = state.Frames;
            if (state.Frames.Count <= 1) return;
            DeviceControl.Tick += OnTick;
        }
        void OnTick()
        {
            _startTick++;
            var frame = _frames[_tick];
            if (_startTick*100 < frame.Duration) return;
            _tick = (_tick + 1) % _frames.Count;
            _startTick = 0;
            frame = _frames[_tick];
            foreach (var canvas in _canvases)
            {
                canvas.Visibility = Visibility.Collapsed;
            }
            _canvases[_tick].Visibility = Visibility.Visible;
        }
    }
}