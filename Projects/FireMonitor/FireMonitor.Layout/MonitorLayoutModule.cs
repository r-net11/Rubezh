using System.Collections.Generic;
using FireMonitor.Layout.ViewModels;
using FireMonitor.ViewModels;
using FiresecAPI.Enums;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;

namespace FireMonitor.Layout
{
	public class MonitorLayoutModule : ModuleBase, ILayoutProviderModule
	{
		internal MonitorLayoutShellViewModel MonitorLayoutShellViewModel { get; set; }

		public override void CreateViewModels()
		{
		}
		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>();
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Monitor; }
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}
		public override void AfterInitialize()
		{
			base.AfterInitialize();
			MonitorLayoutShellViewModel.ListenAutomationEvents();
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.EmptySpace, "Пространство", "Exit.png", (p) => new EmptyPartViewModel());
			yield return new LayoutPartPresenter(LayoutPartIdentities.Indicator, "Индикаторы", "Alarm.png", (p) => ((ToolbarViewModel)MonitorLayoutShellViewModel.Toolbar).AlarmGroups);
			yield return new LayoutPartPresenter(LayoutPartIdentities.Navigation, "Навигатор", "Tree.png", (p) => new NavigationPartViewModel(MonitorLayoutShellViewModel));
			yield return new LayoutPartPresenter(LayoutPartIdentities.Content, "Контейнер", "Layouts.png", (p) => new ContentPartViewModel(MonitorLayoutShellViewModel));
			yield return new LayoutPartPresenter(LayoutPartIdentities.Image, "Картинка", "View.png", (p) => new ImagePartViewModel(p as LayoutPartImageProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TemplateContainer, "Макет", "TemplateContainer.png", (p) => new TemplateContainerPartViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TimePresenter, "Часы", "Time.png", (p) => new TimePresenterViewModel(p as LayoutPartTimeProperties));
		}

		#endregion
	}
}