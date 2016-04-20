using System;
using RubezhAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class StateBitViewModel : BaseViewModel
	{
		public GKStateBit StateBit { get; private set; }
		DescriptorViewModel DescriptorViewModel;

		public StateBitViewModel(DescriptorViewModel descriptorViewModel, GKStateBit stateBit, bool isActive = false)
		{
			DescriptorViewModel = descriptorViewModel;
			StateBit = stateBit;
			_isActive = isActive;
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(() => IsActive);
					MainViewModel.Current.RebuildIndicators();
				}
			}
		}
	}
}