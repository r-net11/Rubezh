using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Presenter;
using Infrastructure.Plans.ViewModels;
using PlansModule.Designer;
using PlansModule.Primitives;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public double Zoom = 1;
		private PlansViewModel _plansViewModel;
		private FlushAdorner _flushAdorner;
		public PlanViewModel PlanViewModel { get; private set; }
		public double DeviceZoom { get; set; }
		public Plan Plan { get; private set; }
		private PresenterCanvas DesignerCanvas { get; set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel, LayoutPartPlansProperties properties)
		{
			_plansViewModel = plansViewModel;
			DesignerCanvas = new PresenterCanvas();
			_flushAdorner = new FlushAdorner(DesignerCanvas);
			ShowZoomSliders = properties.ShowZoomSliders;
			DeviceZoom = properties.DeviceZoom;
			AllowChangePlanZoom = properties.AllowChangePlanZoom;
		}

		public void SelectPlan(PlanViewModel planViewModel)
		{
			_flushAdorner.Hide();
			using (new TimeCounter("\tPlanDesignerViewModel.SelectPlan: {0}", true, true))
			{
				PlanViewModel = planViewModel;
				Plan = PlanViewModel == null || PlanViewModel.PlanFolder != null ? null : planViewModel.Plan;
				OnPropertyChanged(() => Plan);
				OnPropertyChanged(() => IsNotEmpty);
				using (new WaitWrapper())
					if (Plan != null)
					{
						using (new TimeCounter("\t\tDesignerCanvas.Initialize: {0}"))
							DesignerCanvas.Initialize(Plan);
						using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
							CreatePresenters();
						using (new TimeCounter("\t\tPlanDesignerViewModel.OnUpdated: {0}"))
							Update();
						DesignerCanvas.LoadingFinished();
					}
			}
		}

		private void CreatePresenters()
		{
			foreach (var elementBase in PlanEnumerator.EnumeratePrimitives(Plan))
			{
				if (!(elementBase is ElementTextBox))
				{
					var presenterItem = DesignerCanvas.CreatePresenterItem(elementBase);
					var painter = PrimitivePainterFactory.CreatePainter(this.DesignerCanvas, elementBase);
					presenterItem.OverridePainter(painter);
				}
			}

			foreach (var elementBase in Plan.ElementSubPlans)
			{
				var presenterItem = DesignerCanvas.CreateMonitorPresenterItem(elementBase);
				presenterItem.OverridePainter(new MonitorSubPlanPainter(presenterItem, elementBase.PlanUID));
			}
			foreach (var elementBase in Plan.ElementPolygonSubPlans)
			{
				var presenterItem = DesignerCanvas.CreateMonitorPresenterItem(elementBase);
				presenterItem.OverridePainter(new MonitorPolygonSubPlanPainter(presenterItem, elementBase.PlanUID));
			}

			foreach (var elementBase in Plan.ElementTextBoxes)
			{
				var presenterItem = DesignerCanvas.CreateMonitorTextBoxPresenterItem(elementBase);
				presenterItem.OverridePainter(new MonitorTextBoxPainter(presenterItem));
			}

			foreach (var planPresenter in _plansViewModel.PlanPresenters)
				foreach (var element in planPresenter.LoadPlan(Plan))
				{
					PresenterItem presenterItem = DesignerCanvas.CreatePresenterItem(element);
					planPresenter.RegisterPresenterItem(presenterItem);
				}
			DesignerCanvas.Refresh();
		}

		public IEnumerable<PresenterItem> PresenterItems
		{
			get { return DesignerCanvas.PresenterItems; }
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

		public bool IsNotEmpty
		{
			get { return Plan != null; }
		}
		public object Toolbox
		{
			get { return null; }
		}
		public CommonDesignerCanvas Canvas
		{
			get { return DesignerCanvas; }
		}
		public bool AllowScalePoint
		{
			get
			{
				return ShowZoomSliders;
			}
		}
		public bool AllowChangePlanZoom { get; set; }
		public bool ShowZoomSliders { get; set; }
		public bool FullScreenSize
		{
			get { return true; }
		}

		public void ChangeZoom(double zoom)
		{
			Zoom = zoom;
			DesignerCanvas.UpdateZoom(Zoom, DeviceZoom);
			_flushAdorner.UpdateDeviceZoom(Zoom, DeviceZoom);
		}
		public void ChangeDeviceZoom(double deviceZoom)
		{
			DeviceZoom = deviceZoom;
			DesignerCanvas.UpdateZoomPoint(Zoom, DeviceZoom);
			_flushAdorner.UpdateDeviceZoom(Zoom, DeviceZoom);
		}
		public void ResetZoom(double zoom, double deviceZoom)
		{
			DeviceZoom = deviceZoom;
			ChangeZoom(zoom);
		}

		public bool HasPermissionsToScale
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_ChangeView); }
		}
		public bool AlwaysShowScroll
		{
			get { return false; }
		}

		public bool CanCollapse
		{
			get { return false; }
		}
		public bool IsCollapsed { get; set; }

		#endregion
	}
}