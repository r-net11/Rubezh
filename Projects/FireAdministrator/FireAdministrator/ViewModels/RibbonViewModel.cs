using System;
using System.ComponentModel;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class RibbonViewModel : BaseViewModel
	{
		public RibbonViewModel(MenuViewModel menuViewModel)
		{
			Menu = menuViewModel;
		}

		public MenuViewModel Menu { get; private set; }
	}
}