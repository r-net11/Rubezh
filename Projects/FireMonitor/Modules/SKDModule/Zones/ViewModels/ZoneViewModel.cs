using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State
		{
			get { return Zone.State; }
		}

		public ZoneViewModel(SKDZone zone)
		{
			Zone = zone;
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			ZoneCommand = new RelayCommand(OnZone, CanZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("ZoneStateViewModel");
		}

		public string PresentationName
		{
			get { return Zone.Name; }
		}

		public string PresentationDescription
		{
			get { return Zone.Description; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ServiceFactory.OnPublishEvent<SKDZone, ShowSKDZoneOnPlanEvent>(Zone);
		}
		private bool CanShowOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == Zone.UID)))
					return true;
				if (plan.ElementRectangleSKDZones.Any(x => (x.ZoneUID != Guid.Empty) && (x.ZoneUID == Zone.UID)))
					return true;
			}
			return false;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}


		#region Ignore
		public RelayCommand ZoneCommand { get; private set; }
		void OnZone()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				//FiresecManager.FiresecService.SKDSetIgnoreRegime(Zone);
			}
		}
		bool CanZone()
		{
			return true && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}
		#endregion

		public bool IsBold { get; set; }
	}
}