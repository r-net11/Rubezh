using System.Linq;
using System.Windows;
using System.Windows.Input;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using System.Windows.Media;
using System.Windows.Controls;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : CommonDesignerItem
	{
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false, IsSelectedChanged, IsSelectedCoerce));
		public virtual bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(true, IsSelectableChanged, IsSelectableCoerce));
		public virtual bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var designerItem = d as DesignerItem;
			if (e.Property == IsSelectedProperty)
			{
				if ((bool)e.NewValue && designerItem != null && designerItem.DesignerCanvas != null && designerItem.DesignerCanvas.SelectedItems.Count() == 1)
					EventService.EventAggregator.GetEvent<ElementSelectedEvent>().Publish(((CommonDesignerItem)d).Element);
				designerItem.IsSelectedChanged();
				designerItem.Redraw();
			}
		}
		private static void IsSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == IsSelectableProperty)
			{
				if (!(bool)e.NewValue)
					d.SetValue(IsSelectedProperty, false);
				((DesignerItem)d).IsSelectableChanged();
			}
		}
		private static object IsSelectedCoerce(DependencyObject d, object e)
		{
			DesignerItem designerItem = d as DesignerItem;
			return designerItem != null && !designerItem.IsSelectable ? false : e;
		}
		private static object IsSelectableCoerce(DependencyObject d, object e)
		{
			return e;
		}

		public override bool IsVisibleLayout
		{
			get { return base.IsVisibleLayout; }
			set
			{
				if (IsVisibleLayout != value && !value)
					IsSelected = false;
				base.IsVisibleLayout = value;
			}
		}
		public ICommand ShowPropertiesCommand { get; protected set; }
		public ICommand DeleteCommand { get; protected set; }

		public ResizeChrome ResizeChrome { get; protected set; }
		public string Group { get; set; }
		protected bool IsDragging { get; private set; }
		protected bool IsMoved { get; private set; }
		private Point _previousPosition;

		public DesignerItem(ElementBase element)
			: base(element)
		{
			Group = string.Empty;
			IsVisibleLayout = true;
		}

		public override void ResetElement(ElementBase element)
		{
			base.ResetElement(element);
			if (DesignerCanvas != null)
				Redraw();
		}
		public void UpdateAdorner()
		{
			//if (ResizeChrome != null)
			//    ResizeChrome.Initialize();
		}
		public virtual void UpdateAdornerLayout()
		{
		}

		public override void UpdateZoom()
		{
			//if (ResizeChrome != null)
			//    ResizeChrome.UpdateZoom();
		}
		public override void Redraw()
		{
			UpdateAdorner();
			base.Redraw();
		}
		protected override void Render(DrawingContext drawingContext)
		{
			base.Render(drawingContext);
			if (IsSelected)
				drawingContext.DrawRectangle(null, new Pen(Brushes.Green, 2), Element.GetRectangle());
		}

		protected override void MouseDown(MouseButtonEventArgs e)
		{
			base.MouseDown(e);
			if (IsSelectable && DesignerCanvas != null)
			{
				if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
					IsSelected = !IsSelected;
				else if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
				if (!IsDragging)
				{
					e.Handled = true;
					_previousPosition = e.GetPosition(DesignerCanvas);
					DragStarted();
				}
			}
		}
		protected override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);
			if (IsSelectable && DesignerCanvas != null && IsDragging)
			{
				if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
				{
					Point position = e.GetPosition(DesignerCanvas);
					if (_previousPosition != position)
					{
						DragDelta(new Vector(position.X - _previousPosition.X, position.Y - _previousPosition.Y));
						_previousPosition = position;
						e.Handled = true;
					}
				}
				else
					DragCompleted();
			}
		}
		protected override void MouseUp(MouseButtonEventArgs e)
		{
			base.MouseUp(e);
			if (IsSelectable && DesignerCanvas != null && IsDragging)
			{
				e.Handled = true;
				DragCompleted();
			}
		}
		protected override void MouseDoubleClick(MouseButtonEventArgs e)
		{
			base.MouseDoubleClick(e);
			if (IsSelected && DesignerCanvas != null)
				ShowPropertiesCommand.Execute(null);
		}
		internal override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			DesignerCanvas.Cursor = value && IsSelectable ? Cursors.SizeAll : Cursors.Arrow;
		}
		internal override ContextMenu ContextMenuOpening()
		{
			if (IsSelectable)
			{
				if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
				return base.ContextMenuOpening();
			}
			else
				return null;
		}

		protected abstract void OnShowProperties();
		protected abstract void OnDelete();

		protected void IsSelectableChanged()
		{
			SetIsMouseOver(false);
		}
		protected void IsSelectedChanged()
		{
			if (ResizeChrome != null)
				ResizeChrome.Visibility = IsSelected ? Visibility.Visible : Visibility.Hidden;
		}

		protected virtual void DragStarted()
		{
			IsBusy = true;
			IsDragging = true;
			IsMoved = false;
			DesignerCanvas.BeginChange();
			DesignerCanvas.SurfaceCaptureMouse();
		}
		protected virtual void DragCompleted()
		{
			IsBusy = false;
			IsDragging = false;
			if (DesignerCanvas.IsSurfaceMouseCaptured)
				DesignerCanvas.SurfaceReleaseMouseCapture();
			if (IsMoved)
				DesignerCanvas.EndChange();
		}
		protected virtual void DragDelta(Vector shift)
		{
			if (IsSelected)
			{
				IsMoved = true;
				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					var rect = designerItem.GetVisualRect();
					if (rect.Right + shift.X > DesignerCanvas.CanvasWidth)
						shift.X = DesignerCanvas.CanvasWidth - rect.Right;
					if (rect.Left + shift.X < 0)
						shift.X = -rect.Left;
					if (rect.Bottom + shift.Y > DesignerCanvas.CanvasHeight)
						shift.Y = DesignerCanvas.CanvasHeight - rect.Bottom;
					if (rect.Top + shift.Y < 0)
						shift.Y = -rect.Top;
				}
				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					designerItem.Element.Position += shift;
					designerItem.Redraw();
				}
			}
		}
	}
}