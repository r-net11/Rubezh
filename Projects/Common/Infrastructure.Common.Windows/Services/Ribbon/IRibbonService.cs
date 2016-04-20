using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.Ribbon;

namespace Infrastructure.Common.Windows.Services.Ribbon
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
