using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using AutomationModule.ViewModels;
using AutomationModule.Events;

namespace AutomationModule
{
	public class AutomationModuleLoader : ModuleBase
	{
		ProceduresViewModel ProceduresViewModel;
		NavigationItem _proceduresNavigationItem;

		public override void CreateViewModels()
		{
			ProceduresViewModel = new ProceduresViewModel();
			ProcessShedule();
		}

		void ProcessShedule()
		{
		}

		public override void Initialize()
		{
			_proceduresNavigationItem.IsVisible = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Count > 0;
			ProceduresViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_proceduresNavigationItem = new NavigationItem<ShowAutomationEvent, object>(ProceduresViewModel, "Автоматизация", "/Controls;component/Images/Video1.png");
			return new List<NavigationItem>()
			{
				_proceduresNavigationItem
			};
		}

		public override string Name
		{
			get { return "Автоматизация"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}
		public override void Dispose()
		{
		}
	}
}