using System;
using System.Linq;
using GKImitator.Processor;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		void AddStateBit(GKStateBit stateBit, bool isActive = false)
		{
			if(StateBits.All(x => x.StateBit != stateBit))
				StateBits.Add(new StateBitViewModel(this, stateBit, isActive));
		}

		public bool GetStateBit(GKStateBit stateBit)
		{
			var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBit);
			if (stateBitViewModel != null)
			{
				return stateBitViewModel.IsActive;
			}
			return false;
		}

		public bool SetStateBit(GKStateBit stateBit, bool value, ImitatorJournalItem additionalJournalItem = null)
		{
			var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBit);
			if (stateBitViewModel != null)
			{
				if (stateBitViewModel.IsActive != value)
				{
					stateBitViewModel.IsActive = value;
					OnStateBitChanged(stateBit, value, additionalJournalItem);
					NotifyIndicators(stateBit, value);
					return true;
				}
			}
			return false;
		}

		void NotifyIndicators(GKStateBit stateBit, bool value)
		{
			var fireZoneCondition = GKBase is GKZone && (stateBit == GKStateBit.Attention || stateBit == GKStateBit.Fire1 || stateBit == GKStateBit.Fire2);
			var manualCondition = HasManualRegime && stateBit == GKStateBit.Norm;
			var ignoreCondition = HasIgnoreRegime && stateBit == GKStateBit.Ignore;
			if (GKBase is GKMPT || GKBase is GKPumpStation || GKBase is GKDirection)
			{
				if (stateBit == GKStateBit.On || stateBit == GKStateBit.TurningOn && (value || TurningState == TurningState.Paused))
					OnStateChanged(GKStateBit.Reserve1, value, GKBase.UID);
				OnStateChanged(GKStateBit.Reserve2, TurningState == TurningState.Paused, GKBase.UID);
			}
			if (fireZoneCondition ||  manualCondition || ignoreCondition || stateBit == GKStateBit.Failure)
			{
				if (OnStateChanged != null)
					OnStateChanged(stateBit, value, GKBase.UID);
			}
		}

		int StatesToInt()
		{
			var state = 0;
			foreach (var stateBitViewModel in StateBits)
			{
				if (stateBitViewModel.IsActive)
				{
					state += (1 << (int)stateBitViewModel.StateBit);
				}
			}
			return state;
		}
	}
}