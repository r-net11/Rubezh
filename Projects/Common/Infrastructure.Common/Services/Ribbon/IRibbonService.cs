﻿using Infrastructure.Common.Ribbon;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Services.Ribbon
{
	public interface IRibbonService
	{
		ObservableCollection<RibbonMenuItemViewModel> Items { get; }

		void AddRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems);

		void RemoveRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems);

		void AddRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems);

		void RemoveRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems);
	}
}