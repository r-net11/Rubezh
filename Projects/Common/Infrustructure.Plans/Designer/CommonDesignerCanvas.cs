using Common;
using Infrustructure.Plans.Painters;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerCanvas : Decorator
	{
		private Dictionary<Guid, CommonDesignerItem> _map;
		protected DesignerSurface DesignerSurface { get; private set; }
		protected Guid SelectedUID { get; private set; }
		public PainterCache PainterCache { get; private set; }
		public IGridLineController GridLineController { get; protected set; }
		public virtual double Zoom { get { return 1; } }
		public virtual double PointZoom { get { return CommonDesignerItem.DefaultPointSize; } }
		public bool IsLocked { get; set; }

		public CommonDesignerCanvas(IEventAggregator eventAggregator)
		{
			PainterCache = new PainterCache();
			IsLocked = false;
			_map = new Dictionary<Guid, CommonDesignerItem>();
			DataContext = this;
			EventService.RegisterEventAggregator(eventAggregator);
			ClipToBounds = false;
			GridLineController = null;

			DesignerSurface = new DesignerSurface(this);
			Child = DesignerSurface;
			Width = 100;
			Height = 100;
		}

		public abstract void BeginChange(IEnumerable<DesignerItem> designerItems);
		public abstract void BeginChange();
		public abstract void EndChange();

		protected void Initialize()
		{
			_map.Clear();
			DesignerSurface.ClearDesignerItems();
			using (new TimeCounter("\t\t\tDesignerCanvas.Background: {0}"))
				Update();
		}
		public void LoadingFinished()
		{
			DesignerSurface.UpdateZIndex();
			DesignerSurface.Update(true);
		}
		public void Refresh()
		{
			if (DesignerSurface != null)
				DesignerSurface.InvalidateVisual();
		}

		protected void Remove(CommonDesignerItem designerItem)
		{
			_map.Remove(designerItem.Element.UID);
			DesignerSurface.DeleteDesignerItem(designerItem);
		}
		protected void Add(CommonDesignerItem designerItem)
		{
			_map.Add(designerItem.Element.UID, designerItem);
			designerItem.Bind(this);
			DesignerSurface.AddDesignerItem(designerItem);
		}
		public double CanvasWidth
		{
			get { return DesignerSurface.Width; }
			set
			{
				Width = value;
				DesignerSurface.Width = value;
			}
		}
		public double CanvasHeight
		{
			get { return DesignerSurface.Height; }
			set
			{
				Height = value;
				DesignerSurface.Height = value;
			}
		}
		public Brush CanvasBackground
		{
			get { return DesignerSurface.BackgroundBrush; }
			set { DesignerSurface.BackgroundBrush = value; }
		}
		public Pen CanvasBorder
		{
			get { return DesignerSurface.Border; }
			set { DesignerSurface.Border = value; }
		}
		public void UpdateZIndex()
		{
			if (DesignerSurface != null)
				DesignerSurface.UpdateZIndex();
		}
		public void SetTitle(object title)
		{
			if (DesignerSurface != null)
			{
				var toolTip = DesignerSurface.ToolTip as ToolTip;
				if (toolTip != null)
					toolTip.IsOpen = false;
				DesignerSurface.ToolTip = null;
				if (title != null)
				{
					toolTip = new ToolTip();
					toolTip.Content = title;
					toolTip.Placement = PlacementMode.MousePoint;
					if (DesignerSurface.IsMouseOver)
					{
						DesignerSurface.ToolTip = toolTip;
						toolTip.IsOpen = true;
					}
				}
			}
		}

		public IEnumerable<DesignerItem> Items
		{
			get { return DesignerSurface == null ? Enumerable.Empty<DesignerItem>() : DesignerSurface.Items.OfType<DesignerItem>(); }
		}
		public IEnumerable<DesignerItem> SelectedItems
		{
			get { return DesignerSurface == null ? Enumerable.Empty<DesignerItem>() : from item in DesignerSurface.Items.OfType<DesignerItem>() where item.IsSelected == true select item; }
		}
		public IEnumerable<ElementBase> SelectedElements
		{
			get { return DesignerSurface == null ? Enumerable.Empty<ElementBase>() : from item in DesignerSurface.Items.OfType<DesignerItem>() where item.IsSelected == true select item.Element; }
		}
		public DesignerItem GetDesignerItem(ElementBase elementBase)
		{
			return GetDesignerItem(elementBase.UID);
		}
		public DesignerItem GetDesignerItem(Guid elementUID)
		{
			return _map.ContainsKey(elementUID) ? _map[elementUID] as DesignerItem : null;
		}
		public bool IsPresented(DesignerItem designerItem)
		{
			return designerItem != null && designerItem.Element != null && _map.ContainsKey(designerItem.Element.UID);
		}
		protected IEnumerable<T> InternalItems<T>()
		{
			return DesignerSurface == null ? Enumerable.Empty<T>() : DesignerSurface.Items.OfType<T>();
		}

		public void SelectAll()
		{
			if (DesignerSurface != null)
				foreach (var designerItem in Items)
					designerItem.IsSelected = true;
		}
		public void DeselectAll()
		{
			if (DesignerSurface != null)
				foreach (DesignerItem item in this.SelectedItems)
					item.IsSelected = false;
		}

		public abstract void Update();
		public abstract void CreateDesignerItem(ElementBase element);

		public void ZoomChanged()
		{
			PainterCache.UpdateZoom(Zoom, PointZoom);
			ResizeChrome.UpdateZoom(Zoom);
		}

		protected internal virtual void RenderBackground(DrawingContext drawingContext)
		{
		}
		protected internal virtual void RenderForeground(DrawingContext drawingContext)
		{
			if (GridLineController != null)
				GridLineController.Render(drawingContext);
		}
		protected internal virtual void SetDesignerItemOver(CommonDesignerItem designerItem, bool isOver)
		{
		}

		public virtual void BackgroundMouseDown(MouseButtonEventArgs e)
		{
		}

		public virtual void RevertLastAction()
		{
		}
		public virtual void DesignerChanged()
		{
		}
	}
}