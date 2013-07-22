using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;

namespace Controls.Ribbon.Views
{
	public class RibbonMenuView : TabControl
	{
		public RibbonMenuView()
		{
			TabStripPlacement = Dock.Left;
		}
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
			var view = CollectionViewSource.GetDefaultView(ItemsSource);
			if (view != null)
				view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
		}

		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
				SelectedIndex = -1;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new RibbonMenuItemView();
		}
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is RibbonMenuItemView;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}
