using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Windows;
using DeviceControls;
using Infrustructure.Plans.Painters;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using System.Windows.Input;
using Infrastructure;

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

		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
		}
		
		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationZones);
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
				return IsOnPlan
					? (Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single)
					: VisualizationState.NotPresent;
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