using System;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using LayoutModule.ViewModels;

namespace LayoutModule
{
	public class LayoutModule : ModuleBase, ILayoutDeclarationModule
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
			FiresecManager.LayoutsConfiguration.Update();
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription()
			{
				Name = "XXXX",
				Description = "xxxx xxx xxxx",
				Index = 10,
				UID = Guid.NewGuid(),
				ImageSource = "/Controls;component/Images/BCopy.png",
				AllowMultiple = true,
			};
			yield return new LayoutPartDescription()
			{
				Name = "YYYY",
				Description = "yyyy yyyyyyy yyyy",
				Index = 1,
				UID = Guid.NewGuid(),
				ImageSource = "/Controls;component/Images/BCut.png",
				AllowMultiple = false,
			};
			yield return new LayoutPartDescription()
			{
				Name = "ZZZZ",
				Description = "zzzz zzzz zzzzzzzzzzzzzzzzz",
				Index = 100,
				UID = Guid.NewGuid(),
				ImageSource = "/Controls;component/Images/BPaste.png",
				AllowMultiple = true,
			};
		}

		#endregion
	}
}