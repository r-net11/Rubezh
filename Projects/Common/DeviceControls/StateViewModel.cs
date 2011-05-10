using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using DeviceLibrary;
using Frame = DeviceLibrary.Models.Frame;

namespace DeviceControls
{
    public class StateViewModel : IDisposable
    {
        private readonly List<Frame> _frames;
        private readonly ObservableCollection<Canvas> _stateCanvases;
        private readonly DispatcherTimer _timer;
        private Canvas _canvas = new Canvas();
        private int _tick;
        public StateViewModel(State state, ObservableCollection<Canvas> stateCanvases)
        {
            _stateCanvases = stateCanvases;
            _frames = state.Frames;
            var frame = _frames[0];
            if (state.Frames.Count > 1)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += TimerTick;
                _timer.Start();
            }
            else
            {
                Functions.Draw(_stateCanvases, ref _canvas, frame.Image, frame.Layer);
            }
        }
        private void TimerTick(object sender, EventArgs e)
        {
            var frame = _frames[_tick];
            _timer.Interval = TimeSpan.FromMilliseconds(frame.Duration);
            Functions.Draw(_stateCanvases, ref _canvas, frame.Image, frame.Layer);
            _tick = (_tick + 1)%_frames.Count;
        }
        #region IDisposable Members
        public void Dispose()
        {
            if (_timer != null)
                _timer.Stop();
        }
        #endregion
    }
}