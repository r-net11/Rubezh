using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common.Services.Ribbon
{
	public interface IRibbonService
	{
		void AddRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems);
		void RemoveRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems);
		void AddRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems);
		void RemoveRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems);
	}
}
