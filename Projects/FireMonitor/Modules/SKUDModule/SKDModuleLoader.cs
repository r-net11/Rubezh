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

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Devices/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Zones/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Verification/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Access/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Cards/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Departments/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Positions/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Documents/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "AdditionalColumns/DataTemplates/Dictionary.xaml"));
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
	}
}