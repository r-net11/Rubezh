using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Client;
using LayoutModule.ViewModels;
using FiresecClient;

namespace LayoutModule
{
	public class LayoutModule : ModuleBase
	{
		private MonitorLayoutsViewModel _monitorLayoutsViewModel;

		public override void CreateViewModels()
		{
			_monitorLayoutsViewModel = new MonitorLayoutsViewModel();
		}
		public override void Initialize()
		{
			_monitorLayoutsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
				{
					new NavigationItem<ShowMonitorLayoutEvent, Guid>(_monitorLayoutsViewModel, "Макеты ОЗ", "/Controls;component/Images/Layouts.png", null, null, Guid.Empty),
				};
		}
		public override string Name
		{
			get { return "Конфигуратор макетов ОЗ"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			//var resourceService = new ResourceService();
			//resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
			//resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Parameters/DataTemplates/Dictionary.xaml"));
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации макетов ОЗ");
			//GKDriversCreator.Create();
			//XManager.UpdateConfiguration();
			return true;
		}
	}
}