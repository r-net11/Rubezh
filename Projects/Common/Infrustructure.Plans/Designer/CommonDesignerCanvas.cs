using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerCanvas : Canvas
	{
		public virtual double Zoom { get { return 1; } }
		public virtual double PointZoom { get { return 10; } }

		public CommonDesignerCanvas(IEventAggregator eventAggregator)
		{
			EventService.RegisterEventAggregator(eventAggregator);
		}

		public abstract void BeginChange(IEnumerable<DesignerItem> designerItems);
		public abstract void BeginChange();
		public abstract void EndChange();

		public IEnumerable<DesignerItem> Items
		{
			get { return from item in this.Children.OfType<DesignerItem>() select item; }
		}
		public IEnumerable<DesignerItem> SelectedItems
		{
			get { return from item in this.Children.OfType<DesignerItem>() where item.IsSelected == true select item; }
		}
		public IEnumerable<ElementBase> SelectedElements
		{
			get { return from item in this.Children.OfType<DesignerItem>() where item.IsSelected == true select item.Element; }
		}

		public void SelectAll()
		{
			foreach (var designerItem in Items)
				designerItem.IsSelected = true;
		}
		public void DeselectAll()
		{
			foreach (DesignerItem item in this.SelectedItems)
				item.IsSelected = false;
		}

		public abstract void Update();
		public abstract void CreateDesignerItem(ElementBase element);
		public abstract void Remove(List<Guid> elementUIDs);
	}
}