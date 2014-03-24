using System.Linq;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public SKDJournalItem JournalItem { get; private set; }
		public SKDDevice Device { get; private set; }
		
		public JournalItemViewModel(SKDJournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			JournalItem = journalItem;
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.DeviceUID);
		}

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
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
		bool CanShowProperties()
		{
			return Device != null;
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(JournalItem.DeviceUID);
		}
		bool CanShowObject()
		{
			return Device != null;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDevice(Device);
		}
		bool CanShowOnPlan()
		{
			return Device != null && ShowOnPlanHelper.CanShowDevice(Device);
		}
	}
}