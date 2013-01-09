using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using System.Windows.Media;
using System.Windows.Input;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : DrawingVisual
	{
		public const int BigConstatnt = 100000;

		public event EventHandler ItemPropertyChanged;
		public event EventHandler TitleChanged;

		public CommonDesignerCanvas DesignerCanvas { get; internal set; }
		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; protected set; }
		public double Shift { get; protected set; }
		public bool IsMouseOver { get; private set; }
		public bool IsBusy { get; protected set; }

		public event Action<CommonDesignerItem> UpdateProperties;

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				if (Title != value)
				{
					_title = value;
					if (TitleChanged != null)
						TitleChanged(this, EventArgs.Empty);
				}
			}
		}

		private bool _isVisibleLayout;
		public virtual bool IsVisibleLayout
		{
			get { return _isVisibleLayout; }
			set
			{
				if (_isVisibleLayout != value)
				{
					_isVisibleLayout = value;
					Redraw();
				}
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			IsBusy = false;
			Shift = double.NaN;
			ResetElement(element);
		}

		public virtual void UpdateZoom()
		{
		}
		public virtual void UpdateZoomPoint()
		{
			Redraw();
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
			Painter = PainterFactory.Create(Element);
		}
		public virtual void Redraw()
		{
			//MinWidth = Element.BorderThickness;
			//MinHeight = Element.BorderThickness;
			//if (Element is ElementBaseShape)
			//{
			//    MinWidth += 3;
			//    MinHeight += 3;
			//}
			using (DrawingContext drawingContext = RenderOpen())
				if (IsVisibleLayout)
					Render(drawingContext);
		}
		protected virtual void Render(DrawingContext drawingContext)
		{
			if (Painter != null)
				Painter.Draw(drawingContext, Element, GetVisualRect());
		}
		protected Rect GetVisualRect()
		{
			var rect = Element.GetRectangle();
			if (Shift == Shift)
				rect = new Rect(rect.X - Shift, rect.Y - Shift, 2 * Shift, 2 * Shift);
			return rect;
		}

		public virtual void UpdateElementProperties()
		{
			OnUpdateProperties();
		}
		protected void OnUpdateProperties()
		{
			if (UpdateProperties != null)
				UpdateProperties(this);
		}

		public virtual ContextMenu GetContextMenu()
		{
			return null;
		}

		protected void OnDesignerItemPropertyChanged()
		{
			if (ItemPropertyChanged != null)
				ItemPropertyChanged(this, EventArgs.Empty);
		}

		internal void OnMouseDown(MouseButtonEventArgs e)
		{
			MouseDown(e);
		}
		internal void OnMouseUp(MouseButtonEventArgs e)
		{
			MouseUp(e);
		}
		internal void OnMouseMove(MouseEventArgs e)
		{
			MouseMove(e);
		}
		internal void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			MouseDoubleClick(e);
		}
		protected virtual void MouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.SetTitle(null);
		}
		protected virtual void MouseUp(MouseButtonEventArgs e)
		{
		}
		protected virtual void MouseMove(MouseEventArgs e)
		{
		}
		protected virtual void MouseDoubleClick(MouseButtonEventArgs e)
		{
		}

		internal virtual void SetIsMouseOver(bool value)
		{
			IsMouseOver = value;
			DesignerCanvas.SetTitle(value ? Title : null);
		}
		internal virtual ContextMenu ContextMenuOpening()
		{
			DesignerCanvas.SetTitle(null);
			return GetContextMenu();
		}
	}
}