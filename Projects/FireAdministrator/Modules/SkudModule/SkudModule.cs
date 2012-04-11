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

		public SkudModule()
		{
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Unsubscribe(OnShowSkud);
			ServiceFactory.Events.GetEvent<ShowSkudEvent>().Subscribe(OnShowSkud);
			ServiceFactory.Events.GetEvent<ShowEmployeeCardIndexEvent>().Unsubscribe(OnShowEmployeeCardIndexEvent);
			ServiceFactory.Events.GetEvent<ShowEmployeeCardIndexEvent>().Subscribe(OnShowEmployeeCardIndexEvent);
			ServiceFactory.Events.GetEvent<ShowPassCardEvent>().Unsubscribe(OnShowPassCardEvent);
			ServiceFactory.Events.GetEvent<ShowPassCardEvent>().Subscribe(OnShowPassCardEvent);

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
		}

		private static void OnShowSkud(object obj)
		{
			ServiceFactory.Layout.Show(_skudViewModel);
		}
		private static void OnShowEmployeeCardIndexEvent(object obj)
		{
			ServiceFactory.Layout.Show(_employeeCardIndexViewModel);
		}
		private static void OnShowPassCardEvent(object obj)
		{
			ServiceFactory.Layout.Show(_skudViewModel);
		}
	}
}