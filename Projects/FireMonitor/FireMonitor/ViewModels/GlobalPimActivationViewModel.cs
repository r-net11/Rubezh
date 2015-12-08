using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;

namespace FireMonitor.ViewModels
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
			ChangeGlobalPimActivationCommand = new RelayCommand(OnChangeGlobalPimActivation);
			OnStateChanged();
		}

		bool _isActivate;
		public bool IsActivate
		{
			get { return _isActivate; }
			set
			{
				_isActivate = value;
				OnPropertyChanged(() => IsActivate);
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
			IsActivate = State == XStateClass.On;
		}

		public RelayCommand ChangeGlobalPimActivationCommand { get; private set; }

		void OnChangeGlobalPimActivation()
		{
			if (IsActivate)
			{
				ClientManager.FiresecService.GKTurnOffNowGlobalPimsInAutomatic();
			}
			else
			{
				ClientManager.FiresecService.GKTurnOnNowGlobalPimsInAutomatic();
			}
		}
	}
}