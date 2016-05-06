using System;
using System.Collections.Generic;
using Common;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Presenter;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public double Zoom = 1;
		public double DeviceZoom = DesignerItem.DefaultPointSize;
		private PlansViewModel _plansViewModel;
		private FlushAdorner _flushAdorner;
		public PlanViewModel PlanViewModel { get; private set; }
		public Plan Plan { get; private set; }
		private PresenterCanvas DesignerCanvas { get; set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			_plansViewModel = plansViewModel;
			DesignerCanvas = new PresenterCanvas();
			_flushAdorner = new FlushAdorner(DesignerCanvas);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
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
				DesignerCanvas.CreatePresenterItem(elementBase).CreatePainter();

			foreach (var elementBase in Plan.ElementSubPlans)
			{
				var presenterItem = DesignerCanvas.CreateMonitorPresenterItem(elementBase);
				presenterItem.OverridePainter(new MonitorSubPlanPainter(presenterItem, elementBase.PlanUID));
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

		void OnUserChanged(UserChangedEventArgs userChangedEventArgs)
		{
			OnPropertyChanged(() => HasPermissionsToScale);
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
			get { return true; }
		}
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
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ChangeView); }
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