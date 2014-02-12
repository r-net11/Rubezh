using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.Plans;
using SKDModule.ViewModels;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Client.Layout;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase, ILayoutProviderModule
	{
		EmployeesViewModel EmployeesViewModel;
		JournalViewModel JournalViewModel;
		DevicesViewModel DevicesViewModel;
		ZonesViewModel ZonesViewModel;
		VerificationViewModel VerificationViewModel;
		UsersAccessViewModel UsersAccessViewModel;
		CardsViewModel CardsViewModel;
		DepartmentsViewModel DepartmentsViewModel;
		PositionsViewModel PositionsViewModel;
		DocumentsViewModel DocumentsViewModel;
		AdditionalColumnsViewModel AdditionalColumnsViewModel;
		TimeIntervalsViewModel TimeIntervalsViewModel;
		WeeklyIntervalsViewModel WeeklyIntervalsViewModel;
		SlideDayIntervalsViewModel SlideDayIntervalsViewModel;
		SlideWeekIntervalsViewModel SlideWeekIntervalsViewModel;
		HolidaysViewModel HolidaysViewModel;
		private PlanPresenter _planPresenter;

		public SKDModuleLoader()
		{
			_planPresenter = new PlanPresenter();
		}

		public override void CreateViewModels()
		{
			EmployeesViewModel = new EmployeesViewModel();
			JournalViewModel = new JournalViewModel();
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			VerificationViewModel = new VerificationViewModel();
			UsersAccessViewModel = new UsersAccessViewModel();
			CardsViewModel = new CardsViewModel();
			DepartmentsViewModel = new DepartmentsViewModel();
			PositionsViewModel = new PositionsViewModel();
			DocumentsViewModel = new DocumentsViewModel();
			AdditionalColumnsViewModel = new AdditionalColumnsViewModel();
			TimeIntervalsViewModel = new TimeIntervalsViewModel();
			WeeklyIntervalsViewModel = new WeeklyIntervalsViewModel();
			SlideDayIntervalsViewModel = new SlideDayIntervalsViewModel();
			SlideWeekIntervalsViewModel = new SlideWeekIntervalsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
				new NavigationItem("СКД", "/Controls;component/Images/tree.png",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowSKDEmployeesEvent>(EmployeesViewModel, "Сотрудники", "/Controls;component/Images/levels.png"),
						new NavigationItem<ShowSKDJournalEvent>(JournalViewModel, "Журнал", "/Controls;component/Images/levels.png"),
						new NavigationItem<ShowSKDDeviceEvent, Guid>(DevicesViewModel, "Устройства", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDZoneEvent, Guid>(ZonesViewModel, "Зоны", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						new NavigationItem<ShowSKDVerificationEvent>(VerificationViewModel, "Верификация", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDUsersAccessEvent>(UsersAccessViewModel, "Доступ сотрудников", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDCardsEvent>(CardsViewModel, "Карты", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDDepartmentsEvent>(DepartmentsViewModel, "Отделы", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDPositionsEvent>(PositionsViewModel, "Должности", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDDocumentsEvent>(DocumentsViewModel, "Документы", "/Controls;component/Images/tree.png"),
						new NavigationItem<ShowSKDAdditionalColumnsEvent>(AdditionalColumnsViewModel, "Дополнительные колонки", "/Controls;component/Images/tree.png"),
						new NavigationItem("Интервалы", null, new List<NavigationItem>()
						{
							new NavigationItem<ShowSKDTimeIntervalsEvent, Guid>(TimeIntervalsViewModel, "Именованные интервалы", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowSKDWeeklyIntervalsEvent, Guid>(WeeklyIntervalsViewModel, "Недельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowSKDSlideDayIntervalsEvent, Guid>(SlideDayIntervalsViewModel, "Скользящие посуточные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowSKDSlideWeekIntervalsEvent, Guid>(SlideWeekIntervalsViewModel, "Скользящие понедельные графики", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
							new NavigationItem<ShowSKDHolidaysEvent, Guid>(HolidaysViewModel, "Праздничные дни", "/Controls;component/Images/tree.png", null, null, Guid.Empty),
						}),
					})
				};
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Publish(_planPresenter);
			DevicesViewModel.Initialize();
			ZonesViewModel.Initialize();
		}

		public override string Name
		{
			get { return "СКД"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Employees/DataTemplates/Dictionary.xaml"));
			//resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Verification/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Access/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Cards/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Departments/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Positions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Documents/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AdditionalColumns/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Intervals/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			SKDManager.CreateStates();
			return true;
		}
		public override void AfterInitialize()
		{
			SafeFiresecService.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResult);
			SafeFiresecService.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResult);
			ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			AutoActivationWatcher.Run();
		}

		void OnSKDCallbackResult(SKDCallbackResult skdCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (skdCallbackResult.JournalItems.Count > 0)
				{
					ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Publish(skdCallbackResult.JournalItems);
				}
				CopySKDStates(skdCallbackResult.SKDStates);
				ServiceFactoryBase.Events.GetEvent<SKDObjectsStateChangedEvent>().Publish(null);
			});
		}
		void CopySKDStates(SKDStates skdStates)
		{
			foreach (var remoteDeviceState in skdStates.DeviceStates)
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
					device.State.OnStateChanged();
				}
			}
		}


		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDEmployees, "Сотрудники", "BLevels.png", (p) => EmployeesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDJournal, "Журнал", "BLevels.png", (p) => JournalViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDevices, "СКД устройства", "BTree.png", (p) => DevicesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDZones, "СКД зоны", "BTree.png", (p) => ZonesViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDVerification, "Верификация", "BTree.png", (p) => VerificationViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDUsersAccess, "Доступ сотрудников", "BTree.png", (p) => UsersAccessViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDCards, "Карты", "BTree.png", (p) => CardsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDepartments, "Отделы", "BTree.png", (p) => DepartmentsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDPositions, "Должности", "BTree.png", (p) => PositionsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDDocuments, "Документы", "BTree.png", (p) => DocumentsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDAdditionalColumns, "Дополнительные колонки", "BTree.png", (p) => AdditionalColumnsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDTimeIntervals, "Именованные интервалы", "BTree.png", (p) => TimeIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDWeeklyIntervals, "Недельные графики", "BTree.png", (p) => WeeklyIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDSlideDayIntervals, "Скользящие посуточные графики", "BTree.png", (p) => SlideDayIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDSlideWeekIntervals, "Скользящие понедельные графики", "BTree.png", (p) => SlideWeekIntervalsViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.SKDHolidays, "Праздничные дни", "BTree.png", (p) => HolidaysViewModel);
		}

		#endregion
	}
}