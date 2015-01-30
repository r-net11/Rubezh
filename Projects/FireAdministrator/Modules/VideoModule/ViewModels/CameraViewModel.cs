using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		CamerasViewModel _camerasViewModel;
		public Camera Camera { get; set; }

		public CameraViewModel(CamerasViewModel camerasViewModel, Camera camera)
		{
			_camerasViewModel = camerasViewModel;
			Camera = camera;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand,
				CanAllowMultipleVizualizationCommand);
		}

		public string PresentationAddress
		{
			get
			{
				return Camera.Ip;
			}
		}
		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = GKManager.GetCommaSeparatedObjects(new List<ModelBase>(zones));
				return presentationZones;
			}
		}

		public string PresentationState
		{
			get
			{
				if ((Camera.StateClass == XStateClass.Fire1) || (Camera.StateClass == XStateClass.Fire2) ||
					(Camera.StateClass == XStateClass.Attention) || (Camera.StateClass == XStateClass.Ignore))
					return Camera.StateClass.ToDescription();
				return null;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationZones);
			OnPropertyChanged(() => PresentationAddress);
			OnPropertyChanged(() => PresentationState);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public bool IsOnPlan
		{
			get { return Camera.PlanElementUIDs.Count > 0; }
		}

		public VisualizationState VisualizationState
		{
			get
			{
				if (IsOnPlan)
					return Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single;
				return VisualizationState.NotPresent;
			}
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }

		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			_camerasViewModel.SelectedCamera = this;
			var plansElement = new ElementCamera
			{
				CameraUID = Camera.UID
			};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}

		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}

		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }

		private UIElement OnCreateDragVisual(IDataObject dataObject)
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
		private void OnShowOnPlan()
		{
			if (Camera.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Camera.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		private void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Camera.AllowMultipleVizualization = isAllow;
			Update();
			CommandManager.InvalidateRequerySuggested();
			ServiceFactory.SaveService.CamerasChanged = true;
		}

		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Camera.AllowMultipleVizualization != isAllow;
		}
	}
}