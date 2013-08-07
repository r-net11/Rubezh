using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services.DragDrop;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.Designer.DesignerItems;
using PlansModule.ViewModels;
using System.Diagnostics;

namespace PlansModule.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public Plan Plan { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
		public ToolboxViewModel Toolbox { get; set; }
		private Point? _startPoint = null;
		private List<ElementBase> _initialElements;
		private MoveAdorner _moveAdorner;

		public DesignerCanvas()
			: base(ServiceFactory.Events)
		{
			ServiceFactory.DragDropService.DragOver += OnDragServiceDragOver;
			ServiceFactory.DragDropService.Drop += OnDragServiceDrop;
			PainterCache.Initialize(ServiceFactory.ContentService.GetBitmapContent, ServiceFactory.ContentService.GetDrawing);
			Width = 100;
			Height = 100;
			Focusable = false;

			DesignerSurface.AllowDrop = true;
			var pasteItem = new MenuItem()
			{
				Header = "Вставить (Ctrl+V)",
				CommandParameter = this
			};
			pasteItem.SetBinding(MenuItem.CommandProperty, new Binding("Toolbox.PlansViewModel.PasteCommand"));
			var editItem = new MenuItem()
			{
				Header = "Редактировать",
			};
			editItem.SetBinding(MenuItem.CommandProperty, new Binding("Toolbox.PlansViewModel.EditCommand"));
			ContextMenu = new ContextMenu();
			ContextMenu.Items.Add(pasteItem);
			ContextMenu.Items.Add(editItem);
			_moveAdorner = new MoveAdorner(this);
			GridLineController = new GridLineController(this);
			RemoveGridLinesCommand = new RelayCommand(OnRemoveGridLinesCommand);
		}

		public override double Zoom
		{
			get { return PlanDesignerViewModel.Zoom; }
		}
		public override double PointZoom
		{
			get { return PlanDesignerViewModel.DeviceZoom / Zoom; }
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			Initialize();
		}

		public void RemoveAllSelected()
		{
			var elements = SelectedElements;
			if (elements.Count() == 0)
				return;

			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(elements.ToList());
			foreach (var designerItem in SelectedItems.ToList())
				RemoveElement(designerItem);
			ServiceFactory.SaveService.PlansChanged = true;
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
				CreateDesignerItem(elementBase);
				e.Handled = true;
			}
		}
		private void OnDragServiceDragOver(object sender, DragServiceEventArgs e)
		{
			if (IsMouseInside)
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
			if (IsMouseInside)
			{
				var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
				if (elementBase != null)
				{
					Toolbox.SetDefault();
					//elementBase.SetDefault();
					Point position = Mouse.GetPosition(this);
					elementBase.Position = position;
					CreateDesignerItem(elementBase);
					e.Handled = true;
				}
				_startPoint = null;
			}
		}
		private bool IsMouseInside
		{
			get
			{
				Point point = Mouse.GetPosition(this);
				return point.X > 0 && point.Y > 0 && point.X < ActualWidth && point.Y < ActualHeight;
			}
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
			if (e.Source == DesignerSurface && e.ChangedButton == MouseButton.Left && !ServiceFactory.DragDropService.IsDragging)
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

		public override void CreateDesignerItem(ElementBase elementBase)
		{
			var designerItem = AddElement(elementBase);
			if (designerItem != null)
			{
				DeselectAll();
				designerItem.IsSelected = true;
				PlanDesignerViewModel.MoveToFrontCommand.Execute();
				ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
				//Toolbox.SetDefault();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public DesignerItem AddElement(ElementBase elementBase)
		{
			if (elementBase is ElementRectangle)
				Plan.ElementRectangles.Add(elementBase as ElementRectangle);
			else if (elementBase is ElementEllipse)
				Plan.ElementEllipses.Add(elementBase as ElementEllipse);
			else if (elementBase is ElementPolygon)
				Plan.ElementPolygons.Add(elementBase as ElementPolygon);
			else if (elementBase is ElementPolyline)
				Plan.ElementPolylines.Add(elementBase as ElementPolyline);
			else if (elementBase is ElementTextBlock)
				Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
			else
				Toolbox.PlansViewModel.ElementAdded(elementBase);

			return Create(elementBase);
		}
		public void RemoveElement(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangle)
				Plan.ElementRectangles.Remove(designerItem.Element as ElementRectangle);
			else if (designerItem.Element is ElementEllipse)
				Plan.ElementEllipses.Remove(designerItem.Element as ElementEllipse);
			else if (designerItem.Element is ElementPolygon)
				Plan.ElementPolygons.Remove(designerItem.Element as ElementPolygon);
			else if (designerItem.Element is ElementPolyline)
				Plan.ElementPolylines.Remove(designerItem.Element as ElementPolyline);
			else if (designerItem.Element is ElementTextBlock)
				Plan.ElementTextBlocks.Remove(designerItem.Element as ElementTextBlock);
			else if (designerItem.Element is ElementSubPlan)
				Plan.ElementSubPlans.Remove(designerItem.Element as ElementSubPlan);
			else
				Toolbox.PlansViewModel.ElementRemoved(designerItem.Element);
			Remove(designerItem);
			Refresh();
		}
		public void RemoveElement(ElementBase elementBase)
		{
			var designerItem = GetDesignerItem(elementBase);
			if (designerItem != null)
				RemoveElement(designerItem);
		}
		public void UpdateElement(ElementBase elementBase)
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
		}

		public DesignerItem Create(ElementBase elementBase)
		{
			var designerItem = DesignerItemFactory.Create(elementBase);
			Toolbox.PlansViewModel.RegisterDesignerItem(designerItem);
			Add(designerItem);
			Refresh();
			return designerItem;
		}

		public override void Update()
		{
			if (Plan != null)
				Update(Plan);
		}
		private void Update(Plan plan)
		{
			CanvasWidth = plan.Width;
			CanvasHeight = plan.Height;
			CanvasBackground = PainterCache.GetBrush(plan);
			GridLineController.Invalidate();
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
			var after = Toolbox.PlansViewModel.AddHistoryItem(_initialElements);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(after);
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
			Toolbox.PlansViewModel.RevertLastAction();
		}
	}
}