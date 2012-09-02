using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.ViewModels
{
	public class MenuViewPartViewModel : ViewPartViewModel
	{
		public BaseViewModel Menu { get; protected set; }

		public MenuViewPartViewModel()
		{
			Menu = null;
		}

		public override void OnShow()
		{
			base.OnShow();
			ServiceFactory.MenuService.Show(Menu);
		}
		public override void OnHide()
		{
			base.OnHide();
			ServiceFactory.MenuService.Show(null);
		}
	}
}
