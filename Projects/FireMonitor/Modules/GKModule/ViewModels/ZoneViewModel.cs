using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public XZoneState ZoneState { get; private set; }
		public XZone Zone
		{
			get { return ZoneState.Zone; }
		}

		public ZoneViewModel(XZoneState zoneState)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ZoneState = zoneState;
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("ToolTip");
		}

		public string ToolTip
		{
			get
			{
				var toolTip = Zone.PresentationName;
                toolTip += "\n" + "Состояние: " + ZoneState.StateClass.ToDescription();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 1: " + Zone.Fire1Count.ToString();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 2: " + Zone.Fire2Count.ToString();
				return toolTip;
			}
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

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			ObjectCommandSendHelper.ResetZone(Zone);
		}
		bool CanResetFire()
		{
			return ZoneState.StateBits.Contains(XStateBit.Fire2) || ZoneState.StateBits.Contains(XStateBit.Fire1) || ZoneState.StateBits.Contains(XStateBit.Attention);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			ObjectCommandSendHelper.SetIgnoreRegimeForZone(Zone);
		}
		bool CanSetIgnore()
		{
            return !ZoneState.StateBits.Contains(XStateBit.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			ObjectCommandSendHelper.SetAutomaticRegimeForZone(Zone);
		}
		bool CanResetIgnore()
		{
            return ZoneState.StateBits.Contains(XStateBit.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(Zone);
			DialogService.ShowWindow(zoneDetailsViewModel);
		}
	}
}