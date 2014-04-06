using System;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ZoneViewModel : TreeNodeViewModel<ZoneViewModel>
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
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
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
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowZone(Zone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowZone(Zone);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}
		public bool CanShowProperties()
		{
			return true;
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