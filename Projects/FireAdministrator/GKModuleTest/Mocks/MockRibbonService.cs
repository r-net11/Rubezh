﻿using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services.Ribbon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKModuleTest
{
	public class MockRibbonService : IRibbonService
	{
		public ObservableCollection<RibbonMenuItemViewModel> Items
		{
			get { return new ObservableCollection<RibbonMenuItemViewModel>(); }
		}

		public void AddRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems)
		{
		}

		public void RemoveRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems)
		{
		}

		public void AddRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems)
		{
		}

		public void RemoveRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems)
		{
		}
	}
}