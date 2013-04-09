using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.Designer.DesignerItems;
using PlansModule.ViewModels;
using System.Windows.Media;
using Common;
using System.Windows.Shapes;
using Infrastructure.Common;

namespace PlansModule.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public Plan Plan { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
		public GridLinePresenter GridLinePresenter { get; private set; }
		public ToolboxViewModel Toolbox { get; set; }
		private Point? _startPoint = null;
		private List<ElementBase> _initialElements;
		private MoveAdorner _moveAdorner;

		public DesignerCanvas()
			: base(ServiceFactory.Events)
		{
			PainterCache.Initialize(ServiceFactory.ContentService.GetBitmapContent, ServiceFactory.ContentService.GetDrawing);
			Width = 100;
			Height = 100;

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
			GridLinePresenter = new GridLinePresenter(this);
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
			if (e.Source == DesignerSurface && e.ChangedButton == MouseButton.Left)
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

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			e.Effects = e.Data.GetDataPresent("DESIGNER_ITEM") ? DragDropEffects.Move : DragDropEffects.None;
			e.Handled = true;
			if (e.Effects == DragDropEffects.Move)
				Toolbox.SetDefault();
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
		//protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		//{
		//    base.OnGiveFeedback(e);
		//    e.UseDefaultCursors = false;
		//    e.Handled = true;
		//}
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
			GridLinePresenter.Invalidate();
		}

		public List<ElementBase> CloneElements(IEnumerable<DesignerItem> designerItems)
		{
			System.Console.WriteLine("CloneElements");
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
			System.Console.WriteLine("BeginChange");
			_initialElements = CloneElements(designerItems);
		}
		public override void BeginChange()
		{
			BeginChange(SelectedItems);
		}
		public override void EndChange()
		{
			System.Console.WriteLine("EndChange");
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(_initialElements);
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

		protected override void RenderForeground(DrawingContext drawingContext)
		{
			GridLinePresenter.Render(drawingContext);
		}

		public RelayCommand RemoveGridLinesCommand { get; private set; }
		private void OnRemoveGridLinesCommand()
		{
			GridLinePresenter.GridLines.Clear();
		}
	}
}