using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Journal;
using System.Windows.Input;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public void ClientCommand(GKStateBit stateBit)
		{
			switch (stateBit)
			{
				case GKStateBit.SetRegime_Automatic:
					OnSetAutomaticRegime();
					CommandManager.InvalidateRequerySuggested();
					break;

				case GKStateBit.SetRegime_Manual:
					OnSetManualRegime();
					CommandManager.InvalidateRequerySuggested();
					break;

				case GKStateBit.SetRegime_Off:
					OnSetIgnoreRegime();
					CommandManager.InvalidateRequerySuggested();
					break;

				case GKStateBit.TurnOn_InManual:
					if (Regime == Regime.Manual)
						OnTurnOn();
					break;

				case GKStateBit.TurnOnNow_InManual:
					if (Regime == Regime.Manual)
						OnTurnOnNow();
					break;

				case GKStateBit.TurnOff_InManual:
					if (Regime == Regime.Manual)
						OnTurnOff();
					break;

				case GKStateBit.TurnOffNow_InManual:
					if (Regime == Regime.Manual)
						OnTurnOffNow();
					break;

				case GKStateBit.TurnOn_InAutomatic:
					if (Regime == Regime.Automatic)
						OnTurnOn();
					break;

				case GKStateBit.TurnOnNow_InAutomatic:
					if (Regime == Regime.Automatic)
						OnTurnOnNow();
					break;

				case GKStateBit.TurnOff_InAutomatic:
					if (Regime == Regime.Automatic)
						OnTurnOff();
					break;

				case GKStateBit.TurnOffNow_InAutomatic:
					if (Regime == Regime.Automatic)
						OnTurnOffNow();
					break;

				case GKStateBit.Fire1:
				case GKStateBit.Fire2:
				case GKStateBit.Reset:
					if (Regime == Regime.Automatic)
						OnResetFire();
					break;
			}
			RecalculateOutputLogic();
		}
	}
}