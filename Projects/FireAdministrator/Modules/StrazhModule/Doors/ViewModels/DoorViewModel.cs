using DeviceControls;
using Localization.Strazh.ViewModels;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using System;
using System.Windows;
using System.Windows.Shapes;

namespace StrazhModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public SKDDevice InDevice { get; private set; }
		public SKDDevice OutDevice { get; private set; }

		public DoorViewModel(SKDDoor door)
		{
			Door = door;
			ChangeInDeviceCommand = new RelayCommand(OnChangeInDevice);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			Update(door);
			door.Changed += new Action(door_Changed);
		}

		void door_Changed()
		{
			Update(Door);
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public string Description
		{
			get { return Door.Description; }
			set
			{
				Door.Description = value;
				Door.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public string InDeviceName
		{
			get
			{
				if (InDevice != null)
					return InDevice.NameWithParent;
				return CommonViewModels.Device_Select;
			}
		}

		public bool IsInDeviceGrayed
		{
			get { return InDevice == null; }
		}

		public RelayCommand ChangeInDeviceCommand { get; private set; }
		void OnChangeInDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(Door.InDeviceUID, Door.DoorType);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (deviceSelectationViewModel.SelectedDevice != null)
				{
					InDevice = deviceSelectationViewModel.SelectedDevice;
					SKDManager.ChangeDoorDevice(Door, InDevice);
					Update(Door);
				}

				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public void Update(SKDDoor door)
		{
			Door = door;
			SKDManager.UpdateDoor(door);
			InDevice = door.InDevice;
			OutDevice = door.OutDevice;

			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => InDevice);
			OnPropertyChanged(() => InDeviceName);
			OnPropertyChanged(() => IsInDeviceGrayed);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => OutDevice);
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public bool IsOnPlan
		{
			get { return Door.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return true; }
		}
		public VisualizationState VisualizationState
		{
			get { return IsOnPlan ? (Door.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		void OnCreateDragObjectCommand(DataObject dataObject)
		{
			DoorsViewModel.Current.SelectedDoor = this;
			var plansElement = new ElementDoor
			{
				DoorUID = Door.UID
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
			var brush = PictureCacheSource.DoorPicture.GetDefaultBrush();
			return new Rectangle
			{
				Fill = brush,
				Height = PainterCache.DefaultPointSize,
				Width = PainterCache.DefaultPointSize,
			};
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Door.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Door.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Door.AllowMultipleVizualization = isAllow;
			Update();
		}
		bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Door.AllowMultipleVizualization != isAllow;
		}
	}
}