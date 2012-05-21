using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SkudModule.ViewModels;
using Infrastructure.Common;

namespace SkudModule
{
	public class SkudModule : ModuleBase
	{
		private SkudViewModel _skudViewModel;
		private EmployeeCardIndexViewModel _employeeCardIndexViewModel;
		private EmployeeDepartmentsViewModel _employeeDepartmentsViewModel;
		private EmployeePositionsViewModel _employeePositionsViewModel;
		private EmployeeGroupsViewModel _employeeGroupsViewModel;

		public SkudModule()
		{
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Unsubscribe(OnShowSkud);
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Subscribe(OnShowSkud);
			ServiceFactory.Events.GetEvent<ShowEmployeeCardIndexEvent>().Unsubscribe(OnShowEmployeeCardIndexEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeeCardIndexEvent>().Subscribe(OnShowEmployeeCardIndexEvent);
			ServiceFactory.Events.GetEvent<ShowPassCardEvent>().Unsubscribe(OnShowPassCardEvent);
			ServiceFactory.Events.GetEvent<ShowPassCardEvent>().Subscribe(OnShowPassCardEvent);

			ServiceFactory.Events.GetEvent<ShowEmployeeDepartmentsEvent>().Unsubscribe(OnShowSkudDepartmentsEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeeDepartmentsEvent>().Subscribe(OnShowSkudDepartmentsEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeeGroupsEvent>().Unsubscribe(OnShowSkudGroupsEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeeGroupsEvent>().Subscribe(OnShowSkudGroupsEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeePositionsEvent>().Unsubscribe(OnShowSkudPositionsEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeePositionsEvent>().Subscribe(OnShowSkudPositionsEvent);
		}

		private void OnShowSkud(object obj)
		{
			//ServiceFactory.Layout.Show(_skudViewModel);
		}
		private void OnShowEmployeeCardIndexEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeCardIndexViewModel);
		}
		private void OnShowPassCardEvent(object obj)
		{
			ServiceFactory.Layout.Show(_skudViewModel);
		}

		private void OnShowSkudDepartmentsEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeDepartmentsViewModel);
		}
		private void OnShowSkudGroupsEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeGroupsViewModel);
		}
		private void OnShowSkudPositionsEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeePositionsViewModel);
		}

		public override void Initialize()
		{
			_skudViewModel = new SkudViewModel();
			_employeeCardIndexViewModel = new EmployeeCardIndexViewModel();
			_employeeCardIndexViewModel.Initialize();

			_employeeDepartmentsViewModel = new EmployeeDepartmentsViewModel();
			_employeeGroupsViewModel = new EmployeeGroupsViewModel();
			_employeePositionsViewModel = new EmployeePositionsViewModel();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("СКУД", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowEmployeeCardIndexEvent>("Картотека",null),
					new NavigationItem<ShowPassCardEvent>("Пропуск",null),
					new NavigationItem("Справочники",null, new List<NavigationItem>()
					{
						new NavigationItem<ShowEmployeePositionsEvent>("Должности",null),
						new NavigationItem<ShowEmployeeDepartmentsEvent>("Подразделения",null),
						new NavigationItem<ShowEmployeeGroupsEvent>("Группы",null),
					})
				}, PermissionType.Adm_SKUD),
			};
		}
	}
}