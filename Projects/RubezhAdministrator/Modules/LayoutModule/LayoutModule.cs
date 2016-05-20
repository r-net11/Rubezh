using System;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using LayoutModule.ViewModels;
using Infrastructure.Common.Validation;
using LayoutModule.Validation;
using LayoutModule.LayoutParts.ViewModels;
using RubezhAPI.Automation;

namespace LayoutModule
{
	public class LayoutModule : ModuleBase, ILayoutDeclarationModule, IValidationModule
	{
		MonitorLayoutsViewModel _monitorLayoutsViewModel;

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
				new NavigationItem<ShowMonitorLayoutEvent, Guid>(_monitorLayoutsViewModel, ModuleType.ToDescription(), "Layouts", null, null, Guid.Empty),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Layout; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "LayoutParts/DataTemplates/Dictionary.xaml");
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			LoadingService.DoStep("Загрузка конфигурации макетов ОЗ");
			ClientManager.LayoutsConfiguration.Update();
			return true;
		}

		void OnBeforeConfigurationSerialize(object obj)
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
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.EmptySpace, 0, "Пространство", "Свободное пространство", "BExit.png")
			{
				Factory = (p) => new LayoutPartEmptyViewModel(),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Image, 4, "Картинка", "Показывает статическое изображение", "BView.png")
			{
				Factory = (p) => new LayoutPartImageViewModel(p as LayoutPartImageProperties),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Control, LayoutPartIdentities.TextBlock, 1001, "Метка", "Метка", "BText.png", new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(string), LayoutPartPropertyName.Text))
			{
				Factory = (p) => new LayoutPartTextViewModel(p as LayoutPartTextProperties, false),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Control, LayoutPartIdentities.TextBox, 1002, "Текстовое поле", "Текстовое поле", "BText.png", new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(string), LayoutPartPropertyName.Text))
			{
				Factory = (p) => new LayoutPartTextViewModel(p as LayoutPartTextProperties, true),
			};
		}

		#endregion

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			return new Validator().Validate();
		}

		#endregion
	}
}