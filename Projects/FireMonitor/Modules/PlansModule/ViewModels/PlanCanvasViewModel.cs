using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Events;
using PlansModule.Views;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace PlansModule.ViewModels
{
	public class PlanCanvasViewModel : ViewPartViewModel
	{
		public Plan Plan { get; private set; }
		public Canvas Canvas { get; private set; }

		public List<ElementSubPlanViewModel> SubPlans { get; set; }
		public List<ElementZoneViewModel> Zones { get; set; }
		public List<ElementDeviceViewModel> Devices { get; set; }

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

			Canvas = new Canvas()
			{
				Width = Plan.Width,
				Height = Plan.Height
			};
			Canvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_canvas_PreviewMouseLeftButtonDown);

			if (Plan.BackgroundPixels != null)
			{
				Canvas.Background = PainterHelper.CreateBrush(Plan.BackgroundPixels); //TODO: ~20-25 % общего времени
			}
			else
			{
				Canvas.Background = new SolidColorBrush(Plan.BackgroundColor);
			}

			foreach (var elementRectangle in Plan.ElementRectangles)
			{
				DrawElement(elementRectangle);
			}
			foreach (var elementEllipse in Plan.ElementEllipses)
			{
				DrawElement(elementEllipse);
			}
			foreach (var elementTextBlock in Plan.ElementTextBlocks)
			{
				DrawElement(elementTextBlock);
			}
			foreach (var elementPolygon in Plan.ElementPolygons)
			{
				DrawElement(elementPolygon);
			}
			if (Plan.ElementPolylines == null)
				Plan.ElementPolylines = new List<ElementPolyline>();
			foreach (var elementPolyline in Plan.ElementPolylines)
			{
				DrawElement(elementPolyline);
			}
			foreach (var elementSubPlan in Plan.ElementSubPlans)
			{
				var subPlanViewModel = new ElementSubPlanViewModel(elementSubPlan);
				DrawElement(subPlanViewModel.ElementSubPlanView, elementSubPlan, subPlanViewModel);
				SubPlans.Add(subPlanViewModel);
			}

			foreach (var elementRectangleZone in Plan.ElementRectangleZones.Where(x => x.ZoneNo != null))
			{
				var elementZoneViewModel = new ElementZoneViewModel(RectangleZoneToPolygon(elementRectangleZone));
				DrawElement(elementZoneViewModel.ElementZoneView, elementRectangleZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}

			foreach (var elementPolygonZone in Plan.ElementPolygonZones.Where(x => x.ZoneNo != null))
			{
				var elementZoneViewModel = new ElementZoneViewModel(elementPolygonZone);
				DrawElement(elementZoneViewModel.ElementZoneView, elementPolygonZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}

			foreach (var elementDevice in Plan.ElementDevices)
			{
				var elementDeviceViewModel = new ElementDeviceViewModel(elementDevice);
				if (elementDeviceViewModel.DeviceState != null)
				{
					Devices.Add(elementDeviceViewModel);

					elementDeviceViewModel.DrawElementDevice();
					if (elementDeviceViewModel.ElementDeviceView.Parent != null)
						return;
					Canvas.Children.Add(elementDeviceViewModel.ElementDeviceView);
				}
			}
		}

		void DrawElement(ElementBase elementBase)
		{
			//var frameworkElement = elementBase.Draw();
			//frameworkElement.Width = elementBase.Width;
			//frameworkElement.Height = elementBase.Height;
			//Canvas.SetLeft(frameworkElement, elementBase.Left);
			//Canvas.SetTop(frameworkElement, elementBase.Top);
			//Canvas.Children.Add(frameworkElement);
		}

		void DrawElement(FrameworkElement frameworkElement, ElementBase elementBase, BaseViewModel elementViewModel)
		{
			frameworkElement.DataContext = elementViewModel;
			//frameworkElement.Width = elementBase.Width;
			//frameworkElement.Height = elementBase.Height;
			//Canvas.SetLeft(frameworkElement, elementBase.Left);
			//Canvas.SetTop(frameworkElement, elementBase.Top);
			Canvas.Children.Add(frameworkElement);
		}

		ElementPolygonZone RectangleZoneToPolygon(ElementRectangleZone elementRectangleZone)
		{
			var elementPolygonZone = new ElementPolygonZone()
			{
				//Left = elementRectangleZone.Left,
				//Top = elementRectangleZone.Top,
				//Width = elementRectangleZone.Width,
				//Height = elementRectangleZone.Height,
				ZoneNo = elementRectangleZone.ZoneNo
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

		public void SelectZone(ulong zoneNo)
		{
			Zones.ForEach(x => x.IsSelected = false);
			Zones.FirstOrDefault(x => x.ZoneNo.Value == zoneNo).IsSelected = true;
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