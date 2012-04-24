using System;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SkudModule.ViewModels;

namespace SkudModule
{
	public class SkudModule
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

			RegisterResources();
			CreateViewModels();
		}

		void RegisterResources()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}

		static void CreateViewModels()
		{
			_skudViewModel = new SkudViewModel();
			_employeeCardIndexViewModel = new EmployeeCardIndexViewModel();
			_employeeCardIndexViewModel.Initialize();

			_employeeDepartmentsViewModel = new EmployeeDepartmentsViewModel();
			_employeeGroupsViewModel = new EmployeeGroupsViewModel();
			_employeePositionsViewModel = new EmployeePositionsViewModel();
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

	}
}