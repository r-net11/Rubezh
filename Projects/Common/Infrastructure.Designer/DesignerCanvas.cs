using Common;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.DragDrop;
using Infrastructure.Designer.DesignerItems;
using Infrastructure.Designer.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }
		public ToolboxViewModel Toolbox { get; private set; }
		private Point? _startPoint = null;
		private List<ElementBase> _initialElements;
		private MoveAdorner _moveAdorner;

		public DesignerCanvas(PlanDesignerViewModel planDesignerViewModel)
			: base(ServiceFactoryBase.Events)
		{
			GridLineController = new GridLineController(this);
			RemoveGridLinesCommand = new RelayCommand(OnRemoveGridLinesCommand);
			PlanDesignerViewModel = planDesignerViewModel;
			Toolbox = new ToolboxViewModel(this);
			ServiceFactoryBase.DragDropService.DragOver += OnDragServiceDragOver;
			ServiceFactoryBase.DragDropService.Drop += OnDragServiceDrop;
			ServiceFactoryBase.DragDropService.DragCorrection += OnDragServiceCorrection;
			PainterCache.Initialize(ServiceFactoryBase.ContentService.GetBitmapContent, ServiceFactoryBase.ContentService.GetDrawing, ServiceFactoryBase.ContentService.GetVisual);
			Width = 100;
			Height = 100;
			Focusable = false;
			DesignerSurface.AllowDrop = true;

			var menuItem = DesignerCanvasHelper.BuildMenuItem(
				"Вставить (Ctrl+V)",
				"BPaste",
				PlanDesignerViewModel.PasteCommand
			);
			menuItem.CommandParameter = this;
			var pasteItem = menuItem;

			ContextMenu = new ContextMenu();
			ContextMenu.HasDropShadow = false;
			ContextMenu.Items.Add(pasteItem);
			_moveAdorner = new MoveAdorner(this);
		}

		public override double Zoom
		{
			get { return PlanDesignerViewModel.Zoom; }
		}
		public override double PointZoom
		{
			get { return PlanDesignerViewModel.DeviceZoom / Zoom; }
		}

		public void RemoveAll()
		{
			RemoveDesignerItems(Items);
		}
		public void RemoveAllSelected()
		{
			RemoveDesignerItems(SelectedItems);
		}
		void RemoveDesignerItems(IEnumerable<DesignerItem> designerItems)
		{
			if (designerItems.Count() != 0)
			{
				var elements = designerItems.Select(item => item.Element).ToList();
				foreach (var designerItem in designerItems.ToList())
					RemoveDesignerItem(designerItem);
				ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Publish(elements);
				DesignerChanged();
			}
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			e.Effects = e.Data.GetDataPresent("DESIGNER_ITEM") ? DragDropEffects.Move : DragDropEffects.None;
			e.Handled = true;
			if (e.Effects == DragDropEffects.Move)
			{
				Toolbox.SetDefault();
				DeselectAll();
			}
		}
		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
			if (elementBase != null)
			{
				Toolbox.SetDefault();
				//elementBase.SetDefault();
				Point position = e.GetPosition(this);
				elementBase.Position = position;
				if (GridLineController != null)
					elementBase.Position += GridLineController.Pull(elementBase.GetRectangle());
				CreateDesignerItem(elementBase);
				e.Handled = true;
			}
		}
		private void OnDragServiceDragOver(object sender, DragServiceEventArgs e)
		{
			if (IsMouseInside())
			{
				e.Effects = e.Data.GetDataPresent("DESIGNER_ITEM") ? DragDropEffects.Move : DragDropEffects.None;
				if (e.Effects == DragDropEffects.Move)
				{
					Toolbox.SetDefault();
					DeselectAll();
					e.Handled = true;
				}
			}
		}
		private void OnDragServiceDrop(object sender, DragServiceEventArgs e)
		{
			if (IsMouseInside())
			{
				var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
				if (elementBase != null)
				{
					Toolbox.SetDefault();
					//elementBase.SetDefault();
					Point position = Mouse.GetPosition(this);
					elementBase.Position = position;
					if (GridLineController != null)
						elementBase.Position += GridLineController.Pull(elementBase.GetRectangle());
					CreateDesignerItem(elementBase);
					e.Handled = true;
				}
				_startPoint = null;
			}
		}
		private void OnDragServiceCorrection(object sender, DragCorrectionEventArgs e)
		{
			if (GridLineController != null)
			{
				var position = e.GetPosition(this);
				var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
				if (elementBase != null && IsMouseInside(position))
					e.Correction = GridLineController.Pull(position) * Zoom;
			}
		}
		private bool IsMouseInside(Point? point = null)
		{
			if (!point.HasValue)
				point = Mouse.GetPosition(this);
			return PlanDesignerViewModel.IsNotEmpty && point.Value.X > 0 && point.Value.Y > 0 && point.Value.X < ActualWidth && point.Value.Y < ActualHeight;
		}

		public override void BackgroundMouseDown(MouseButtonEventArgs e)
		{
			base.BackgroundMouseDown(e);
			if (Toolbox.ActiveInstrument != null & Toolbox.ActiveInstrument.Adorner != null && Toolbox.ActiveInstrument.Adorner.AllowBackgroundStart)
			{
				var ee = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				ee.RoutedEvent = MouseDownEvent;
				DesignerSurface.RaiseEvent(ee);
				e.Handled = true;
			}
		}
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Source == DesignerSurface && e.ChangedButton == MouseButton.Left && !ServiceFactoryBase.DragDropService.IsDragging)
			{
				_startPoint = new Point?(e.GetPosition(this));
				Toolbox.Apply(_startPoint);
				e.Handled = true;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
				_startPoint = null;
		}

		public DesignerItem CreateElement(ElementBase elementBase)
		{
			return AddElement(elementBase);
		}
		public override void CreateDesignerItem(ElementBase elementBase)
		{
			var designerItem = AddElement(elementBase);
			if (designerItem != null)
			{
				DeselectAll();
				designerItem.IsSelected = true;
				PlanDesignerViewModel.MoveToFrontCommand.Execute();
				ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
				//Toolbox.SetDefault();
				DesignerChanged();
			}
		}
		public void RemoveDesignerItems(List<ElementBase> elements)
		{
			elements.ForEach(x => RemoveDesignerItem(x));
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Publish(elements);
		}
		void RemoveDesignerItem(ElementBase elementBase)
		{
			var designerItem = GetDesignerItem(elementBase);
			if (designerItem != null)
				RemoveDesignerItem(designerItem);
		}
		public void RemoveDesignerItem(DesignerItem designerItem)
		{
			RemoveElement(designerItem.Element);
			Remove(designerItem);
			Refresh();
			designerItem.OnRemoved();
		}
		public DesignerItem UpdateElement(ElementBase elementBase)
		{
			var designerItem = GetDesignerItem(elementBase);
			if (designerItem != null)
			{
				var element = designerItem.Element;
				elementBase.Copy(element);
				designerItem.ResetElement(element);
			}
			else
				Logger.Error("DesignerCanvas Undo/Redo designerItem = null");
			return designerItem;
		}

		public DesignerItem Create(ElementBase elementBase)
		{
			var designerItem = DesignerItemFactory.Create(elementBase);
			PlanDesignerViewModel.RegisterDesignerItem(designerItem);
			Add(designerItem);
			Refresh();
			return designerItem;
		}

		public List<ElementBase> CloneElements(IEnumerable<DesignerItem> designerItems)
		{
			//Debug.WriteLine("CloneElements");
			_initialElements = new List<ElementBase>();
			foreach (var designerItem in designerItems)
			{
				designerItem.UpdateElementProperties();
				_initialElements.Add(designerItem.Element.Clone());
			}
			return _initialElements;
		}

		public override void Update()
		{
			GridLineController.Invalidate();
		}
		protected void Update(object element)
		{
			IElementRectangle elementRectangle = element as IElementRectangle;
			if (elementRectangle != null)
			{
				CanvasWidth = elementRectangle.Width;
				CanvasHeight = elementRectangle.Height;
			}
			IElementBackground elementBackground = element as IElementBackground;
			if (elementBackground != null)
				CanvasBackground = PainterCache.GetBrush(elementBackground);
			IElementBorder elementBorder = element as IElementBorder;
			if (elementBorder != null)
				CanvasBorder = PainterCache.GetPen(elementBorder.BorderColor, elementBorder.BorderThickness);
		}
		public override void BeginChange(IEnumerable<DesignerItem> designerItems)
		{
			//Debug.WriteLine("BeginChange");
			_initialElements = CloneElements(designerItems);
		}
		public override void BeginChange()
		{
			BeginChange(SelectedItems);
		}
		public override void EndChange()
		{
			//Debug.WriteLine("EndChange");
			var after = PlanDesignerViewModel.AddHistoryItem(_initialElements);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Publish(after);
			foreach (var designerItem in SelectedItems)
				designerItem.UpdateElementProperties();
		}

		public void UpdateZoom()
		{
			Margin = new Thickness(25 / Zoom);
			ZoomChanged();
			Toolbox.UpdateZoom();
			foreach (DesignerItem designerItem in Items)
			{
				designerItem.UpdateZoom();
				if (designerItem is DesignerItemPoint)
					designerItem.UpdateZoomPoint();
			}
		}
		public void UpdateZoomPoint()
		{
			ZoomChanged();
			foreach (DesignerItem designerItem in Items)
				if (designerItem is DesignerItemPoint)
					designerItem.UpdateZoomPoint();
		}

		public void BeginMove(Point point)
		{
			_moveAdorner.Show(point);
		}
		public void EndMove()
		{
			_moveAdorner.Hide();
		}

		public RelayCommand RemoveGridLinesCommand { get; private set; }
		private void OnRemoveGridLinesCommand()
		{
			GridLineController.GridLines.Clear();
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			Keyboard.Focus(this);
			base.OnPreviewMouseDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Vector? vector = null;
			switch (e.Key)
			{
				case Key.Up:
					vector = new Vector(0, -GetShift());
					break;
				case Key.Down:
					vector = new Vector(0, GetShift());
					break;
				case Key.Left:
					vector = new Vector(-GetShift(), 0);
					break;
				case Key.Right:
					vector = new Vector(GetShift(), 0);
					break;
			}
			if (vector.HasValue)
			{
				var designerItem = SelectedItems.FirstOrDefault();
				if (designerItem != null)
				{
					if (vector.HasValue)
					{
						var point = designerItem.Element.Position;
						if (!e.IsRepeat)
							designerItem.DragStarted(designerItem.Element.Position);
						designerItem.DragDelta(point, vector.Value);
						e.Handled = true;
					}
				}
			}
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
				case Key.Down:
				case Key.Left:
				case Key.Right:
					var designerItem = SelectedItems.FirstOrDefault();
					if (designerItem != null)
					{
						designerItem.DragCompleted(designerItem.Element.Position);
						e.Handled = true;
					}
					break;
			}
			base.OnKeyUp(e);
		}
		private double GetShift()
		{
			if (Keyboard.Modifiers == ModifierKeys.None)
				return 10;
			if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
				return Math.Max(CanvasHeight, CanvasWidth);
			else if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
				return 50;
			else
				return 0;
		}

		public override void RevertLastAction()
		{
			PlanDesignerViewModel.RevertLastAction();
		}

		protected virtual DesignerItem AddElement(ElementBase elementBase)
		{
			return Create(elementBase);
		}
		protected virtual void RemoveElement(ElementBase elementBase)
		{
		}
	}
}