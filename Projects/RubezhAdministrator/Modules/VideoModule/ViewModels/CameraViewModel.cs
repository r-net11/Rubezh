using DeviceControls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using RubezhAPI.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		CamerasViewModel _camerasViewModel;
		public Camera Camera { get; set; }
		public bool IsCamera { get { return Camera != null; } }
		public string PresentationName { get; private set; }
		public string PresentationAddress { get; private set; }
		public string ImageSource { get; private set; }
		public CameraViewModel(CamerasViewModel camerasViewModel, Camera camera, string presentationName)
		{
			_camerasViewModel = camerasViewModel;
			Camera = camera;
			camera.PlanElementUIDsChanged += UpdateVisualizationState;
			PresentationName = presentationName;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand,
				CanAllowMultipleVizualizationCommand);
			ImageSource = camera.ImageSource;
			Update();
		}
		public CameraViewModel(RviServer server)
		{
			PresentationName = server.PresentationName;
			ImageSource = server.ImageSource;
		}
		public CameraViewModel(RviDevice device)
		{
			PresentationName = device.Name;
			PresentationAddress = device.Ip;
			ImageSource = device.ImageSource;
		}
		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationAddress);
			UpdateVisualizationState();
		}
		void UpdateVisualizationState()
		{
			VisualizationState = IsOnPlan ? (Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent;
		}
		public bool IsOnPlan
		{
			get { return Camera != null && Camera.PlanElementUIDs.Count > 0; }
		}
		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				_visualizationState = value;
				OnPropertyChanged(() => VisualizationState);
			}
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		void OnCreateDragObjectCommand(DataObject dataObject)
		{
			_camerasViewModel.SelectedCamera = this;
			var plansElement = new ElementCamera
			{
				CameraUID = Camera.UID
			};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}

		bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}

		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }
		UIElement OnCreateDragVisual(IDataObject dataObject)
		{
			ServiceFactory.Layout.SetRightPanelVisible(true);
			var brush = PictureCacheSource.CameraPicture.GetDefaultBrush();
			return new System.Windows.Shapes.Rectangle
			{
				Fill = brush,
				Height = PainterCache.DefaultPointSize,
				Width = PainterCache.DefaultPointSize,
			};
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Camera.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Camera.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Camera.AllowMultipleVizualization = isAllow;
			Camera.OnChanged();
			Update();
			CommandManager.InvalidateRequerySuggested();
			ServiceFactory.SaveService.CamerasChanged = true;
		}

		bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Camera.AllowMultipleVizualization != isAllow;
		}

		public void UnsubscribeEvents()
		{
			if (IsCamera)
				Camera.PlanElementUIDsChanged -= UpdateVisualizationState;
		}
	}
}