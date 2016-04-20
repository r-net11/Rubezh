using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common.Services.Ribbon
{
	public class RibbonService : IRibbonService
	{
		private object _locker = new object();

		#region IRibbonService Members

		public ObservableCollection<RibbonMenuItemViewModel> Items
		{
			get { return ApplicationService.Shell.RibbonContent.Items; }
		}

		public void AddRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems)
		{
			ForEach(ribbonMenuItems, item =>
			{
				if (!ApplicationService.Shell.RibbonContent.Items.Contains(item))
					ApplicationService.Shell.RibbonContent.Items.Add(item);
			});
		}
		public void RemoveRibbonItems(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems)
		{
			ForEach(ribbonMenuItems, item => ApplicationService.Shell.RibbonContent.Items.Remove(item));
		}

		public void AddRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems)
		{
			AddRibbonItems((IEnumerable<RibbonMenuItemViewModel>)ribbonMenuItems);
		}
		public void RemoveRibbonItems(params RibbonMenuItemViewModel[] ribbonMenuItems)
		{
			RemoveRibbonItems((IEnumerable<RibbonMenuItemViewModel>)ribbonMenuItems);
		}

		#endregion

		private void ForEach(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems, Action<RibbonMenuItemViewModel> action)
		{
			if (ribbonMenuItems != null)
				foreach (var ribbonMenuItem in ribbonMenuItems)
					action(ribbonMenuItem);
		}
	}
}
