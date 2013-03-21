using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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
			SendControlCommand(XStateType.Reset);
		}
		bool CanResetFire()
		{
			return ZoneState.States.Contains(XStateType.Fire1);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			SendControlCommand(XStateType.SetRegime_Off);
		}
		bool CanSetIgnore()
		{
            return !ZoneState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			SendControlCommand(XStateType.SetRegime_Automatic);
		}
		bool CanResetIgnore()
		{
            return ZoneState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		void SendControlCommand(XStateType stateType)
		{
			var code = 0x80 + (int)stateType;
			ObjectCommandSendHelper.SendControlCommand(Zone, (byte)code);
		}
	}
}