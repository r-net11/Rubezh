using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
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
					return true;
				}
			}
			return false;
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