using System.Collections.Generic;
using FireMonitor.Layout.ViewModels;
using FiresecAPI.Models.Layouts;
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
		public override string Name
		{
			get { return "Монитор"; }
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter()
			{
				Name = "Заглушка",
				UID = LayoutPartIdentities.EmptySpace,
				IconSource = "/Controls;component/Images/Exit.png",
				Factory = (p) => new EmptyPartViewModel(),
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Индикаторы",
				UID = LayoutPartIdentities.Indicator,
				IconSource = "/Controls;component/Images/Alarm.png",
				Factory = (p) => MonitorLayoutShellViewModel.Toolbar,
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Навигатор",
				UID = LayoutPartIdentities.Navigation,
				IconSource = "/Controls;component/Images/Tree.png",
				Factory = (p) => new NavigationPartViewModel(MonitorLayoutShellViewModel),
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Контейнер",
				UID = LayoutPartIdentities.Content,
				IconSource = "/Controls;component/Images/Layouts.png",
				Factory = (p) => new ContentPartViewModel(MonitorLayoutShellViewModel),
			};
			yield return new LayoutPartPresenter()
			{
				Name = "Картинка",
				UID = LayoutPartIdentities.Image,
				IconSource = "/Controls;component/Images/View.png",
				Factory = (p) => new ImagePartViewModel(p as LayoutPartImageProperties),
			};
		}

		#endregion
	}
}