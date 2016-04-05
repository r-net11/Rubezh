using System.Collections.Generic;
using System.Windows;
using FireAdministrator.ViewModels;
using FiresecAPI.Enums;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;

namespace FireAdministrator
{
	public class AdministratorModule : ModuleBase, ILayoutDeclarationModule
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
			get { return ModuleType.Administrator; }
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Navigation, 2, "Навигатор", "Панель навигации", "BTree.png", false, new LayoutPartSize() { PreferedSize = new Size(150, 500) });
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Content, 3, "Контейнер", "Контейнер содержания", "BLayouts.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.TimePresenter, 5, "Часы", "Панель отображающая время", "BTime.png", true, new LayoutPartSize() { PreferedSize = new Size(220, 30) })
			{
				Factory = (p) => new LayoutPartTimeViewModel(p as LayoutPartTimeProperties),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Reports, 201, "Отчеты", "Панель отчетов", "BLevels.png");
		}
		#endregion
	}
}