using FiresecClient;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Presenter;
using PlansModule.Designer;
using StrazhAPI.Models;
using System;
using System.Collections.Generic;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public double Zoom = 1;
		public double DeviceZoom = CommonDesignerItem.DefaultPointSize;
		private readonly PlansViewModel _plansViewModel;
		private readonly FlushAdorner _flushAdorner;
		public PlanViewModel PlanViewModel { get; private set; }
		public Plan Plan { get; private set; }
		private PresenterCanvas DesignerCanvas { get; set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			_plansViewModel = plansViewModel;
			DesignerCanvas = new PresenterCanvas();
			_flushAdorner = new FlushAdorner(DesignerCanvas);
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
		}

		public void SelectPlan(PlanViewModel planViewModel)
		{
			_flushAdorner.Hide();
			PlanViewModel = planViewModel;
			Plan = PlanViewModel == null || PlanViewModel.PlanFolder != null ? null : planViewModel.Plan;
			OnPropertyChanged(() => Plan);
			OnPropertyChanged(() => IsNotEmpty);

			if (Plan == null) return;

			DesignerCanvas.Initialize(Plan);
			CreatePresenters();
			Update();
			DesignerCanvas.LoadingFinished();
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
					var presenterItem = DesignerCanvas.CreatePresenterItem(element);
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