using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;

namespace JournalModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public bool IsExistsInConfig { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string ObjectImageSource { get; private set; }
		public string ObjectName { get; private set; }
		public XStateClass StateClass { get; private set; }

		CompositePresentationEvent<Guid> ShowObjectEvent;
		CompositePresentationEvent<Guid> ShowObjectDetailsEvent;

		public GKDevice Device { get; set; }
		GKSKDZone GKSKDZone { get; set; }
		GKDoor GKDoor { get; set; }
		SKDDevice SKDDevice { get; set; }
		SKDZone SKDZone { get; set; }
		SKDDoor SKDDoor { get; set; }
		Camera Camera { get; set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowVideoCommand = new RelayCommand(OnShowVideo, CanShowVideo);

			JournalItem = journalItem;

			if (journalItem.JournalEventNameType != JournalEventNameType.NULL)
			{
				Name = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
			}

			if (journalItem.JournalEventDescriptionType != JournalEventDescriptionType.NULL)
			{
				Description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
				if (!string.IsNullOrEmpty(journalItem.DescriptionText))
					Description += " " + journalItem.DescriptionText;
			}
			else
			{
				Description = journalItem.DescriptionText;
			}

			IsExistsInConfig = true;
			ObjectImageSource = "/Controls;component/Images/Blank.png";
			StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);

			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					Device = GKManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Device != null)
					{
						ObjectName = Device.PresentationName;
						ObjectImageSource = Device.Driver.ImageSource;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>();
					}
					break;

				case JournalObjectType.GKSKDZone:
					GKSKDZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (GKSKDZone != null)
					{
						ObjectName = GKSKDZone.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKSKDZoneEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKSKDZoneDetailsEvent>();
					}
					ObjectImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.GKDoor:
					GKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (GKDoor != null)
					{
						ObjectName = GKDoor.PresentationName;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowGKDoorEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDoorDetailsEvent>();
					}
					ObjectImageSource = "/Controls;component/Images/Door.png";
					break;

				case JournalObjectType.SKDDevice:
					SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (SKDDevice != null)
					{
						ObjectName = SKDDevice.Name;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDDeviceDetailsEvent>();
						ObjectImageSource = SKDDevice.Driver.ImageSource;
					}
					break;

				case JournalObjectType.SKDZone:
					SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (SKDZone != null)
					{
						ObjectName = SKDZone.Name;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDZoneDetailsEvent>();
					}
					ObjectImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.SKDDoor:
					SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (SKDDoor != null)
					{
						ObjectName = SKDDoor.Name;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowSKDDoorEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDDoorDetailsEvent>();
					}
					ObjectImageSource = "/Controls;component/Images/Door.png";
					break;

				case JournalObjectType.VideoDevice:
					Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (Camera != null)
					{
						ObjectName = Camera.Name;
						ShowObjectEvent = ServiceFactory.Events.GetEvent<ShowCameraEvent>();
						ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>();
					}
					ObjectImageSource = "/Controls;component/Images/Camera.png";
					break;

				case JournalObjectType.None:
				case JournalObjectType.GKUser:
					ObjectName = JournalItem.ObjectName != null ? JournalItem.ObjectName : "";
					break;
			}

			if (ObjectName == null)
			{
				ObjectName = JournalItem.ObjectName;
				IsExistsInConfig = false;
			}

			if (ObjectName == null)
				ObjectName = "<Нет в конфигурации>";

			//if (JournalItem.EmployeeUID != Guid.Empty)
			//{
			//	var employee = EmployeeHelper.GetDetails(JournalItem.EmployeeUID);
			//	if (employee != null)
			//	{

			//	}
			//}
		}

		public bool IsStateImage
		{
			get { return JournalItem != null && JournalItem.ObjectName != null && JournalItem.ObjectName.EndsWith("АМ-R2"); }
		}

		public bool CanShow
		{
			get { return CanShowObject() || CanShowOnPlan(); }
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowObject())
				OnShowObject();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (ShowObjectEvent != null)
				ShowObjectEvent.Publish(JournalItem.ObjectUID);
		}
		bool CanShowObject()
		{
			return IsExistsInConfig && ShowObjectEvent != null;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (ShowObjectDetailsEvent != null)
				ShowObjectDetailsEvent.Publish(JournalItem.ObjectUID);
		}
		bool CanShowProperties()
		{
			return IsExistsInConfig && ShowObjectDetailsEvent != null;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					if (Device != null)
					{
						ShowOnPlanHelper.ShowDevice(Device);
					}
					break;
				case JournalObjectType.GKSKDZone:
					if (GKSKDZone != null)
					{
						ShowOnPlanHelper.ShowGKSKDZone(GKSKDZone);
					}
					break;
				case JournalObjectType.GKDoor:
					if (GKDoor != null)
					{
						ShowOnPlanHelper.ShowDoor(GKDoor);
					}
					break;
				case JournalObjectType.SKDDevice:
					if (SKDDevice != null)
					{
						ShowOnPlanHelper.ShowSKDDevice(SKDDevice);
					}
					break;
				case JournalObjectType.SKDZone:
					if (SKDZone != null)
					{
						ShowOnPlanHelper.ShowSKDZone(SKDZone);
					}
					break;
				case JournalObjectType.SKDDoor:
					if (SKDDoor != null)
					{
						ShowOnPlanHelper.ShowSKDDoor(SKDDoor);
					}
					break;
			}
		}
		bool CanShowOnPlan()
		{
			if (!IsExistsInConfig)
				return false;

			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					if (Device != null)
					{
						return ShowOnPlanHelper.CanShowDevice(Device);
					}
					break;
				case JournalObjectType.GKSKDZone:
					if (GKSKDZone != null)
					{
						return ShowOnPlanHelper.CanShowGKSKDZone(GKSKDZone);
					}
					break;
				case JournalObjectType.GKDoor:
					if (GKDoor != null)
					{
						return ShowOnPlanHelper.CanShowDoor(GKDoor);
					}
					break;
				case JournalObjectType.SKDDevice:
					if (SKDDevice != null)
					{
						return ShowOnPlanHelper.CanShowSKDDevice(SKDDevice);
					}
					break;
				case JournalObjectType.SKDZone:
					if (SKDZone != null)
					{
						return ShowOnPlanHelper.CanShowSKDZone(SKDZone);
					}
					break;
				case JournalObjectType.SKDDoor:
					if (SKDDoor != null)
					{
						return ShowOnPlanHelper.CanShowSKDDoor(SKDDoor);
					}
					break;
			}
			return false;
		}

		public RelayCommand ShowVideoCommand { get; private set; }
		void OnShowVideo()
		{
			var videoViewModel = new VideoViewModel(JournalItem.VideoUID, JournalItem.CameraUID);
			DialogService.ShowModalWindow(videoViewModel);
		}
		bool CanShowVideo()
		{
			return JournalItem.VideoUID != Guid.Empty;
		}
	}
}