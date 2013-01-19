using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : DependencyObject, IVisualItem
	{
		public const int DefaultPointSize = 30;

		public event EventHandler ItemPropertyChanged;
		public event EventHandler TitleChanged;

		public CommonDesignerCanvas DesignerCanvas { get; internal set; }
		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; protected set; }
		public bool IsMouseOver { get; private set; }
		public bool IsBusy { get; protected set; }
		public bool IsEnabled { get; protected set; }
		public virtual bool AllowDrag { get { return false; } }

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
					if (Painter != null)
						Painter.IsVisible = IsVisibleLayout;
					ResetIsEnabled();
				}
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			IsBusy = false;
			ResetIsEnabled();
			Element = element;
		}

		public double MinHeight { get; protected set; }
		public double MinWidth { get; protected set; }
		public Rect ContentBounds
		{
			get { return Painter == null ? Rect.Empty : Painter.Bounds; }
		}

		public virtual void UpdateZoom()
		{
		}
		public virtual void UpdateZoomPoint()
		{
		}

		public void ResetElement(ElementBase element)
		{
			Element = element;
			ResetElement();
		}
		public void ResetElement()
		{
			Painter = PainterFactory.Create(Element);
			if (DesignerCanvas != null && Painter != null)
			{
				Painter.IsVisible = IsVisibleLayout;
				SetMinSize();
			}
		}
		public virtual void Redraw()
		{
			SetMinSize();
			Painter.Redraw(Element, GetRectangle());
			//Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)delegate(){});
		}
		public virtual void Render(DrawingContext context)
		{
			Painter.Draw(context, Element, GetRectangle());
		}
		public virtual Rect GetRectangle()
		{
			return Element.GetRectangle();
		}
		protected virtual void ResetIsEnabled()
		{
			IsEnabled = IsVisibleLayout;
		}
		public virtual void Transform()
		{
			Painter.Transform(Element, GetRectangle());
		}

		protected virtual void SetMinSize()
		{
			MinWidth = Element.BorderThickness;
			MinHeight = Element.BorderThickness;
			if (Element is ElementBaseShape)
			{
				MinWidth += 3;
				MinHeight += 3;
			}
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

		protected virtual void MouseDown(Point point, MouseButtonEventArgs e)
		{
			DesignerCanvas.SetTitle(null);
		}
		protected virtual void MouseUp(Point point, MouseButtonEventArgs e)
		{
		}
		protected virtual void MouseMove(Point point, MouseEventArgs e)
		{
		}
		protected virtual void MouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
		}

		protected virtual void SetIsMouseOver(bool value)
		{
			IsMouseOver = value;
			DesignerCanvas.SetTitle(value ? Title : null);
		}
		protected virtual ContextMenu ContextMenuOpening()
		{
			DesignerCanvas.SetTitle(null);
			return GetContextMenu();
		}

		public virtual bool HitTest(Point point)
		{
			return Painter == null || !IsEnabled ? false : Painter.HitTest(point);
		}

		#region IVisualItem Members

		void IVisualItem.SetIsMouseOver(bool isMouseOver, Point point)
		{
			SetIsMouseOver(isMouseOver);
		}
		ContextMenu IVisualItem.ContextMenuOpening()
		{
			return ContextMenuOpening();
		}

		void IVisualItem.OnMouseDown(Point point, MouseButtonEventArgs e)
		{
			if (IsEnabled)
				MouseDown(point, e);
		}
		void IVisualItem.OnMouseUp(Point point, MouseButtonEventArgs e)
		{
			if (IsEnabled)
				MouseUp(point, e);
		}
		void IVisualItem.OnMouseMove(Point point, MouseEventArgs e)
		{
			if (IsEnabled)
				MouseMove(point, e);
		}
		void IVisualItem.OnMouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
			if (IsEnabled)
				MouseDoubleClick(point, e);
		}

		#endregion

		public virtual void DragStarted(Point point)
		{
		}
		public virtual void DragCompleted(Point point)
		{
		}
		public virtual void DragDelta(Point point, Vector shift)
		{
		}
	}
}