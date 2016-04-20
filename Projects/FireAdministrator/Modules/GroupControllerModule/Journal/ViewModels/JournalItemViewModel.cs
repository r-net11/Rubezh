using System;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI.Journal;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public string PresentationName { get; private set; }
		public string TypeName { get; private set; }
		public string Address { get; private set; }
		public string ImageSource { get; private set; }
		public string GKJournalRecordNo { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
		public XStateClass StateClass { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
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

			InitializeTypeAddressImageSource(journalItem);
			PresentationName = TypeName + " " + Address;
			InitializePresentationName();

			var noItem = journalItem.JournalDetalisationItems.FirstOrDefault(x => x.Name == "Запись ГК");
			if (noItem != null)
			{
				GKJournalRecordNo = noItem.Value;
			}

			StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
		}

		void InitializeTypeAddressImageSource(JournalItem journalItem)
		{
			ImageSource = "/Controls;component/Images/Blank.png";

			int descriptorType = 0;
			int descriptorAddress;
			var descriptorTypeItem = journalItem.JournalDetalisationItems.FirstOrDefault(x => x.Name == "Тип дескриптора");
			if (descriptorTypeItem != null)
			{
				if (!Int32.TryParse(descriptorTypeItem.Value, out descriptorType))
					return;
			}
			else
			{
				return;
			}
			var descriptorAddressItem = journalItem.JournalDetalisationItems.FirstOrDefault(x => x.Name == "Адрес дескриптора");
			if (descriptorAddressItem != null)
			{
				if (!Int32.TryParse(descriptorAddressItem.Value, out descriptorAddress))
					return;
			}
			else
			{
				return;
			}

			if (descriptorType == 0)
			{
				TypeName = "ГК";
				Address = "";
				return;
			}

			Address = descriptorAddress!= 0 ? descriptorAddress.ToString(): String.Empty;

			var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == descriptorType);
			if (driver != null)
			{
				TypeName = driver.ShortName;
				if (driver.IsDeviceOnShleif)
					Address = (descriptorAddress / 256 + 1).ToString() + "." + (descriptorAddress % 256).ToString();
				if (!driver.HasAddress)
					Address = "";
				ImageSource = driver.ImageSource;
			}
			else
			{
				switch (descriptorType)
				{
					case 0x100:
						TypeName = "Зона";
						ImageSource = "/Controls;component/Images/Zone.png";
						break;

					case 0x101:
						TypeName = "Задержка";
						ImageSource = "/Controls;component/Images/Delay.png";
						break;

					case 0x104:
						TypeName = "Точка доступа";
						ImageSource = "/Controls;component/Images/Door.png";
						break;

					case 0x106:
						TypeName = "Направление";
						ImageSource = "/Controls;component/Images/Blue_Direction.png";
						break;

					case 0x107:
						TypeName = "ПИМ";
						ImageSource = "/Controls;component/Images/Pim.png";
						break;

					case 0x108:
						TypeName = "Охранная зона";
						ImageSource = "/Controls;component/Images/GuardZone.png";
						break;


				}
			}
		}

		void InitializePresentationName()
		{
			try
			{
				switch (JournalItem.JournalObjectType)
				{
					case JournalObjectType.GKDevice:
						var device = GKManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (device != null)
						{
							PresentationName = device.PresentationName;
						}
						break;

					case JournalObjectType.GKZone:
						var zone = GKManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (zone != null)
						{
							PresentationName = zone.PresentationName;
						}
						break;

					case JournalObjectType.GKGuardZone:
						var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (guardZone != null)
						{
							PresentationName = guardZone.PresentationName;
						}
						break;

					case JournalObjectType.GKDirection:
						var direction = GKManager.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (direction != null)
						{
							PresentationName = direction.PresentationName;
						}
						break;

					case JournalObjectType.GKPumpStation:
						var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (pumpStation != null)
						{
							PresentationName = pumpStation.PresentationName;
							ImageSource = "/Controls;component/Images/BPumpStation.png";
						}
						break;

					case JournalObjectType.GKDoor:
						var door = GKManager.Doors.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (door != null)
						{
							PresentationName = door.PresentationName;
							ImageSource = "/Controls;component/Images/Door.png";
						}
						break;

					case JournalObjectType.GKMPT:
						var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (mpt != null)
						{
							PresentationName = mpt.PresentationName;
							ImageSource = "/Controls;component/Images/BMPT.png";
						}
						break;

					case JournalObjectType.GKDelay:
						var delay = GKManager.Delays.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (delay != null)
						{
							PresentationName = delay.PresentationName;
						}
						break;

					case JournalObjectType.GKPim:
						var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (pim != null)
						{
							PresentationName = pim.PresentationName;
						}
						break;

					case JournalObjectType.GKUser:
						PresentationName = JournalItem.UserName;
						break;

					case JournalObjectType.None:
						PresentationName = "";
						break;
				}

				if (PresentationName == null)
				{
					PresentationName = JournalItem.ObjectName;
				}

				if (PresentationName == null)
					PresentationName = "<Нет в конфигурации>";
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalItemViewModel ctr");
			}
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					if (GKManager.Devices.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalObjectType.GKZone:
					if (GKManager.Zones.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalObjectType.GKGuardZone:
					if (GKManager.GuardZones.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalObjectType.GKDirection:
					if (GKManager.Directions.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>().Publish(JournalItem.ObjectUID);
					}
					break;
				case JournalObjectType.GKPumpStation:
					if (GKManager.PumpStations.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowGKPumpStationEvent>().Publish(JournalItem.ObjectUID);
					}
					break;
			}
		}
		bool CanShowObject()
		{
			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
				case JournalObjectType.GKZone:
				case JournalObjectType.GKGuardZone:
				case JournalObjectType.GKDirection:
				case JournalObjectType.GKPumpStation:
					return true;
			}
			return false;
		}
	}
}