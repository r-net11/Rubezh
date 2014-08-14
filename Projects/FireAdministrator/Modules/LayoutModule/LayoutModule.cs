using System;
using System.Collections.Generic;
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
using LayoutModule.ViewModels;

namespace LayoutModule
{
	public class LayoutModule : ModuleBase, ILayoutDeclarationModule
	{
		private MonitorLayoutsViewModel _monitorLayoutsViewModel;

		public override void CreateViewModels()
		{
			_monitorLayoutsViewModel = new MonitorLayoutsViewModel();
			ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Unsubscribe(OnBeforeConfigurationSerialize);
			ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Subscribe(OnBeforeConfigurationSerialize);
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

		private void OnBeforeConfigurationSerialize(object obj)
		{
			_monitorLayoutsViewModel.SaveConfiguration();
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.TemplateContainer, 0, "Макет", "Вложенный макет", "BTemplateContainer.png")
			{
				Factory = (p) => new LayoutPartTemplateContainerViewModel(p as LayoutPartReferenceProperties),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.EmptySpace, 0, "Заглушка", "Пустая панель", "BExit.png")
			{
				Factory = (p) => new LayoutPartEmptyViewModel(),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Image, 4, "Картинка", "Показывает статическое изображение", "BView.png")
			{
				Factory = (p) => new LayoutPartImageViewModel(p as LayoutPartImageProperties),
			};
		}

		#endregion
	}
}