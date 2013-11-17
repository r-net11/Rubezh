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

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription()
			{
				Name = "XXXX",
				Description = "xxxx xxx xxxx",
				Index = 10,
				UID = new Guid("{8B0BF10A-AD4C-48CC-B1D0-ADF39552B936}"),
				IconSource = "/Controls;component/Images/BCopy.png",
				AllowMultiple = true,
			};
			yield return new LayoutPartDescription()
			{
				Name = "YYYY",
				Description = "yyyy yyyyyyy yyyy",
				Index = 1,
				UID = new Guid("{8EC166C3-5D1C-4BBD-AE33-A91EC58FB74B}"),
				IconSource = "/Controls;component/Images/BCut.png",
				AllowMultiple = false,
				Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BCut.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "ZZZZ",
				Description = "zzzz zzzz zzzzzzzzzzzzzzzzz",
				Index = 100,
				UID = new Guid("{7FA6996D-0E0F-4409-BFF9-064EFEAF5C35}"),
				IconSource = "/Controls;component/Images/BPaste.png",
				AllowMultiple = true,
				Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BPaste.png" },
			};
		}

		#endregion
	}
}