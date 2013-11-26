using System.Linq;
using System.Text;
using GKProcessor;
using FiresecAPI;
using FiresecAPI.XModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using System.Diagnostics;
using System;
using Common;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XDirection Direction { get; private set; }
		public XDelay Delay { get; private set; }
		public XPim Pim { get; private set; }
		public string PresentationName { get; private set; }
		
		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowInTree);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
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

					case JournalItemType.Delay:
						foreach (var gkDatabase in DescriptorsManager.GkDatabases)
						{
							Delay = gkDatabase.Delays.FirstOrDefault(x => x.Name == JournalItem.ObjectName);
							if (Delay != null)
								break;
						}
						if (Delay != null)
						{
							PresentationName = Delay.PresentationName;
						}
						break;

					case JournalItemType.Pim:
						foreach (var gkDatabase in DescriptorsManager.GkDatabases)
						{
							Pim = gkDatabase.Pims.FirstOrDefault(x => x.Name == JournalItem.ObjectName);
							if (Pim != null)
								break;
						}
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

		public bool CanShow
		{
			get
			{
				return CanShowInTree() || CanShowOnPlan();
			}
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowInTree())
				OnShowObject();
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

#if DEBUG
				case JournalItemType.Delay:
					ServiceFactory.Events.GetEvent<ShowXDelayEvent>().Publish(JournalItem.ObjectUID);
					break;
#endif
				case JournalItemType.GK:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
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
#if DEBUG
				case JournalItemType.Delay:
				case JournalItemType.Pim:
#endif
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