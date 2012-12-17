using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using Microsoft.Practices.Prism.Events;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerCanvas : Canvas
	{
		protected virtual Canvas SelectedCanvas { get; set; }
		public virtual double Zoom { get { return 1; } }
		public virtual double PointZoom { get { return 10; } }

		public CommonDesignerCanvas(IEventAggregator eventAggregator)
		{
			DataContext = this;
			EventService.RegisterEventAggregator(eventAggregator);
			//Items = new List<DesignerItem>();
			ClipToBounds = true;
		}

		public abstract void BeginChange(IEnumerable<DesignerItem> designerItems);
		public abstract void BeginChange();
		public abstract void EndChange();

		public virtual void Clear()
		{
			Children.Clear();
			//Items.Clear();
		}
		protected void AddCanvas()
		{
			SelectedCanvas = new Canvas();
			Children.Add(SelectedCanvas);
		}
		protected void RemoveCanvas()
		{
			Children.Remove(SelectedCanvas);
			SelectedCanvas = null;
		}

		public void Remove(DesignerItem designerItem)
		{
			SelectedCanvas.Children.Remove(designerItem);
			//Items.Remove(designerItem);
		}
		public void Add(DesignerItem designerItem)
		{
			designerItem.DesignerCanvas = this;
			designerItem.UpdateAdornerLayout();
			SelectedCanvas.Children.Add(designerItem);
			//Items.Add(designerItem);
		}
		public double CanvasWidth
		{
			get { return SelectedCanvas.Width; }
			set
			{
				Width = value;
				SelectedCanvas.Width = value;
			}
		}
		public double CanvasHeight
		{
			get { return SelectedCanvas.Height; }
			set
			{
				Height = value;
				SelectedCanvas.Height = value;
			}
		}
		public Brush CanvasBackground
		{
			get { return SelectedCanvas.Background; }
			set { SelectedCanvas.Background = value; }
		}

		//public List<DesignerItem> Items { get; private set; }
		//public IEnumerable<DesignerItem> SelectedItems
		//{
		//    get { return from item in Items where item.IsSelected select item; }
		//}
		//public IEnumerable<ElementBase> SelectedElements
		//{
		//    get { return from item in Items where item.IsSelected select item.Element; }
		//}
		public IEnumerable<DesignerItem> Items
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<DesignerItem>() : from item in SelectedCanvas.Children.OfType<DesignerItem>() select item; }
		}
		public IEnumerable<DesignerItem> SelectedItems
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<DesignerItem>() : from item in SelectedCanvas.Children.OfType<DesignerItem>() where item.IsSelected == true select item; }
		}
		public IEnumerable<ElementBase> SelectedElements
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<ElementBase>() : from item in SelectedCanvas.Children.OfType<DesignerItem>() where item.IsSelected == true select item.Element; }
		}

		public void SelectAll()
		{
			if (SelectedCanvas != null)
				foreach (var designerItem in Items)
					designerItem.IsSelected = true;
		}
		public void DeselectAll()
		{
			if (SelectedCanvas != null)
				foreach (DesignerItem item in this.SelectedItems)
					item.IsSelected = false;
		}

		public abstract void Update();
		public abstract void CreateDesignerItem(ElementBase element);
		public abstract void Remove(List<Guid> elementUIDs);
	}
}