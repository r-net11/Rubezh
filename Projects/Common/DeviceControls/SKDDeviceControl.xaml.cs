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
	public partial class SKDDeviceControl : INotifyPropertyChanged
	{
		public SKDDeviceControl()
		{
			InitializeComponent();
			DataContext = this;
		}

		void SKDDeviceControl_Loaded(object sender, RoutedEventArgs e)
		{
			//_canvas.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
		}

		public Guid DriverId { get; set; }
		List<SKDStateViewModel> _stateViewModelList;

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
			}
		}

		public void Update()
		{
			var libraryDevice = SKDManager.SKDLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
			if (libraryDevice == null)
			{
				Logger.Error("SKDDeviceControl.Update libraryDevice = null " + DriverId.ToString());
				return;
			}

			var resultLibraryStates = new List<SKDLibraryState>();

			if (_stateViewModelList.IsNotNullOrEmpty())
				_stateViewModelList.ForEach(x => x.Dispose());
			_stateViewModelList = new List<SKDStateViewModel>();

			var libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateClass == StateClass);
			if (libraryState == null)
			{
				libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateClass == XStateClass.No);
				if (libraryState == null)
				{
					Logger.Error("SKDDeviceControl.Update libraryState = null " + DriverId.ToString());
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
				_stateViewModelList.Add(new SKDStateViewModel(libraryStates, canvases));
			}

			_canvas.Children.Clear();
			foreach (var canvas in canvases)
				_canvas.Children.Add(new Viewbox() { Child = canvas });
		}

		public static FrameworkElement GetDefaultPicture(Guid driverId)
		{
			UIElement content = null;

			var device = SKDManager.SKDLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverId);
			if (device != null)
			{
				var state = device.States.FirstOrDefault(x => x.Code == null && x.StateClass == XStateClass.No);
				Canvas canvas = Helper.Xml2Canvas(state.Frames[0].Image);
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