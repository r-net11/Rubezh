using System;
using System.Linq;
using Common;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public string PresentationName { get; private set; }
		public string TypeName { get; private set; }
		public string Address { get; private set; }
		public string ImageSource { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			JournalItem = journalItem;

			InitializeTypeAddressImageSource(journalItem);
			PresentationName = TypeName + " " + Address;
			InitializePresentationName();
		}

		void InitializeTypeAddressImageSource(JournalItem journalItem)
		{
			if (journalItem.DescriptorType == 0)
			{
				TypeName = "ГК";
				Address = "";
				ImageSource = "/Controls;component/Images/Blank.png"; ;
				return;
			}

			Address = journalItem.DescriptorAddress.ToString();

			var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == journalItem.DescriptorType);
			if (driver != null)
			{
				TypeName = driver.ShortName;
				if (driver.IsDeviceOnShleif)
					Address = (journalItem.DescriptorAddress / 256 + 1).ToString() + "." + (journalItem.DescriptorAddress % 256).ToString();
				if (!driver.HasAddress)
					Address = "";
				ImageSource = driver.ImageSource;
			}
			else
			{
				switch (journalItem.DescriptorType)
				{
					case 0x100:
						TypeName = "Зона";
						ImageSource = "/Controls;component/Images/zone.png";
						break;

					case 0x101:
						TypeName = "Задержка";
						ImageSource = "/Controls;component/Images/Delay.png";
						break;

					case 0x106:
						TypeName = "Направление";
						ImageSource = "/Controls;component/Images/Blue_Direction.png";
						break;

					case 0x107:
						TypeName = "ПИМ";
						ImageSource = "/Controls;component/Images/Pim.png";
						break;
				}
			}
		}

		void InitializePresentationName()
		{
			try
			{
				switch (JournalItem.JournalItemType)
				{
					case JournalItemType.Device:
						var device = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (device != null)
						{
							PresentationName = device.PresentationName;
						}
						break;

					case JournalItemType.Zone:
						var zone = XManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (zone != null)
						{
							PresentationName = zone.PresentationName;
						}
						break;

					case JournalItemType.Direction:
						var direction = XManager.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (direction != null)
						{
							PresentationName = direction.PresentationName;
						}
						break;

					case JournalItemType.PumpStation:
						var pumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (pumpStation != null)
						{
							PresentationName = pumpStation.PresentationName;
							ImageSource = "/Controls;component/Images/BPumpStation.png";
						}
						break;

					case JournalItemType.Delay:
						var delay = XManager.Delays.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (delay != null)
						{
							PresentationName = delay.PresentationName;
						}
						break;

					case JournalItemType.Pim:
						var pim = XManager.Pims.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (pim != null)
						{
							PresentationName = pim.PresentationName;
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
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					if (XManager.Devices.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalItemType.Zone:
					if (XManager.Zones.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalItemType.Direction:
					if (XManager.Directions.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(JournalItem.ObjectUID);
					}
					break;

				case JournalItemType.PumpStation:
					if (XManager.PumpStations.Any(x => x.UID == JournalItem.ObjectUID))
					{
						ServiceFactory.Events.GetEvent<ShowXPumpStationEvent>().Publish(JournalItem.ObjectUID);
					}
					break;
			}
		}
		bool CanShowObject()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
				case JournalItemType.Direction:
				case JournalItemType.PumpStation:
					return true;
			}
			return false;
		}
	}
}