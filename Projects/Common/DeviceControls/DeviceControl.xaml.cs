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
using Common;

namespace DeviceControls
{
	public partial class DeviceControl : INotifyPropertyChanged
	{
		public DeviceControl()
		{
			InitializeComponent();
			DataContext = this;
			AdditionalStateCodes = new List<string>();
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
			}
		}

		List<string> _additionalStateCodes;
		public List<string> AdditionalStateCodes
		{
			get { return _additionalStateCodes; }
			set
			{
				_additionalStateCodes = value;
			}
		}

		List<StateViewModel> _stateViewModelList;

		public void Update()
		{
            var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
            if (libraryDevice == null)
            {
                Logger.Error("DeviceControl.Update libraryDevice = null " + DriverId.ToString());
                return;
            }

            var additionalLibraryStates = new List<LibraryState>();
            foreach (var additionalStateCode in AdditionalStateCodes)
            {
                var additionalState = libraryDevice.States.FirstOrDefault(x => x.Code == additionalStateCode);
                if (additionalState != null)
                {
                    if (additionalState.StateType == StateType)
                    {
                        additionalLibraryStates.Add(additionalState);
                    }
                }
            }

            var resultLibraryStates = new List<LibraryState>();

			if (_stateViewModelList.IsNotNullOrEmpty())
				_stateViewModelList.ForEach(x => x.Dispose());
			_stateViewModelList = new List<StateViewModel>();

			var libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType);
            if (libraryState == null)
            {
                if (!additionalLibraryStates.Any(x=>x.StateType == StateType))
                {
                    libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
                    if (libraryState == null)
                    {
                        Logger.Error("DeviceControl.Update libraryState = null " + DriverId.ToString());
                        return;
                    }
                }
            }

            if (libraryState!= null)
            {
                resultLibraryStates.Add(libraryState);
            }
            foreach (var additionalLibraryState in additionalLibraryStates)
            {
                resultLibraryStates.Add(additionalLibraryState);
            }

			var sortedResultLibraryStates = from LibraryState state in resultLibraryStates
											orderby state.Frames.FirstOrDefault().Layer
											select state;
            var canvases = new List<Canvas>();
			foreach (var libraryStates in sortedResultLibraryStates)
            {
                _stateViewModelList.Add(new StateViewModel(libraryStates, canvases));
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