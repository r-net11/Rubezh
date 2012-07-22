using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.Designer.Designer;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
	public class DesignerCanvas : CommonDesignerCanvas
	{
		public Plan Plan { get; set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
		public ToolboxViewModel Toolbox { get; set; }
		private Point? _startPoint = null;
		private List<ElementBase> _initialElements;

		public DesignerCanvas()
			: base(ServiceFactory.Events)
		{
			AllowDrop = true;
			Background = new SolidColorBrush(Colors.DarkGray);
			Width = 100;
			Height = 100;

			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			DataContext = this;
		}

		public override double Zoom
		{
			get { return PlanDesignerViewModel.Zoom; }
		}
		public override double PointZoom
		{
			get { return PlanDesignerViewModel.DeviceZoom / Zoom; }
		}

		public void RemoveAllSelected()
		{
			if (SelectedElements.Count == 0)
				return;

			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(new List<ElementBase>(SelectedElements));
			for (int i = Items.Count() - 1; i >= 0; i--)
			{
				var designerItem = Children[i] as DesignerItem;
				if (designerItem.IsSelected)
					RemoveElement(designerItem);
			}
			ServiceFactory.SaveService.PlansChanged = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Source == this)
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

		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;
			//elementBase.SetDefault();

			Point position = e.GetPosition(this);
			elementBase.Position = position;
			CreateDesignerItem(elementBase);
			e.Handled = true;
		}
		public void CreateDesignerItem(ElementBase elementBase)
		{
			var designerItem = AddElement(elementBase);
			if (designerItem != null)
			{
				DeselectAll();
				designerItem.IsSelected = true;
				PlanDesignerViewModel.MoveToFrontCommand.Execute();
				ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
				Toolbox.SetDefault();
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
			else if (elementBase is ElementRectangleZone)
				Plan.ElementRectangleZones.Add(elementBase as ElementRectangleZone);
			else if (elementBase is ElementPolygonZone)
				Plan.ElementPolygonZones.Add(elementBase as ElementPolygonZone);
			else if (elementBase is ElementSubPlan)
				Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
			else if (elementBase is ElementDevice)
			{
				var elementDevice = elementBase as ElementDevice;
				Helper.SetDevice(elementDevice);
				Plan.ElementDevices.Add(elementDevice);
			}

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
			else if (designerItem.Element is ElementRectangleZone)
				Plan.ElementRectangleZones.Remove(designerItem.Element as ElementRectangleZone);
			else if (designerItem.Element is ElementPolygonZone)
				Plan.ElementPolygonZones.Remove(designerItem.Element as ElementPolygonZone);
			else if (designerItem.Element is ElementSubPlan)
				Plan.ElementSubPlans.Remove(designerItem.Element as ElementSubPlan);
			else if (designerItem.Element is ElementDevice)
				Plan.ElementDevices.Remove(designerItem.Element as ElementDevice);
			Children.Remove(designerItem);
		}
		public void RemoveElement(ElementBase elementBase)
		{
			var designerItem = Items.FirstOrDefault(x => x.Element.UID == elementBase.UID);
			if (designerItem != null)
				RemoveElement(designerItem);
		}

		public DesignerItem Create(ElementBase elementBase)
		{
			var designerItem = DesignerItemFactory.Create(elementBase);
			Children.Add(designerItem);
			designerItem.Redraw();
			SetZIndex(designerItem);
			return designerItem;
		}
		private void SetZIndex(DesignerItem designerItem)
		{
			int bigConstatnt = 100000;

			if (designerItem.Element is IElementZIndex)
				Panel.SetZIndex(designerItem, (designerItem.Element as IElementZIndex).ZIndex);

			if (designerItem.Element is ElementSubPlan)
				Panel.SetZIndex(designerItem, 1 * bigConstatnt);

			if (designerItem.Element is IElementZone)
			{
				Panel.SetZIndex(designerItem, 2 * bigConstatnt);
				IElementZone elementZone = designerItem.Element as IElementZone;
				Zone zone = Helper.GetZone(elementZone);
				if (zone != null)
				{
					if (zone.ZoneType == ZoneType.Fire)
						Panel.SetZIndex(designerItem, 3 * bigConstatnt);

					if (zone.ZoneType == ZoneType.Guard)
						Panel.SetZIndex(designerItem, 4 * bigConstatnt);
				}
			}

			if (designerItem.Element is ElementDevice)
				Panel.SetZIndex(designerItem, 5 * bigConstatnt);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var designerPropertiesViewModel = new DesignerPropertiesViewModel(Plan);
			if (DialogService.ShowModalWindow(designerPropertiesViewModel))
			{
				Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		public override void Update()
		{
			Width = Plan.Width;
			Height = Plan.Height;
			Background = new SolidColorBrush(Plan.BackgroundColor);
			if (Plan.BackgroundPixels != null)
				Background = PainterHelper.CreateBrush(Plan.BackgroundPixels);
		}

		public List<ElementBase> CloneElements(IEnumerable<DesignerItem> designerItems)
		{
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
			_initialElements = CloneElements(designerItems);
		}
		public override void BeginChange()
		{
			_initialElements = CloneElements(SelectedItems);
		}
		public override void EndChange()
		{
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(_initialElements);
			foreach (var designerItem in SelectedItems)
				designerItem.UpdateElementProperties();
		}

		public void UpdateZoom()
		{
			Toolbox.UpdateZoom();
			foreach (DesignerItem designerItem in Items)
				designerItem.UpdateZoom();
			UpdateZoomPoint();
		}
		public void UpdateZoomPoint()
		{
			foreach (DesignerItem designerItem in Items)
				designerItem.UpdateZoomPoint();
		}
	}
}