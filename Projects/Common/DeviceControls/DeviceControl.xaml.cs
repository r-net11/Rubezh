using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

namespace DeviceControls
{
	public partial class DeviceControl : INotifyPropertyChanged
	{
		public DeviceControl()
		{
			InitializeComponent();
			DataContext = this;
			AdditionalStateCodes = new List<string>();
			IsManualUpdate = false;
		}

		void DeviceControl_Loaded(object sender, RoutedEventArgs e)
		{
			//_canvas.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
		}

		public Guid DriverId { get; set; }
		public bool IsManualUpdate { get; set; }

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				if (IsManualUpdate == false)
					Update();
			}
		}

		List<string> _additionalStateCodes;
		public List<string> AdditionalStateCodes
		{
			get { return _additionalStateCodes; }
			set
			{
				_additionalStateCodes = value;

				if ((IsManualUpdate == false) && _additionalStateCodes.IsNotNullOrEmpty())
					Update();
			}
		}

		List<StateViewModel> _stateViewModelList;

		public void Update()
		{
			var canvases = new List<Canvas>();

			if (_stateViewModelList.IsNotNullOrEmpty())
				_stateViewModelList.ForEach(x => x.Dispose());
			_stateViewModelList = new List<StateViewModel>();

			var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
			if (libraryDevice == null)
				return;

			var libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType);
			if (libraryState != null)
				_stateViewModelList.Add(new StateViewModel(libraryState, canvases));

			if (AdditionalStateCodes.IsNotNullOrEmpty())
				foreach (var additionalStateCode in AdditionalStateCodes)
				{
					var additionalState = libraryDevice.States.FirstOrDefault(x => x.Code == additionalStateCode);
					if (additionalState != null)
						_stateViewModelList.Add(new StateViewModel(additionalState, canvases));
				}

			_canvas.Children.Clear();
			foreach (var canvas in canvases)
				_canvas.Children.Add(new Viewbox() { Child = canvas });
		}

		public static FrameworkElement GetDefaultPicture(Guid DriverId)
		{
			UIElement content = null;

			var device = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
			if (device != null)
			{
				var state = device.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
				Canvas canvas = Helper.Xml2Canvas(state.Frames[0].Image, 0);
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