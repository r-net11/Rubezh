using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SkudModule.ViewModels;

namespace SkudModule
{
	public class SkudModule : ModuleBase
	{
		private SkudViewModel _skudViewModel;
		private EmployeeCardIndexViewModel _employeeCardIndexViewModel;
		private EmployeeDepartmentsViewModel _employeeDepartmentsViewModel;
		private EmployeePositionsViewModel _employeePositionsViewModel;
		private EmployeeGroupsViewModel _employeeGroupsViewModel;
		private PassCardsDesignerViewModel _passCardDesignerViewModel;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Unsubscribe(OnShowSkud);
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Subscribe(OnShowSkud);

			_skudViewModel = new SkudViewModel();
			_employeeCardIndexViewModel = new EmployeeCardIndexViewModel();
			_employeeDepartmentsViewModel = new EmployeeDepartmentsViewModel();
			_employeeGroupsViewModel = new EmployeeGroupsViewModel();
			_employeePositionsViewModel = new EmployeePositionsViewModel();
			_passCardDesignerViewModel = new PassCardsDesignerViewModel();
		}

		private void OnShowSkud(object obj)
		{
			//ServiceFactory.Layout.Show(_skudViewModel);
		}

		public override void Initialize()
		{
			_skudViewModel.Initialize();
			_employeeCardIndexViewModel.Initialize();

			_employeeDepartmentsViewModel.Initialize();
			_employeeGroupsViewModel.Initialize();
			_employeePositionsViewModel.Initialize();
			_passCardDesignerViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Adm_SKUD))
				return null;

			return new List<NavigationItem>()
			{
				new NavigationItem("СКУД", null, new List<NavigationItem>()
				{
					new NavigationItem<ShowEmployeeCardIndexEvent>(_employeeCardIndexViewModel, "Картотека",null),
					new NavigationItem<ShowPassCardEvent>(_skudViewModel, "Пропуск",null),
					new NavigationItem<ShowPassCardDesignerEvent>(_passCardDesignerViewModel, "Дизайнер",null),
					new NavigationItem("Справочники",null, new List<NavigationItem>()
					{
						new NavigationItem<ShowEmployeePositionsEvent>(_employeePositionsViewModel, "Должности",null),
						new NavigationItem<ShowEmployeeDepartmentsEvent>(_employeeDepartmentsViewModel, "Подразделения",null),
						new NavigationItem<ShowEmployeeGroupsEvent>(_employeeGroupsViewModel, "Группы",null),
					})
				}, PermissionType.Adm_SKUD) {IsExpanded = true},
			};
		}
		public override string Name
		{
			get { return "СКУД"; }
		}
	}
}