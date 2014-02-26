using System;
using System.Linq;
using Common;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XDirection Direction { get; private set; }
		public XPumpStation PumpStation { get; private set; }
		public XDelay Delay { get; private set; }
		public XPim Pim { get; private set; }
		public string PresentationName { get; private set; }
		
		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowInTree);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			JournalItem = journalItem;
			IsExistsInConfig = true;
			
			try
			{
				switch (JournalItem.JournalItemType)
				{
					case JournalItemType.Device:
						Device = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Device != null)
						{
							PresentationName = Device.PresentationName;
						}
						break;

					case JournalItemType.Zone:
						Zone = XManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Zone != null)
						{
							PresentationName = Zone.PresentationName;
						}
						break;

					case JournalItemType.Direction:
						Direction = XManager.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Direction != null)
						{
							PresentationName = Direction.PresentationName;
						}
						break;

					case JournalItemType.PumpStation:
						PumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (PumpStation != null)
						{
							PresentationName = PumpStation.PresentationName;
						}
						break;

					case JournalItemType.Delay:
						Delay = XManager.Delays.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Delay != null)
						{
							PresentationName = Delay.PresentationName;
						}
						break;

					case JournalItemType.Pim:
						Pim = XManager.Pims.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (Pim != null)
						{
							PresentationName = Pim.PresentationName;
						}
						break;

					case JournalItemType.GkUser:
						PresentationName = JournalItem.UserName;
						break;

					case JournalItemType.GK:
					case JournalItemType.System:
						PresentationName = "";
						break;
				}

				if (PresentationName == null)
				{
					PresentationName = JournalItem.ObjectName;
					IsExistsInConfig = false;
				}

				if(PresentationName == null)
					PresentationName = "<Нет в конфигурации>";
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalItemViewModel ctr");
			}
		}

		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(JournalItem.AdditionalDescription))
					return JournalItem.Description + " " + JournalItem.AdditionalDescription;
				return JournalItem.Description;
			}
		}

		public string ControllerName
		{
			get
			{
				if (JournalItem.ControllerAddress == 0)
					return "Неизвестно";
				if (JournalItem.ControllerAddress == 512)
					return "ГК";
				return "КАУ " + JournalItem.ControllerAddress;
			}
		}

		public bool CanShow
		{
			get { return CanShowInTree() || CanShowOnPlan(); }
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowInTree())
				OnShowObject();
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
					break;

				case JournalItemType.Zone:
					DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
					break;

				case JournalItemType.Direction:
					DialogService.ShowWindow(new DirectionDetailsViewModel(Direction));
					break;

				case JournalItemType.PumpStation:
					DialogService.ShowWindow(new PumpStationDetailsViewModel(PumpStation));
					break;

				case JournalItemType.Delay:
					DialogService.ShowWindow(new DelayDetailsViewModel(Delay));
					break;

				case JournalItemType.Pim:
					DialogService.ShowWindow(new PimDetailsViewModel(Pim));
					break;
			}
		}
		bool CanShowProperties()
		{
			if (!IsExistsInConfig)
				return false;

			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
				case JournalItemType.Direction:
				case JournalItemType.PumpStation:
				case JournalItemType.Delay:
				case JournalItemType.Pim:
					return true;
			}
			return false;
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.Zone:
					ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.Direction:
					ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.PumpStation:
					ServiceFactory.Events.GetEvent<ShowXPumpStationEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.Delay:
					ServiceFactory.Events.GetEvent<ShowXDelayEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.Pim:
					ServiceFactory.Events.GetEvent<ShowXPimEvent>().Publish(JournalItem.ObjectUID);
					break;
			}
		}

		public bool IsExistsInConfig { get; private set; }

		bool CanShowInTree()
		{
			if (!IsExistsInConfig)
				return false;

			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
				case JournalItemType.Direction:
				case JournalItemType.PumpStation:
				case JournalItemType.Delay:
				case JournalItemType.Pim:
				case JournalItemType.GK:
					return true;
			}
			return false;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					if (Device != null)
					{
						ShowOnPlanHelper.ShowDevice(Device);
					}
					break;
				case JournalItemType.Zone:
					if (Zone != null)
					{
						ShowOnPlanHelper.ShowZone(Zone);
					}
					break;
				case JournalItemType.Direction:
					if (Direction != null)
					{
						ShowOnPlanHelper.ShowDirection(Direction);
					}
					break;
			}
		}
		bool CanShowOnPlan()
		{
			if (!IsExistsInConfig)
				return false;
			
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					if (Device != null)
					{
						return ShowOnPlanHelper.CanShowDevice(Device);
					}
					break;
				case JournalItemType.Zone:
					if (Zone != null)
					{
						return ShowOnPlanHelper.CanShowZone(Zone);
					}
					break;
				case JournalItemType.Direction:
					if (Direction != null)
					{
						return ShowOnPlanHelper.CanShowDirection(Direction);
					}
					break;
			}
			return false;
		}

		public bool ShowAdditionalProperties
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}
	}
}