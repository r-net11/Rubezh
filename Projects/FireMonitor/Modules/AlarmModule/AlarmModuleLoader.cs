using System.Collections.Generic;
using AlarmModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace AlarmModule
{
	public class AlarmModuleLoader : ModuleBase
	{
		AlarmWatcher AlarmWatcher;
		AlarmsViewModel AlarmsViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupsViewModel());
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Subscribe(OnShowAlarms);
			AlarmsViewModel = new AlarmsViewModel();
			AlarmWatcher = new AlarmWatcher();
		}

		void OnShowAlarms(AlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
		}

		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowAlarmsEvent, AlarmType?>(AlarmsViewModel, "Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true}
			};
		}

		public override string Name
		{
			get { return "Состояния"; }
		}

		public override void AfterInitialize()
		{
			AlarmsViewModel.SubscribeShortcuts();
		}
	}
}