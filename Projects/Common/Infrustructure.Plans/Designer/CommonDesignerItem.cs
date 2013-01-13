using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Threading;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : DrawingVisual, IVisualItem
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
		protected Rect OriginalRect { get; private set; }
		protected TranslateTransform TranslateTransform { get; private set; }
		protected ScaleTransform ScaleTransform { get; private set; }

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
					Opacity = IsVisibleLayout ? 1 : 0;
					ResetIsEnabled();
				}
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			ScaleTransform = new ScaleTransform();
			TranslateTransform = new TranslateTransform();
			Transform = new TransformGroup()
			{
				Children = new TransformCollection()
				{
					ScaleTransform,
					TranslateTransform
				}
			};
			IsBusy = false;
			ResetIsEnabled();
			ResetElement(element);
		}

		public double MinHeight { get; protected set; }
		public double MinWidth { get; protected set; }

		public virtual void UpdateZoom()
		{
			if (Painter != null && Painter.RedrawOnZoom)
				Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)Redraw);
		}
		public virtual void UpdateZoomPoint()
		{
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
			Painter = PainterFactory.Create(Element);
		}
		public virtual void Redraw()
		{
			SetMinSize();
			//Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)delegate(){});
			using (DrawingContext drawingContext = RenderOpen())
			{
				OriginalRect = GetRectangle();
				Translate(true);
				Render(drawingContext);
			}
		}
		protected virtual void Render(DrawingContext drawingContext)
		{
			if (Painter != null)
				Painter.Draw(drawingContext, Element, GetRectangle());
		}
		public virtual void Translate(bool force = false)
		{
			// if (Painter.AllowScale)?...:Redraw();
			var rect = GetRectangle();
			if (rect.Size != OriginalRect.Size || force)
			{
				ScaleTransform.CenterX = OriginalRect.Left;
				ScaleTransform.CenterY = OriginalRect.Top;
				ScaleTransform.ScaleX = rect.Width / OriginalRect.Width;
				ScaleTransform.ScaleY = rect.Height / OriginalRect.Height;
			}
			if (rect.TopLeft != OriginalRect.TopLeft || force)
			{
				TranslateTransform.X = rect.Left - OriginalRect.Left;
				TranslateTransform.Y = rect.Top - OriginalRect.Top;
			}
		}
		public virtual Rect GetRectangle()
		{
			return Element.GetRectangle();
		}
		protected virtual void ResetIsEnabled()
		{
			IsEnabled = IsVisibleLayout;
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