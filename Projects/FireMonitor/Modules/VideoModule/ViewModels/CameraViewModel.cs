﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Video.RVI_VSS;
using Infrustructure.Plans.Painters;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		public Camera Camera { get; set; }
		readonly CellPlayerWrap _cellPlayerWrap;

		public CameraViewModel(Camera camera, CellPlayerWrap cellPlayerWrap)
		{
			_cellPlayerWrap = cellPlayerWrap;
			_cellPlayerWrap.DropHandler += CellPlayerWrapOnDropHandler;
			Camera = camera;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			UpdateChildren();
		}

		public string ViewName
		{
			get { return _cellPlayerWrap.Name; }
		}

		public bool IsDvr
		{
			get
			{
				return Camera.CameraType == CameraType.Dvr;
			}
		}

		public bool IsChannel
		{
			get
			{
				return Camera.CameraType == CameraType.Channel;
			}
		}

		public bool IsCamera
		{
			get
			{
				return Camera.CameraType == CameraType.Camera;
			}
		}

		public void UpdateChildren()
		{
			if ((Camera != null) && (Camera.CameraType == CameraType.Dvr))
				foreach (var child in Camera.Children)
				{
					var cameraViewModel = new CameraViewModel(child, new CellPlayerWrap());
					AddChild(cameraViewModel);
				}
		}

		public string PresentationName
		{
			get
			{
				if (Camera.CameraType != CameraType.Channel)
					return Camera.Name + " " + Camera.Address;
				return Camera.Name + " " +(Camera.ChannelNumber + 1);
			}
		}

		public string PresentationAddress
		{
			get
			{
				if (Camera.CameraType != CameraType.Channel)
					return Camera.Address;
				return (Camera.ChannelNumber + 1).ToString();
			}
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
				return IsOnPlan
					? (Camera.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single)
					: VisualizationState.NotPresent;
			}
		}

		public void Connect()
		{
			_cellPlayerWrap.Connect(Camera.Address, Camera.Port, Camera.Login, Camera.Password);
		}

		public void Start()
		{
			_cellPlayerWrap.Start(Camera.ChannelNumber);
		}
		
		public void Stop()
		{
			_cellPlayerWrap.Stop();
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }

		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			var camera = Camera;
			dataObject.SetData("DESIGNER_ITEM", camera);
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

		private void CellPlayerWrapOnDropHandler(Camera camera)
		{
			if (String.IsNullOrEmpty(_cellPlayerWrap.Name)) // Запрет менять для однооконного режима(?)
				return;
			Camera = camera;
			if (ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == _cellPlayerWrap.Name).Value == Camera.UID)
				return;
			try
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
				{
					Stop();
					Connect();
					Start();
				}));
				ClientSettings.RviMultiLayoutCameraSettings.Dictionary[_cellPlayerWrap.Name] = Camera.UID;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "LayoutMultiCameraView.OnShowProperties");
			}
		}

		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			if (Camera.CameraType == CameraType.Dvr)
				return false;
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}
	}
}