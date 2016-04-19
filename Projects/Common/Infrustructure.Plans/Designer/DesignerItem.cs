using RubezhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
			}
		}
		private static void IsSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == IsSelectableProperty)
			{
				if (!(bool)e.NewValue)
					d.SetValue(IsSelectedProperty, false);
				var designerItem = d as DesignerItem;
				designerItem.IsSelectableChanged();
			}
		}
		private static object IsSelectedCoerce(DependencyObject d, object e)
		{
			DesignerItem designerItem = d as DesignerItem;
			return designerItem != null && (!designerItem.IsSelectable || !designerItem.IsEnabled) ? false : e;
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

		public ResizeChrome ResizeChrome { get; private set; }
		public string Group { get; set; }
		public override bool AllowDrag { get { return true; } }
		protected bool IsMoved { get; private set; }

		public DesignerItem(ElementBase element)
			: base(element)
		{
			Group = string.Empty;
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			if (ResizeChrome != null)
				ResizeChrome.InvalidateVisual();
		}

		public override void ResetElement(ElementBase element)
		{
			var isNewElement = Element == null || Element.UID != element.UID;
			base.ResetElement(element);
			IsSelectable = !element.IsLocked;
			if (isNewElement || Painter == null)
				Painter = PainterFactory.Create(DesignerCanvas, Element);
			else
				Painter.ResetElement(Element);
			Painter.Invalidate();
			if (DesignerCanvas != null)
				DesignerCanvas.Refresh();
			if (ResizeChrome != null)
				ResizeChrome.ResetElement();
		}
		protected override void ResetIsEnabled()
		{
			base.ResetIsEnabled();
			IsEnabled &= IsSelectable;
			if (!IsEnabled)
				IsSelected = false;
		}
		internal override void Render(DrawingContext drawingContext)
		{
			base.Render(drawingContext);
			if (ResizeChrome != null)
			{
				if (IsSelected)
					ResizeChrome.Render(drawingContext);
				else
					ResizeChrome.Reset();
			}
		}
		public override void RefreshPainter()
		{
			base.RefreshPainter();
			if (ResizeChrome != null)
				ResizeChrome.InvalidateVisual();
		}
		protected void SetResizeChrome(ResizeChrome resizeChrome)
		{
			ResizeChrome = resizeChrome;
			if (ResizeChrome != null)
				ResizeChrome.IsVisible = IsSelected;
		}

		protected override void MouseDown(Point point, MouseButtonEventArgs e)
		{
			base.MouseDown(point, e);
			if (IsEnabled && DesignerCanvas != null)
			{
				if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
					IsSelected = !IsSelected;
				else if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
			}
		}
		protected override void MouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
			base.MouseDoubleClick(point, e);
			if (IsEnabled && DesignerCanvas != null)
				ShowPropertiesCommand.Execute(null);
		}

		protected override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			//if (_moveAdorner == null)
			if (!IsMoved)
				DesignerCanvas.Cursor = value && IsEnabled ? Cursors.SizeAll : Cursors.Arrow;
		}
		protected override ContextMenu ContextMenuOpening()
		{
			if (IsEnabled)
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
			var isLocked = Element.IsLocked;
			Element.IsLocked = !IsSelectable;
			ResetIsEnabled();
			if (DesignerCanvas != null)
				SetIsMouseOver(false);
			if (isLocked == IsSelectable)
				OnChanged();
		}
		protected void IsSelectedChanged()
		{
			if (ResizeChrome != null)
				ResizeChrome.IsVisible = IsSelected;
		}

		public override void DragStarted(Point point)
		{
			IsBusy = true;
			if (DesignerCanvas.GridLineController != null)
				DesignerCanvas.GridLineController.PullReset();
		}
		public override void DragCompleted(Point point)
		{
			IsBusy = false;
			if (IsMoved)
				DesignerCanvas.EndChange();
			IsMoved = false;
		}
		public override void DragDelta(Point point, Vector shift)
		{
			if (IsSelected)
			{
				if (!IsMoved)
					DesignerCanvas.BeginChange();
				IsMoved = true;
				if (DesignerCanvas.GridLineController != null)
					shift = DesignerCanvas.GridLineController.Pull(shift, Element.GetRectangle());
				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					//var rect = designerItem.ContentBounds;
					//var rect = designerItem.GetRectangle();
					var rect = designerItem.Element.GetRectangle();
					if (rect.Right + shift.X > DesignerCanvas.CanvasWidth)
						shift.X = DesignerCanvas.CanvasWidth - rect.Right;
					if (rect.Left + shift.X < 0)
						shift.X = -rect.Left;
					if (rect.Bottom + shift.Y > DesignerCanvas.CanvasHeight)
						shift.Y = DesignerCanvas.CanvasHeight - rect.Bottom;
					if (rect.Top + shift.Y < 0)
						shift.Y = -rect.Top;
				}
				if (shift.X != 0 || shift.Y != 0)
					foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
					{
						designerItem.Element.SetPosition(designerItem.Element.GetPosition() + shift);
						designerItem.RefreshPainter();
					}
			}
		}

		public override IVisualItem HitTest(Point point)
		{
			IVisualItem visualItem = null;
			if (IsSelected)
				visualItem = ResizeChrome.HitTest(point);
			if (visualItem == null)
				visualItem = base.HitTest(point);
			return visualItem;
		}

		public event Action Removed;
		public void OnRemoved()
		{
			if (Removed != null)
				Removed();
		}
	}
}