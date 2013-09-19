using System.Linq;
using Common.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			JournalItem = journalItem;

			InitializeTypeAddressImageSource(journalItem);
			PresentationName = TypeName + " " + Address;
			InitializePresentationName(journalItem);
		}

		void InitializeTypeAddressImageSource(JournalItem journalItem)
		{
			var internalAddress = journalItem.InternalJournalItem.ObjectDeviceAddress;
			var internalType = journalItem.InternalJournalItem.ObjectDeviceType;

			if (internalType == 0)
			{
				TypeName = "ГК";
				Address = "";
				ImageSource = "/Controls;component/GKIcons/GK.png";
				return;
			}

			Address = internalAddress.ToString();

			var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				TypeName = driver.ShortName;
				if (driver.IsDeviceOnShleif)
					Address = (internalAddress / 256 + 1).ToString() + "." + (internalAddress % 256).ToString();
				if (!driver.HasAddress)
					Address = "";
				ImageSource = driver.ImageSource;
			}
			switch (internalType)
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
			}
		}

		void InitializePresentationName(JournalItem journalItem)
		{
			JournalItem = journalItem;
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					var device = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (device != null)
					{
						PresentationName = device.Driver.ShortName + " " + device.DottedAddress;
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

				case JournalItemType.System:
					break;
			}
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

				case JournalItemType.GK:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
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
				case JournalItemType.GK:
					return true;
			}
			return false;
		}

		public JournalItem JournalItem { get; private set; }
		public string PresentationName { get; private set; }
		public string TypeName { get; private set; }
		public string Address { get; private set; }
		public string ImageSource { get; private set; }
	}
}