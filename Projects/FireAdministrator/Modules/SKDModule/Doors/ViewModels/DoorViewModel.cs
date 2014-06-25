using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using FiresecAPI.Models;
using DeviceControls;
using System.Windows.Shapes;
using Infrustructure.Plans.Painters;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;

namespace SKDModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public Door Door { get; private set; }
		public SKDDevice InDevice { get; private set; }
		public SKDDevice OutDevice { get; private set; }

		public DoorViewModel(Door door)
		{
			ChangeInDeviceCommand = new RelayCommand(OnChangeInDevice);
			ChangeOutDeviceCommand = new RelayCommand(OnChangeOutDevice);
			Door = door;
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			Update();
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged("Name");
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
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public string InDeviceName
		{
			get
			{
				if (InDevice != null)
					return InDevice.Name;
				return "Нажмите для выбора устройства";
			}
		}

		public string OutDeviceName
		{
			get
			{
				if (OutDevice != null)
					return OutDevice.Name;
				return "Нажмите для выбора устройства";
			}
		}

		public bool IsInDeviceGrayed
		{
			get { return InDevice == null; }
		}

		public bool IsOutDeviceGrayed
		{
			get { return OutDevice == null; }
		}

		public RelayCommand ChangeInDeviceCommand { get; private set; }
		void OnChangeInDevice()
		{
			//IsSelected = true;
			var deviceSelectationViewModel = new DeviceSelectationViewModel(Door.InDeviceUID);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (deviceSelectationViewModel.SelectedDevice != null)
				{
					InDevice = deviceSelectationViewModel.SelectedDevice;
					Door.InDeviceUID = InDevice.UID;
				}
				OnPropertyChanged("InDevice");
				OnPropertyChanged("InDeviceName");
				OnPropertyChanged("IsInDeviceGrayed");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand ChangeOutDeviceCommand { get; private set; }
		void OnChangeOutDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(Door.OutDeviceUID);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (deviceSelectationViewModel.SelectedDevice != null)
				{
					OutDevice = deviceSelectationViewModel.SelectedDevice;
					Door.OutDeviceUID = OutDevice.UID;
				}
				OnPropertyChanged("OutDevice");
				OnPropertyChanged("OutDeviceName");
				OnPropertyChanged("IsOutDeviceGrayed");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public void Update(Door door)
		{
			Door = door;
			OnPropertyChanged("Door");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		public void Update()
		{
			InDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
			OutDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.OutDeviceUID);

			if (Door.PlanElementUIDs == null)
				Door.PlanElementUIDs = new List<Guid>();
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
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			DoorsViewModel.Current.SelectedDoor = this;
			var plansElement = new ElementDoor
			{
				DoorUID = Door.UID
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
		private void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Door.AllowMultipleVizualization = isAllow;
			Update();
		}
		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Door.AllowMultipleVizualization != isAllow;
		}
	}
}