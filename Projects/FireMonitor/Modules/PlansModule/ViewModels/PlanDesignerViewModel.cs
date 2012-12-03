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
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		private PlansViewModel _plansViewModel;
		private FlushAdorner _flushAdorner;
		private double _zoom;
		private double _deviceZoom;
		public PlanViewModel PlanViewModel { get; private set; }
		public Plan Plan { get; private set; }
		public Canvas Canvas { get; private set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			_plansViewModel = plansViewModel;
			Canvas = new Canvas();
			_flushAdorner = new FlushAdorner(Canvas);
		}

		public void Initialize(PlanViewModel planViewModel)
		{
			ChangeZoom(1);
			PlanViewModel = planViewModel;
			Plan = PlanViewModel == null ? null : planViewModel.Plan;
			if (Plan != null)
			{
				DrawPlan();
				Update();
			}
			OnPropertyChanged("Plan");
			OnPropertyChanged("Canvas");
		}

		public void DrawPlan()
		{
			Canvas.Children.Clear();
			UpdateCanvas();
			CreatePresenters();
		}
		private void UpdateCanvas()
		{
			Canvas.Width = Plan.Width;
			Canvas.Height = Plan.Height;

			if (Plan.BackgroundPixels != null)
				Canvas.Background = PainterHelper.CreateBrush(Plan.BackgroundPixels);
			else if (Plan.BackgroundColor == Colors.Transparent)
				Canvas.Background = PainterHelper.CreateTransparentBrush(_zoom);
			else
				Canvas.Background = new SolidColorBrush(Plan.BackgroundColor);
		}
		private void CreatePresenters()
		{
			foreach (var elementBase in PlanEnumerator.EnumeratePrimitives(Plan))
				CreatePresenter(elementBase).Redraw();

			foreach (var elementBase in Plan.ElementSubPlans)
			{
				var presenterItem = CreatePresenter(elementBase);
				presenterItem.OverridePainter(new MonitorSubPlanPainter(presenterItem, PlanViewModel));
				presenterItem.Redraw();
			}

			foreach (var planPresenter in _plansViewModel.PlanPresenters)
				foreach (var element in planPresenter.LoadPlan(Plan))
				{
					PresenterItem presenterItem = CreatePresenter(element);
					planPresenter.RegisterPresenterItem(presenterItem);
					presenterItem.Redraw();
				}
		}
		private PresenterItem CreatePresenter(ElementBase elementBase)
		{
			var presenterItem = new PresenterItem(elementBase);
			Canvas.Children.Add(presenterItem);
			presenterItem.SetZIndex();
			return presenterItem;
		}

		public IEnumerable<PresenterItem> Items
		{
			get { return Canvas.Children.OfType<PresenterItem>(); }
		}
		public PresenterItem SelectedItem
		{
			get { return Canvas.Children.OfType<PresenterItem>().FirstOrDefault(item => item.IsSelected); }
		}

		public void Update()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}
		public void Navigate(PresenterItem presenterItem)
		{
			_flushAdorner.Show(presenterItem);
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
			if (Plan != null)
				UpdateCanvas();
			ChangeDeviceZoom(_deviceZoom);
		}

		public void ChangeDeviceZoom(double deviceZoom)
		{
			_deviceZoom = deviceZoom;
			if (Canvas == null)
				return;
			double _pointZoom = _deviceZoom / _zoom;
			foreach (var item in Canvas.Children.OfType<PresenterItem>())
				item.UpdateDeviceZoom(_zoom, _pointZoom);
			_flushAdorner.UpdateDeviceZoom(_zoom, _pointZoom);
		}

		#endregion
	}
}