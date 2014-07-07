using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Events;
using FiresecAPI;

namespace JournalModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public SKDDevice Device { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);

			JournalItem = journalItem;
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);

			if (journalItem.Name != GlobalEventNameEnum.NULL)
			{
				Name = EventDescriptionAttributeHelper.ToName(journalItem.Name);
			}
			else
			{
				Name = journalItem.NameText;
			}

			if (journalItem.Description != FiresecAPI.GK.EventDescription.NULL)
			{
				Description = journalItem.Description.ToDescription();
			}
			else
			{
				Description = journalItem.DescriptionText;
			}
			StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.Name);
			ObjectImageSource = "/Controls;component/Images/blank.png";
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string ObjectImageSource { get; private set; }
		public string ObjectName { get; private set; }
		public FiresecAPI.GK.XStateClass StateClass { get; private set; }

		public bool IsExistsInConfig
		{
			get { return Device != null; }
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

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			//DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
		bool CanShowProperties()
		{
			return Device != null;
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			//ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(JournalItem.ObjectUID);
		}
		bool CanShowObject()
		{
			return Device != null;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			//ServiceFactory.OnPublishEvent<SKDDevice, ShowSKDDeviceOnPlanEvent>(Device);
		}
		private bool CanShowOnPlan()
		{
			return false;
			//return Device != null && ShowOnPlanHelper.CanShowDevice(Device);
		}
	}
}