using System.Collections.Generic;
using System.Windows;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
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
		public override string Name
		{
			get { return "Администратор"; }
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartIdentities.Indicator, 1, "Индикаторы", "Панель индикаторов состояния", "BAlarm.png", false, new LayoutPartSize() { PreferedSize = new Size(1000, 100) });
			yield return new LayoutPartDescription(LayoutPartIdentities.Navigation, 2, "Навигатор", "Панель навигации", "BTree.png", false, new LayoutPartSize() { PreferedSize = new Size(150, 500) });
			yield return new LayoutPartDescription(LayoutPartIdentities.Content, 3, "Контейнер", "Контейнер содержания", "BLayouts.png", false);
			yield return new LayoutPartDescription(LayoutPartIdentities.Reports, 201, "Отчеты", "Панель отчетов", "BLevels.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Diagnostics, 202, "Диагностика", "Панель диагностики", "BBug.png");
		}

		#endregion
	}
}