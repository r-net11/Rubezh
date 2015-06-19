using System;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using FiresecAPI.Journal;

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

			Address = descriptorAddress.ToString();

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
				switch (JournalItem.JournalObjectType)
				{
					case JournalObjectType.GKDevice:
						var device = GKManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (device != null)
						{
							PresentationName = device.PresentationName;
						}
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
			}
		}
		bool CanShowObject()
		{
			switch (JournalItem.JournalObjectType)
			{
				case JournalObjectType.GKDevice:
					return true;
			}
			return false;
		}
	}
}