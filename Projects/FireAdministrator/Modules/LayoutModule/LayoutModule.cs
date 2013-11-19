using System;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Client;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using LayoutModule.ViewModels;

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

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации макетов ОЗ");
			FiresecManager.LayoutsConfiguration.Update();
			return true;
		}
	}
}