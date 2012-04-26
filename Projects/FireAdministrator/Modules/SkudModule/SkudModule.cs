using System;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SkudModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace SkudModule
{
	public class SkudModule : ModuleBase
	{
		private static SkudViewModel _skudViewModel;
		private static EmployeeCardIndexViewModel _employeeCardIndexViewModel;
		private static EmployeeDepartmentsViewModel _employeeDepartmentsViewModel;
		private static EmployeePositionsViewModel _employeePositionsViewModel;
		private static EmployeeGroupsViewModel _employeeGroupsViewModel;

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

		private static void OnShowSkud(object obj)
		{
			//ServiceFactory.Layout.Show(_skudViewModel);
		}
		private static void OnShowEmployeeCardIndexEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeCardIndexViewModel);
		}
		private static void OnShowPassCardEvent(object obj)
		{
			ServiceFactory.Layout.Show(_skudViewModel);
		}

		private static void OnShowSkudDepartmentsEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeDepartmentsViewModel);
		}
		private static void OnShowSkudGroupsEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeGroupsViewModel);
		}
		private static void OnShowSkudPositionsEvent(object obj)
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
				new NavigationItem("СКУД", null, null, new List<NavigationItem>()
				{
					new NavigationItem("Картотека",null, typeof(ShowEmployeeCardIndexEvent)),
					new NavigationItem("Пропуск",null, typeof(ShowPassCardEvent)),
					new NavigationItem("Справочники",null, null, new List<NavigationItem>()
					{
						new NavigationItem("Должности",null, typeof(ShowEmployeePositionsEvent)),
						new NavigationItem("Подразделения",null, typeof(ShowEmployeeDepartmentsEvent)),
						new NavigationItem("Группы",null, typeof(ShowEmployeeGroupsEvent)),
					})
				}),
				//new NavigationItem("СКУД1", null, null, new List<NavigationItem>()),
				//new NavigationItem("СКУД2", null, null, new List<NavigationItem>()),
				//new NavigationItem("СКУД3", null, null, new List<NavigationItem>()),
				//new NavigationItem("СКУД4", null, null, new List<NavigationItem>()),
				//new NavigationItem("СКУД5", null, null, new List<NavigationItem>()),
				//new NavigationItem("СКУД6", null, null, new List<NavigationItem>())
			};
		}
	}
}