using System.Collections.Generic;
using AlarmModule.Events;
using AlarmModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;

namespace AlarmModule
{
	public class AlarmModuleLoader : ModuleBase
	{
		AlarmWatcher AlarmWatcher;
		AlarmVideoWather AlarmVideoWather;
		AlarmsViewModel AlarmsViewModel;

		public AlarmModuleLoader()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Subscribe(OnShowAlarms);
			AlarmsViewModel = new AlarmsViewModel();

			AlarmWatcher = new AlarmWatcher();
			AlarmVideoWather = new AlarmVideoWather();
		}

		void OnShowAlarms(AlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
			ServiceFactory.Layout.Show(AlarmsViewModel);
		}

		public override void Initialize()
		{
			AlarmsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowAlarmsEvent, AlarmType?>("Состояния", "/Controls;component/Images/Alarm.png") { SupportMultipleSelect = true}
			};
		}

		public override string Name
		{
			get { return "Состояния"; }
		}
	}
}