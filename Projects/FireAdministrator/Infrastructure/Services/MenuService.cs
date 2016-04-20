using System;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Services
{
	public class MenuService
	{
		private Action<BaseViewModel> _action;
		public MenuService(Action<BaseViewModel> action)
		{
			_action = action;
		}

		public void Show(BaseViewModel menu)
		{
			_action(menu);
		}
	}
}