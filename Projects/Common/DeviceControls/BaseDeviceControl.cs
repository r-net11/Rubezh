using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using FiresecAPI;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public abstract class BaseDeviceControl<TFrame> : ContentControl, INotifyPropertyChanged
		where TFrame : ILibraryFrame
	{
		private List<BaseStateViewModel<TFrame>> _stateViewModelList;
		private Canvas _canvas;

		public BaseDeviceControl()
		{
			_canvas = new Canvas();
			Content = _canvas;
			Loaded += DeviceControl_Loaded;
			DataContext = this;
		}

		private void DeviceControl_Loaded(object sender, RoutedEventArgs e)
		{
			//_canvas.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
		}

		public Guid DriverId { get; set; }

		public void Update()
		{
			if (_stateViewModelList.IsNotNullOrEmpty())
				_stateViewModelList.ForEach(x => x.Dispose());
			_stateViewModelList = new List<BaseStateViewModel<TFrame>>();
			var states = GetStates();
			var canvases = new List<Canvas>();
			if (states != null)
			{
				var sortedStates = from ILibraryState<TFrame> state in states
								   orderby state.Layer
								   select state;
				foreach (var libraryStates in sortedStates)
					_stateViewModelList.Add(new BaseStateViewModel<TFrame>(libraryStates, canvases));
			}
			_canvas.Children.Clear();
			foreach (var canvas in canvases)
				_canvas.Children.Add(new Viewbox() { Child = canvas });
		}
		protected abstract IEnumerable<ILibraryState<TFrame>> GetStates();

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
