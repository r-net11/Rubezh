using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RubezhAPI.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : DependencyObject, IVisualItem
	{
		public const int DefaultPointSize = 30;

		public event EventHandler ItemPropertyChanged;
		public event EventHandler IndexPropertyChanged;
		public event EventHandler TitleChanged;
		public event EventHandler IconSourceChanged;

		public CommonDesignerCanvas DesignerCanvas { get; private set; }
		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; protected set; }
		public bool IsMouseOver { get; private set; }
		public bool IsBusy { get; protected set; }
		public bool IsEnabled { get; protected set; }
		public virtual bool AllowDrag { get { return false; } }

		public event Action<CommonDesignerItem> UpdateProperties;


		public Control WPFControl { get; set; }


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

		private int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				if (_index == value) return;

				_index = value;
				if (IndexPropertyChanged != null)
					IndexPropertyChanged(this, EventArgs.Empty);
			}
		}

		public string ClassName { get; set; }

		private string _iconSource;
		public string IconSource
		{
			get { return _iconSource; }
			set
			{
				if (IconSource != value)
				{
					_iconSource = value;
					if (IconSourceChanged != null)
						IconSourceChanged(this, EventArgs.Empty);
				}
			}
		}

		public object ToolTip
		{
			get { return GetToolTip(); }
		}

		public virtual bool IsVisibleLayout
		{
			get { return !Element.IsHidden; }
			set
			{
				if (Element.IsHidden == value)
				{
					Element.IsHidden = !value;
					ResetIsEnabled();
					if (DesignerCanvas != null)
						DesignerCanvas.Refresh();
					OnChanged();
				}
			}
		}

		public Rect ContentBounds
		{
			get { return Painter.Bounds; }
		}

		public CommonDesignerItem(ElementBase element)
		{
			IsBusy = false;
			Element = element;
			ClassName = null;
		}

		internal void Bind(CommonDesignerCanvas designerCanvas)
		{
			DesignerCanvas = designerCanvas;
			ResetElement(Element);
			ResetIsEnabled();
		}

		public double MinHeight { get; protected set; }
		public double MinWidth { get; protected set; }

		public virtual void UpdateZoom()
		{
		}
		public virtual void UpdateZoomPoint()
		{
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
		}
		internal virtual void Render(DrawingContext drawingContext)
		{
			if (Painter != null)
				Painter.Draw(drawingContext);
		}
		public virtual void RefreshPainter()
		{
			SetMinSize();
			Painter.Transform();
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
		public virtual ContextMenu GetElementContextMenu()
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
			DesignerCanvas.SetTitle(value ? GetToolTip() : null);
			DesignerCanvas.SetDesignerItemOver(this, value);
		}
		protected virtual object GetToolTip()
		{
			string format =
				Index == default(int)
				? string.Format("{0}", Title)
				: string.Format("{0}. {1}", Index, Title);
			var tooltip = Painter == null ? null : Painter.GetToolTip(format);
			return tooltip;
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

		public virtual IVisualItem HitTest(Point point)
		{
			return IsEnabled && Painter.HitTest(point) ? this : null;
		}

		public virtual void DragStarted(Point point)
		{
		}
		public virtual void DragCompleted(Point point)
		{
		}
		public virtual void DragDelta(Point point, Vector shift)
		{
		}

		protected virtual void OnChanged()
		{
		}
	}
}