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
using System.Collections.Generic;
using System.Windows.Input;
using Infrastructure.Common.Windows;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		void AddStateBit(GKStateBit stateBit, bool isActive = false)
		{
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

		public bool SetStateBit(GKStateBit stateBit, bool value)
		{
			var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBit);
			if (stateBitViewModel != null)
			{
				if (stateBitViewModel.IsActive != value)
				{
					stateBitViewModel.IsActive = value;
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