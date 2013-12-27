using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

using FiresecClient;
using System;
using System.Linq;
using Infrastructure;
using SKUDModule.ViewModels;
using Microsoft.Practices.Prism.Events;
using System.Diagnostics;

namespace SKUDModule
{
	public class SKUDModuleLoader:ModuleBase
	{
		SKUDViewModel SKUDViewModel;
		NavigationItem _skudNavigationItem;

		public override void CreateViewModels()
		{
			SKUDViewModel = new SKUDViewModel();
		}
		
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_skudNavigationItem = new NavigationItem<ShowSKUDEvent>(SKUDViewModel, "СКУД", "/Controls;component/Images/levels.png");
			return new List<NavigationItem>()
		    {
		        _skudNavigationItem
		    };
		}

		public override void Initialize()
		{
			;
		}

		public override string Name
		{
			get { return "СКУД"; }
		}

	}
}
