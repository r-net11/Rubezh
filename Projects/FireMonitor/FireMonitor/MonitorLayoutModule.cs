using FireMonitor.Layout.ViewModels;
using FireMonitor.ViewModels;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;

namespace FireMonitor
{
	public class MonitorLayoutModule : ModuleBase, ILayoutProviderModule
	{
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

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.EmptySpace, "Пространство", "Exit.png", (p) => new EmptyPartViewModel());
			yield return new LayoutPartPresenter(LayoutPartIdentities.Indicator, "Индикаторы", "Alarm.png", (p) => ((ToolbarViewModel)Bootstrapper.ShellViewModel.Toolbar).AlarmGroups);
			yield return new LayoutPartPresenter(LayoutPartIdentities.Image, "Картинка", "View.png", (p) => new ImagePartViewModel(p as LayoutPartImageProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TemplateContainer, "Макет", "TemplateContainer.png", (p) => new TemplateContainerPartViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TimePresenter, "Часы", "Time.png", (p) => new TimePresenterViewModel(p as LayoutPartTimeProperties));
		}

		#endregion
	}
}