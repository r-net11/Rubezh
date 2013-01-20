using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerCanvas : Canvas
	{
		private Dictionary<Guid, DesignerSurface> _canvasMap;
		protected DesignerSurface SelectedCanvas { get; private set; }
		protected Guid SelectedUID { get; private set; }
		public virtual double Zoom { get { return 1; } }
		public virtual double PointZoom { get { return CommonDesignerItem.DefaultPointSize; } }

		public CommonDesignerCanvas(IEventAggregator eventAggregator)
		{
			_canvasMap = new Dictionary<Guid, DesignerSurface>();
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
			_canvasMap.Clear();
			SelectedCanvas = null;
			SelectedUID = Guid.Empty;
		}
		protected void AddCanvas(Guid uid)
		{
			SelectedUID = uid;
			SelectedCanvas = new DesignerSurface();
			Children.Add(SelectedCanvas);
			_canvasMap.Add(uid, SelectedCanvas);

			CanvasBackground = new SolidColorBrush(Colors.DarkGray);
			CanvasWidth = 100;
			CanvasHeight = 100;
			SelectedCanvas.DataContext = this;
		}
		protected void RemoveCanvas()
		{
			_canvasMap.Remove(SelectedUID);
			Children.Remove(SelectedCanvas);
			SelectedCanvas = null;
			SelectedUID = Guid.Empty;
		}
		protected void SelectCanvas(Guid uid)
		{
			SelectedCanvas = null;
			SelectedUID = uid;
			if (uid != Guid.Empty && _canvasMap.ContainsKey(uid))
			{
				SelectedCanvas = _canvasMap[uid];
				SelectedCanvas.Update(true);
			}
		}
		public void ShowCanvas(Guid uid)
		{
			if (SelectedCanvas != null)
			{
				DeselectAll();
				SelectedCanvas.Visibility = System.Windows.Visibility.Collapsed;
				SelectCanvas(Guid.Empty);
			}
			if (uid != Guid.Empty)
			{
				SelectCanvas(uid);
				Height = SelectedCanvas.Height;
				Width = SelectedCanvas.Width;
				SelectedCanvas.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void Remove(CommonDesignerItem designerItem)
		{
			SelectedCanvas.DeleteDesignerItem(designerItem);
		}
		public void Add(CommonDesignerItem designerItem)
		{
			designerItem.DesignerCanvas = this;
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
					if (SelectedCanvas.IsMouseOver)
					{
						SelectedCanvas.ToolTip = toolTip;
						toolTip.IsOpen = true;
					}
				}
			}
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
		protected IEnumerable<T> InternalItems<T>()
		{
			return SelectedCanvas == null ? Enumerable.Empty<T>() : SelectedCanvas.Items.OfType<T>();
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

		public void ZoomChanged()
		{
			PainterCache.UpdateZoom(Zoom, PointZoom);
			ResizeChrome.UpdateZoom(Zoom);
		}
	}
}