using System.Collections.Generic;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.ViewModels
{
	public class MenuViewPartViewModel : ViewPartViewModel
	{
		public BaseViewModel Menu { get; protected set; }
		public List<RibbonMenuItemViewModel> RibbonItems { get; protected set; }

		public MenuViewPartViewModel()
		{
			Menu = null;
			RibbonItems = CreateRibbonItems();
		}

		protected virtual List<RibbonMenuItemViewModel> CreateRibbonItems()
		{
			return null;
		}
		protected virtual void UpdateRibbonItems()
		{
		}

		public override void OnShow()
		{
			base.OnShow();
			ServiceFactory.MenuService.Show(Menu);
			UpdateRibbonItems();
			ServiceFactory.RibbonService.AddRibbonItems(RibbonItems);
		}
		public override void OnHide()
		{
			base.OnHide();
			ServiceFactory.MenuService.Show(null);
			ServiceFactory.RibbonService.RemoveRibbonItems(RibbonItems);
		}
	}
}