using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class GlobalPimActivationViewModel : BaseViewModel
	{
		public XStateClass State
		{
			get
			{
				if (GlobalPims.All(x => x.State.StateClass == XStateClass.On))
					return XStateClass.On;
				if (GlobalPims.All(x => x.State.StateClass == XStateClass.Off))
					return XStateClass.Off;
				return XStateClass.Unknown;
			}
		}

		public GlobalPimActivationViewModel()
		{
			GlobalPims.ForEach(x => x.State.StateChanged += OnStateChanged);
			ChangeGlobalPimActivationCommand = new RelayCommand(OnChangeGlobalPimActivation, CanChangeGlobalPim);
			OnStateChanged();
		}

		GlobalPimStatusType _globalPimStatusType;
		public GlobalPimStatusType GlobalPimStatusType
		{
			get { return _globalPimStatusType; }
			set
			{
				_globalPimStatusType = value;
				OnPropertyChanged(() => GlobalPimStatusType);
			}
		}

		List<GKPim> GlobalPims
		{
			get
			{
				return GKManager.GlobalPims;
			}
		}

		void OnStateChanged()
		{
			switch (State)
			{
				case XStateClass.On:
					GlobalPimStatusType = GlobalPimStatusType.On;
					break;
				case XStateClass.Off:
					GlobalPimStatusType = GlobalPimStatusType.Off;
					break;
				default:
					GlobalPimStatusType = GlobalPimStatusType.Unknown;
					break;
			}
		}



		//public string ToolTipText
		//{
		//	get
		//	{
		//		switch (State)
		//		{
		//			case XStateClass.On:
		//				GlobalPimStatusType = GlobalPimStatusType.On;
		//				return "Включено";
		//			case XStateClass.Off:
		//				GlobalPimStatusType = GlobalPimStatusType.Off;
		//				return "Выключено";
		//			default:
		//				GlobalPimStatusType = GlobalPimStatusType.Unknown;
		//				return "Состояние неизвестно";
		//		}
		//	}
		//}

		public RelayCommand ChangeGlobalPimActivationCommand { get; private set; }

		void OnChangeGlobalPimActivation()
		{
			if (GlobalPimStatusType == GlobalPimStatusType.On)
			{
				ClientManager.FiresecService.GKTurnOffNowGlobalPimsInAutomatic();
			}
			else
			{
				ClientManager.FiresecService.GKTurnOnNowGlobalPimsInAutomatic();
			}
		}

		bool CanChangeGlobalPim()
		{
			return ClientManager.CheckPermission(PermissionType.Oper_GlobalPIM_Control);
		}

		public bool CanShowGlobalPim
		{
			get {return GKManager.MPTs.Count != 0 || GKManager.PumpStations.Count != 0; }
		}
	}

	public enum GlobalPimStatusType
	{
		Unknown = 0,
		On = 1,
		Off = 2
	}
}