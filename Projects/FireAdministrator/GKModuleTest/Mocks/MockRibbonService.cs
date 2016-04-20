using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Services.Ribbon;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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