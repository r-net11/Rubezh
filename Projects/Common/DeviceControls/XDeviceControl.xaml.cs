using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common;
using FiresecAPI;
using FiresecClient;
using XFiresecAPI;

namespace DeviceControls
{
	public partial class XDeviceControl : INotifyPropertyChanged
	{
		public XDeviceControl()
		{
			InitializeComponent();
			DataContext = this;
		}

		void DeviceControl_Loaded(object sender, RoutedEventArgs e)
		{
			//_canvas.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
		}

		public Guid DriverId { get; set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
			}
		}

		List<XStateViewModel> _stateViewModelList;

		public void Update()
		{
			var libraryXDevice = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == DriverId);
			if (libraryXDevice == null)
			{
				Logger.Error("XDeviceControl.Update libraryXDevice = null " + DriverId.ToString());
				return;
			}

			var resultLibraryStates = new List<LibraryXState>();

			if (_stateViewModelList.IsNotNullOrEmpty())
				_stateViewModelList.ForEach(x => x.Dispose());
			_stateViewModelList = new List<XStateViewModel>();

			var libraryState = libraryXDevice.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == StateClass);
			if (libraryState == null)
			{
				libraryState = libraryXDevice.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
				if (libraryState == null)
				{
					Logger.Error("XDeviceControl.Update libraryState = null " + DriverId.ToString());
					return;
				}
			}

			if (libraryState != null)
			{
				resultLibraryStates.Add(libraryState);
			}

			var canvases = new List<Canvas>();
			foreach (var libraryStates in resultLibraryStates)
			{
				_stateViewModelList.Add(new XStateViewModel(libraryStates, canvases));
			}

			_canvas.Children.Clear();
			foreach (var canvas in canvases)
				_canvas.Children.Add(new Viewbox() { Child = canvas });
		}

		public static FrameworkElement GetDefaultPicture(Guid driverId)
		{
			UIElement content = null;

			var device = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == driverId);
			if (device != null)
			{
				var state = device.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
				Canvas canvas = Helper.Xml2Canvas(state.XFrames[0].Image);
				canvas.Background = Brushes.Transparent;
				content = canvas;
			}
			else
				content = new TextBlock()
				{
					Text = "?",
					Background = Brushes.Transparent
				};
			return new Border()
			{
				BorderThickness = new Thickness(0),
				Background = Brushes.Transparent,
				Child = new Viewbox()
				{
					Child = content
				}
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}