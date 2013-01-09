using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerCanvas : Canvas
	{
		protected virtual DesignerSurface SelectedCanvas { get; set; }
		public virtual double Zoom { get { return 1; } }
		public virtual double PointZoom { get { return CommonDesignerItem.DefaultPointSize; } }

		public CommonDesignerCanvas(IEventAggregator eventAggregator)
		{
			DataContext = this;
			EventService.RegisterEventAggregator(eventAggregator);
			ClipToBounds = true;
		}

		public abstract void BeginChange(IEnumerable<DesignerItem> designerItems);
		public abstract void BeginChange();
		public abstract void EndChange();

		public virtual void Clear()
		{
			Children.Clear();
		}
		protected void AddCanvas()
		{
			SelectedCanvas = new DesignerSurface();
			Children.Add(SelectedCanvas);
		}
		protected void RemoveCanvas()
		{
			Children.Remove(SelectedCanvas);
			SelectedCanvas = null;
		}

		public void Remove(DesignerItem designerItem)
		{
			SelectedCanvas.DeleteDesignerItem(designerItem);
		}
		public void Add(DesignerItem designerItem)
		{
			designerItem.DesignerCanvas = this;
			designerItem.UpdateAdornerLayout();
			SelectedCanvas.AddDesignerItem(designerItem);
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
		public void UpdateZIndex()
		{
			if (SelectedCanvas != null)
				SelectedCanvas.UpdateZIndex();
		}
		public void SetTitle(string title)
		{
			if (SelectedCanvas != null)
			{
				var toolTip = SelectedCanvas.ToolTip as ToolTip;
				if (toolTip != null)
					toolTip.IsOpen = false;
				SelectedCanvas.ToolTip = null;
				if (!string.IsNullOrEmpty(title))
				{
					toolTip = new ToolTip();
					toolTip.Content = title;
					toolTip.Placement = PlacementMode.MousePoint;
					SelectedCanvas.ToolTip = toolTip;

					toolTip.IsOpen = true;
				}
			}
		}

		public bool IsSurfaceMouseCaptured
		{
			get { return SelectedCanvas == null ? false : SelectedCanvas.IsMouseCaptured; }
		}
		public void SurfaceCaptureMouse()
		{
			if (SelectedCanvas != null)
				SelectedCanvas.CaptureMouse();
		}
		public void SurfaceReleaseMouseCapture()
		{
			if (SelectedCanvas != null)
				SelectedCanvas.ReleaseMouseCapture();
		}

		public IEnumerable<DesignerItem> Items
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<DesignerItem>() : SelectedCanvas.Items.OfType<DesignerItem>(); }
		}
		public IEnumerable<DesignerItem> SelectedItems
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<DesignerItem>() : from item in SelectedCanvas.Items.OfType<DesignerItem>() where item.IsSelected == true select item; }
		}
		public IEnumerable<ElementBase> SelectedElements
		{
			get { return SelectedCanvas == null ? Enumerable.Empty<ElementBase>() : from item in SelectedCanvas.Items.OfType<DesignerItem>() where item.IsSelected == true select item.Element; }
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