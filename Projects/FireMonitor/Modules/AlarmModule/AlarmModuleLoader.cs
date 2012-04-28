using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using FiresecAPI.Models;

namespace AlarmModule
{
	public class AlarmModuleLoader
	{
		static AlarmWatcher AlarmWatcher;
		static AlarmVideoWather AlarmVideoWather;
		static AlarmsViewModel AlarmsViewModel;

		public AlarmModuleLoader()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Subscribe(OnShowAlarms);

			RegisterResources();
			CreateViewModels();
			CreateWatchers();
		}

		void RegisterResources()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		public static void Initialize()
		{
		}

		static void CreateViewModels()
		{
			AlarmsViewModel = new AlarmsViewModel();
		}

		static void CreateWatchers()
		{
			AlarmWatcher = new AlarmWatcher();
			AlarmVideoWather = new AlarmVideoWather();
		}

		void OnShowAlarms(AlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
			ServiceFactory.Layout.Show(AlarmsViewModel);
		}
	}
}