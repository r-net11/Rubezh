﻿using System.Collections.Generic;
using DiagnosticsModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace DiagnosticsModule
{
    public class DiagnosticsModuleLoader : ModuleBase
    {
        DiagnosticsViewModel DiagnosticsViewModel;

		public override void CreateViewModels()
        {
            ServiceFactory.Layout.AddToolbarItem(new ImitatorViewModel());
            DiagnosticsViewModel = new DiagnosticsViewModel();
        }

		public override void Initialize()
		{
		}
		public override void AfterInitialize()
		{
			//DiagnosticsViewModel.TestPdf2Command.Execute();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			    new NavigationItem<ShowDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png")
			};
		}
		public override string Name
		{
            get { return "Диагностика"; }
		}
		public override void Dispose()
		{
			DiagnosticsViewModel.StopThreads();
		}
	}
}