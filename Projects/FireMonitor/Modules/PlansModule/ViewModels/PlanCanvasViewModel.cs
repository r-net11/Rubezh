using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using PlansModule.Events;
using PlansModule.Views;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class PlanCanvasViewModel : ViewPartViewModel
	{
		public Plan Plan { get; private set; }
		public Canvas Canvas { get; private set; }

		public List<ElementSubPlanViewModel> SubPlans { get; set; }
		public List<ElementZoneViewModel> Zones { get; set; }
		public List<ElementDeviceViewModel> Devices { get; set; }
		public List<ElementXDeviceViewModel> XDevices { get; set; }

		public PlanCanvasViewModel(Plan plan)
		{
			Plan = plan;
			DrawPlan();

			ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
			ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Subscribe(OnElementDeviceSelected);
			ServiceFactory.Events.GetEvent<ElementZoneSelectedEvent>().Subscribe(OnElementZoneSelected);
		}

		public void Update()
		{
			UpdateSubPlans();
			ResetView();
		}

		public void DrawPlan()
		{
			SubPlans = new List<ElementSubPlanViewModel>();
			Zones = new List<ElementZoneViewModel>();
			Devices = new List<ElementDeviceViewModel>();
			XDevices = new List<ElementXDeviceViewModel>();

			Canvas = new Canvas()
			{
				Width = Plan.Width,
				Height = Plan.Height
			};
			Canvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_canvas_PreviewMouseLeftButtonDown);

			if (Plan.BackgroundPixels != null)
				Canvas.Background = PainterHelper.CreateBrush(Plan.BackgroundPixels); //TODO: ~20-25 % общего времени
			else
				Canvas.Background = new SolidColorBrush(Plan.BackgroundColor);

			foreach (var elementRectangle in Plan.ElementRectangles)
				DrawElement(elementRectangle);
			foreach (var elementEllipse in Plan.ElementEllipses)
				DrawElement(elementEllipse);
			foreach (var elementTextBlock in Plan.ElementTextBlocks)
				DrawElement(elementTextBlock);
			foreach (var elementPolygon in Plan.ElementPolygons)
				DrawElement(elementPolygon);
			foreach (var elementPolyline in Plan.ElementPolylines)
				DrawElement(elementPolyline);
			foreach (var elementSubPlan in Plan.ElementSubPlans)
			{
				var subPlanViewModel = new ElementSubPlanViewModel(elementSubPlan);
				DrawElement(subPlanViewModel.ElementSubPlanView, elementSubPlan, subPlanViewModel);
				SubPlans.Add(subPlanViewModel);
			}
			foreach (var elementRectangleZone in Plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty))
			{
				var elementZoneViewModel = new ElementZoneViewModel(RectangleZoneToPolygon(elementRectangleZone));
				DrawElement(elementZoneViewModel.ElementZoneView, elementRectangleZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}
            foreach (var elementPolygonZone in Plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
			{
				var elementZoneViewModel = new ElementZoneViewModel(elementPolygonZone);
				DrawElement(elementZoneViewModel.ElementZoneView, elementPolygonZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}
			foreach (var elementDevice in Plan.ElementDevices)
			{
				if (elementDevice.DeviceUID == Guid.Empty)
					continue;

				var elementDeviceViewModel = new ElementDeviceViewModel(elementDevice);
				if (elementDeviceViewModel.DeviceState != null)
				{
					Devices.Add(elementDeviceViewModel);

					elementDeviceViewModel.DrawElementDevice();
					if (elementDeviceViewModel.ElementDeviceView.Parent == null)
						Canvas.Children.Add(elementDeviceViewModel.ElementDeviceView);
				}
			}
			foreach (var elementXDevice in Plan.ElementXDevices)
			{
				var elementXDeviceViewModel = new ElementXDeviceViewModel(elementXDevice);
				if (elementXDeviceViewModel.DeviceState != null)
				{
					XDevices.Add(elementXDeviceViewModel);

					elementXDeviceViewModel.DrawElementXDevice();
					if (elementXDeviceViewModel.ElementXDeviceView.Parent == null)
						Canvas.Children.Add(elementXDeviceViewModel.ElementXDeviceView);
				}
			}
		}

		private void DrawElement(ElementBase elementBase)
		{
			IPainter painter = PainterFactory.Create(elementBase);
			var frameworkElement = painter.Draw(elementBase);
			DrawElement(elementBase, frameworkElement);
		}
		private void DrawElement(FrameworkElement frameworkElement, ElementBase elementBase, BaseViewModel elementViewModel)
		{
			frameworkElement.DataContext = elementViewModel;
			DrawElement(elementBase, frameworkElement);
		}
		private void DrawElement(ElementBase elementBase, FrameworkElement frameworkElement)
		{
			var rect = elementBase.GetRectangle();
			frameworkElement.Width = rect.Width + elementBase.BorderThickness;
			frameworkElement.Height = rect.Height + elementBase.BorderThickness;
			Canvas.SetLeft(frameworkElement, rect.Left);
			Canvas.SetTop(frameworkElement, rect.Top);
			Canvas.Children.Add(frameworkElement);
		}

		ElementPolygonZone RectangleZoneToPolygon(ElementRectangleZone elementRectangleZone)
		{
			Rect rect = elementRectangleZone.GetRectangle();
			var elementPolygonZone = new ElementPolygonZone()
			{
				Points = new PointCollection(new Point[] { rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft }),
                ZoneUID = elementRectangleZone.ZoneUID
			};

			elementPolygonZone.Points = new PointCollection();
			elementPolygonZone.Points.Add(new Point(0, 0));
			elementPolygonZone.Points.Add(new Point(elementRectangleZone.Width, 0));
			elementPolygonZone.Points.Add(new Point(elementRectangleZone.Width, elementRectangleZone.Height));
			elementPolygonZone.Points.Add(new Point(0, elementRectangleZone.Height));

			return elementPolygonZone;
		}

		void OnElementDeviceSelected(object obj)
		{
			Devices.ForEach(x => x.IsSelected = false);
		}

		void OnElementZoneSelected(object obj)
		{
			Zones.ForEach(x => x.IsSelected = false);
		}

		void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Devices.ForEach(x => x.IsSelected = false);
			Zones.ForEach(x => x.IsSelected = false);
		}

		public void SelectDevice(Guid deviceUID)
		{
			Devices.ForEach(x => x.IsSelected = false);
			Devices.FirstOrDefault(x => x.DeviceUID == deviceUID).IsSelected = true;
		}

		public void SelectZone(Guid zoneUID)
		{
			Zones.ForEach(x => x.IsSelected = false);
            Zones.FirstOrDefault(x => x.ZoneUID == zoneUID).IsSelected = true;
		}

		public void SelectXDevice(XDevice device)
		{
			XDevices.ForEach(x => x.IsSelected = false);
			XDevices.FirstOrDefault(x => x.XDeviceUID == device.UID).IsSelected = true;
		}

		public void SelectXZone(XZone zone)
		{
			//XZones.ForEach(x => x.IsSelected = false);
			//XZones.FirstOrDefault(x => x.ZoneUID == zone.UID).IsSelected = true;
		}

		void OnPlanStateChanged(Guid planUID)
		{
			if ((Plan != null) && (Plan.UID == planUID))
				UpdateSubPlans();
		}

		void UpdateSubPlans()
		{
			foreach (var subPlan in SubPlans)
			{
				var planViewModel = PlansViewModel.Current.Plans.FirstOrDefault(x => x.Plan.UID == subPlan.ElementSubPlan.PlanUID);
				if (planViewModel != null)
					subPlan.StateType = planViewModel.StateType;
			}
		}

		void ResetView()
		{
			if (CanvasView.Current != null)
				CanvasView.Current.Reset();
		}
	}
}