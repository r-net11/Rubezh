using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using PlansModule.Designer;
using FiresecClient;
using Infrustructure.Plans.Designer;
using Common;
using System.Diagnostics;

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
		public Canvas DesignerCanvas { get; private set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			_plansViewModel = plansViewModel;
			DesignerCanvas = new Canvas();
			_flushAdorner = new FlushAdorner(DesignerCanvas);
			FiresecManager.UserChanged += new Action(() => { OnPropertyChanged("HasPermissionsToScale"); });
		}

		public void Initialize()
		{
			
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
		public void SelectPlan(PlanViewModel planViewModel)
		{
			using (new TimeCounter("PlanDesignerViewModel.SelectedPlan: {0}"))
			{
				PlanViewModel = planViewModel;
				Plan = PlanViewModel == null ? null : planViewModel.Plan;
				if (Plan != null)
				{
					DrawPlan();
					Update();
				}
				OnPropertyChanged("Plan");
				OnPropertyChanged("Canvas");
				//DesignerCanvas.ShowPlan(plan);
			}
			Debug.WriteLine("===========================================");
		}

		public void DrawPlan()
		{
			DesignerCanvas.Children.Clear();
			UpdateCanvas();
			CreatePresenters();
		}
		private void UpdateCanvas()
		{
			DesignerCanvas.Width = Plan.Width;
			DesignerCanvas.Height = Plan.Height;

			if (Plan.BackgroundPixels != null)
				DesignerCanvas.Background = PainterHelper.CreateBrush(Plan.BackgroundPixels);
			else if (Plan.BackgroundColor == Colors.Transparent)
				DesignerCanvas.Background = PainterHelper.CreateTransparentBrush(_zoom);
			else
				DesignerCanvas.Background = new SolidColorBrush(Plan.BackgroundColor);
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
			//Canvas.Children.Add(presenterItem);
			//presenterItem.SetZIndex();
			return presenterItem;
		}

		public IEnumerable<PresenterItem> Items
		{
			get { return DesignerCanvas.Children.OfType<PresenterItem>(); }
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
			get { return DesignerCanvas; }
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
			if (DesignerCanvas == null)
				return;
			double _pointZoom = _deviceZoom / _zoom;
			foreach (var item in DesignerCanvas.Children.OfType<PresenterItem>())
				item.UpdateDeviceZoom(_zoom, _pointZoom);
			_flushAdorner.UpdateDeviceZoom(_zoom, _pointZoom);
		}
		public void ResetZoom(double zoom, double deviceZoom)
		{
			_deviceZoom = deviceZoom;
			ChangeZoom(zoom);
		}

		public bool HasPermissionsToScale
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ChangeView); }
		}

		#endregion
	}
}