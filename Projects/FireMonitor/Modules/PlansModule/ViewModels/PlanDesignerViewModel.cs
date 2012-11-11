using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using PlansModule.Events;
using PlansModule.Views;
using XFiresecAPI;
using PlansModule.ViewModels.Elements;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		private double _zoom;
		private double _deviceZoom;
		public PlanViewModel PlanViewModel { get; private set; }
		public Plan Plan { get; private set; }
		public Canvas Canvas { get; private set; }

		public List<ElementSubPlanViewModel> SubPlans { get; set; }
		public List<ElementZoneViewModel> Zones { get; set; }
		public List<ElementXZoneViewModel> XZones { get; set; }
		public List<ElementDeviceViewModel> Devices { get; set; }
		public List<ElementXDeviceViewModel> XDevices { get; set; }

		public PlanDesignerViewModel()
		{
			ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
			ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Subscribe(OnElementDeviceSelected);
			ServiceFactory.Events.GetEvent<ElementZoneSelectedEvent>().Subscribe(OnElementZoneSelected);
			ServiceFactory.Events.GetEvent<ElementXZoneSelectedEvent>().Subscribe(OnElementXZoneSelected);
		}

		public void Initialize(PlanViewModel planViewModel)
		{
			ChangeZoom(1);
			PlanViewModel = planViewModel;
			Plan = planViewModel.Plan;
			DrawPlan();
			Update();
			OnPropertyChanged("Plan");
			OnPropertyChanged("Canvas");
		}
		public void Update()
		{
			UpdateSubPlans();
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
		public void DrawPlan()
		{
			SubPlans = new List<ElementSubPlanViewModel>();
			Zones = new List<ElementZoneViewModel>();
			XZones = new List<ElementXZoneViewModel>();
			Devices = new List<ElementDeviceViewModel>();
			XDevices = new List<ElementXDeviceViewModel>();

			Canvas = new Canvas()
			{
				Width = Plan.Width,
				Height = Plan.Height
			};
			Canvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_canvas_PreviewMouseLeftButtonDown);

			if (Plan.BackgroundPixels != null)
				Canvas.Background = PainterHelper.CreateBrush(Plan.BackgroundPixels);
			else if (Plan.BackgroundColor == Colors.Transparent)
				Canvas.Background = PainterHelper.CreateTransparentBrush(_zoom);
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
				var elementZoneViewModel = new ElementZoneViewModel(RectangleZoneToPolygon<ElementPolygonZone, ElementRectangleZone>(elementRectangleZone));
				DrawElement(elementZoneViewModel.ElementZoneView, elementRectangleZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}
			foreach (var elementPolygonZone in Plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
			{
				var elementZoneViewModel = new ElementZoneViewModel(elementPolygonZone);
				DrawElement(elementZoneViewModel.ElementZoneView, elementPolygonZone, elementZoneViewModel);
				Zones.Add(elementZoneViewModel);
			}
			foreach (var elementRectangleXZone in Plan.ElementRectangleXZones.Where(x => x.ZoneUID != Guid.Empty))
			{
				var elementXZoneViewModel = new ElementXZoneViewModel(RectangleZoneToPolygon<ElementPolygonXZone, ElementRectangleXZone>(elementRectangleXZone));
				DrawElement(elementXZoneViewModel.ElementXZoneView, elementRectangleXZone, elementXZoneViewModel);
				XZones.Add(elementXZoneViewModel);
			}
			foreach (var elementPolygonXZone in Plan.ElementPolygonXZones.Where(x => x.ZoneUID != Guid.Empty))
			{
				var elementXZoneViewModel = new ElementXZoneViewModel(elementPolygonXZone);
				DrawElement(elementXZoneViewModel.ElementXZoneView, elementPolygonXZone, elementXZoneViewModel);
				XZones.Add(elementXZoneViewModel);
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

		TPolygon RectangleZoneToPolygon<TPolygon, TRectangle>(TRectangle elementRectangleZone)
			where TRectangle : ElementBaseRectangle, IElementZone
			where TPolygon : ElementBasePolygon, IElementZone, new()
		{
			var elementPolygonZone = RectangleToPolygon<TPolygon, TRectangle>(elementRectangleZone);
			elementPolygonZone.ZoneUID = elementRectangleZone.ZoneUID;
			return elementPolygonZone;
		}
		TPolygon RectangleToPolygon<TPolygon, TRectangle>(TRectangle elementRectangle)
			where TRectangle : ElementBaseRectangle
			where TPolygon : ElementBasePolygon, new()
		{
			var elementPolygonZone = new TPolygon();
			elementPolygonZone.Points = new PointCollection();
			elementPolygonZone.Points.Add(new Point(0, 0));
			elementPolygonZone.Points.Add(new Point(elementRectangle.Width, 0));
			elementPolygonZone.Points.Add(new Point(elementRectangle.Width, elementRectangle.Height));
			elementPolygonZone.Points.Add(new Point(0, elementRectangle.Height));
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
		void OnElementXZoneSelected(object obj)
		{
			XZones.ForEach(x => x.IsSelected = false);
		}

		void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Devices.ForEach(x => x.IsSelected = false);
			Zones.ForEach(x => x.IsSelected = false);
			XDevices.ForEach(x => x.IsSelected = false);
			XZones.ForEach(x => x.IsSelected = false);
		}

		public void SelectDevice(Guid deviceUID)
		{
			Devices.ForEach(x => x.IsSelected = x.DeviceUID == deviceUID);
		}
		public void SelectZone(Guid zoneUID)
		{
			Zones.ForEach(x => x.IsSelected = x.ZoneUID == zoneUID);
		}
		public void SelectXDevice(XDevice device)
		{
			XDevices.ForEach(x => x.IsSelected = x.XDeviceUID == device.UID);
		}
		public void SelectXZone(XZone zone)
		{
			XZones.ForEach(x => x.IsSelected = x.ZoneUID == zone.UID);
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
				var planViewModel = PlanViewModel.Children.FirstOrDefault(x => x.Plan.UID == subPlan.ElementSubPlan.PlanUID);
				if (planViewModel != null)
					subPlan.StateType = planViewModel.StateType;
			}
		}

		#region IPlanDesignerViewModel Members

		public event EventHandler Updated;

		public object Toolbox
		{
			get { return null; }
		}

		object IPlanDesignerViewModel.Canvas
		{
			get { return Canvas; }
		}

		public void ChangeZoom(double zoom)
		{
			_zoom = zoom;
			ChangeDeviceZoom(_deviceZoom);
		}

		public void ChangeDeviceZoom(double deviceZoom)
		{
			_deviceZoom = deviceZoom;
			if (Canvas == null)
				return;
			double _pointZoom = _deviceZoom / _zoom;
			foreach (var child in Canvas.Children)
				if (child is ElementDeviceView || child is ElementXDeviceView)
				{
					FrameworkElement elementDeviceView = child as FrameworkElement;
					IElementDevice viewModel = elementDeviceView.DataContext as IElementDevice;
					elementDeviceView.Width = _pointZoom;
					elementDeviceView.Height = _pointZoom;
					Canvas.SetLeft(elementDeviceView, viewModel.Location.X - _pointZoom / 2);
					Canvas.SetTop(elementDeviceView, viewModel.Location.Y - _pointZoom / 2);
				}
		}

		#endregion
	}
}