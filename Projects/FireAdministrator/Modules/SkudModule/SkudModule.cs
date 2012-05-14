using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SkudModule.ViewModels;

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
				new NavigationItem("Устройства", "/Controls;component/Images/tree.png", new List<NavigationItem>()),
				new NavigationItem("СКУД", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowSkudEvent>("Картотека",null),
					new NavigationItem("Пропуск",null),
					new NavigationItem<ShowSkudEvent>("Справочники",null, new List<NavigationItem>()
					{
						new NavigationItem("Должности",null),
						new NavigationItem<ShowSkudEvent>("Подразделения",null),
						new NavigationItem("Группы",null),
					})
				}),
				new NavigationItem("СКУД1", null, new List<NavigationItem>()),
				new NavigationItem("СКУД2", null, new List<NavigationItem>()),
				new NavigationItem<ShowSkudEvent>("СКУД3", null,  new List<NavigationItem>()),
				new NavigationItem("СКУД5", null, new List<NavigationItem>()),
				new NavigationItem("СКУД6", null, new List<NavigationItem>())
			};
		}
	}
}