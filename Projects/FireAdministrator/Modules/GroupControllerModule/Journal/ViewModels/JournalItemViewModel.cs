using System.Linq;
using FiresecClient;
using GKProcessor;
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
				ImageSource = "/Controls;component/GKIcons/GK.png";
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
				}
			}
		}

		void InitializePresentationName()
		{
			if (!string.IsNullOrEmpty(JournalItem.ObjectName))
			{
				PresentationName = JournalItem.ObjectName;
				return;
			}

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

				case JournalItemType.Delay:
					XDelay delay = null;
					foreach (var gkDatabase in DescriptorsManager.GkDatabases)
					{
						delay = gkDatabase.Delays.FirstOrDefault(x => x.Name == JournalItem.ObjectName);
						if (delay != null)
							break;
					}
					if (delay != null)
					{
						PresentationName = delay.Name;
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
	}
}