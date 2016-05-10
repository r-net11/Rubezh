using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Common;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using StrazhAPI.SKD;
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
					ObjectName = JournalItem.ObjectName != null ? JournalItem.ObjectName : "";
					break;
			}

			if (ObjectName == null)
			{
				ObjectName = JournalItem.ObjectName;
				IsExistsInConfig = false;
			}

			if (ObjectName == null)
				ObjectName = Resources.Language.JournalItemViewModel.ObjectName_Null;

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
			var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(c => c.UID == JournalItem.CameraUID);
			if (camera == null)
			{
				MessageBoxService.ShowWarning(Resources.Language.JournalItemViewModel.VideoDevice_Null);
				return;
			}

			var videoPath = AppDataFolderHelper.GetTempFileName() + ".mkv";
			try
			{
				RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, JournalItem.VideoUID, JournalItem.CameraUID, videoPath);
				DialogService.ShowModalWindow(new VideoViewModel(videoPath));
			}
			catch (CommunicationObjectFaultedException e)
			{
				Logger.Error(e, "Исключение при вызове VideoViewModel(Guid eventUID, Guid cameraUID)");
				MessageBoxService.ShowError(Resources.Language.JournalItemViewModel.RVi_NotConnected);
			}
			catch (FileNotFoundException)
			{
				MessageBoxService.ShowError(Resources.Language.JournalItemViewModel.FileNotFound);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове VideoViewModel(Guid eventUID, Guid cameraUID)");
			}
		}
		bool CanShowVideo()
		{
			return JournalItem.VideoUID != Guid.Empty;
		}

		public bool CanShowErrorCode
		{
			get { return JournalItem.ErrorCode != JournalErrorCode.None; }
		}
	}
}