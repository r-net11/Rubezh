using FireAdministrator.ViewModels;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Windows;

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
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.TimePresenter, 5, "Часы", "Панель отображающая время", "BTime.png", false, new LayoutPartSize() { PreferedSize = new Size(220, 30) })
			{
				Factory = (p) => new LayoutPartTimeViewModel(p as LayoutPartTimeProperties),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Reports, 201, "Отчеты", "Панель отчетов", "BLevels.png");
			//yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Diagnostics, 202, "Диагностика", "Панель диагностики", "BBug.png");
		}
		#endregion
	}
}