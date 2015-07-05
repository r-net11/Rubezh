using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Journal;
using System.Collections.Generic;
using System.Windows.Input;
using Infrastructure.Common.Windows;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		bool GetStateBit(GKStateBit stateBit)
		{
			var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBit);
			if (stateBitViewModel != null)
			{
				return stateBitViewModel.IsActive;
			}
			return false;
		}

		void SetStateBit(GKStateBit stateBit, bool value)
		{
			var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBit);
			if (stateBitViewModel != null)
			{
				stateBitViewModel.IsActive = value;
			}
		}

		public bool HasStateNorm { get; private set; }
		public bool HasStateAttention { get; private set; }
		public bool HasStateFire1 { get; private set; }
		public bool HasStateFire2 { get; private set; }
		public bool HasStateTest { get; private set; }
		public bool HasStateFailure { get; private set; }
		public bool HasStateIgnore { get; private set; }
		public bool HasStateOn { get; private set; }
		public bool HasStateOff { get; private set; }
		public bool HasStateTurningOn { get; private set; }
		public bool HasStateTurningOff { get; private set; }

		bool _isStateNorm;
		public bool IsStateNorm
		{
			get { return _isStateNorm; }
			set
			{
				_isStateNorm = value;
				OnPropertyChanged(() => IsStateNorm);
			}
		}

		bool _isStateAttention;
		public bool IsStateAttention
		{
			get { return _isStateAttention; }
			set
			{
				_isStateAttention = value;
				OnPropertyChanged(() => IsStateAttention);
			}
		}

		bool _isStateFire1;
		public bool IsStateFire1
		{
			get { return _isStateFire1; }
			set
			{
				_isStateFire1 = value;
				OnPropertyChanged(() => IsStateFire1);
			}
		}

		bool _isStateFire2;
		public bool IsStateFire2
		{
			get { return _isStateFire2; }
			set
			{
				_isStateFire2 = value;
				OnPropertyChanged(() => IsStateFire2);
			}
		}

		bool _isStateTest;
		public bool IsStateTest
		{
			get { return _isStateTest; }
			set
			{
				_isStateTest = value;
				OnPropertyChanged(() => IsStateTest);
			}
		}

		bool _isStateFailure;
		public bool IsStateFailure
		{
			get { return _isStateFailure; }
			set
			{
				_isStateFailure = value;
				OnPropertyChanged(() => IsStateFailure);
			}
		}

		bool _isStateIgnore;
		public bool IsStateIgnore
		{
			get { return _isStateIgnore; }
			set
			{
				_isStateIgnore = value;
				OnPropertyChanged(() => IsStateIgnore);
			}
		}

		bool _isStateOn;
		public bool IsStateOn
		{
			get { return _isStateOn; }
			set
			{
				_isStateOn = value;
				OnPropertyChanged(() => IsStateOn);
			}
		}

		bool _isStateStateOff;
		public bool IsStateOff
		{
			get { return _isStateStateOff; }
			set
			{
				_isStateStateOff = value;
				OnPropertyChanged(() => IsStateOff);
			}
		}

		bool _isStateTurningOn;
		public bool IsStateTurningOn
		{
			get { return _isStateTurningOn; }
			set
			{
				_isStateTurningOn = value;
				OnPropertyChanged(() => IsStateTurningOn);
			}
		}

		bool _isStateTurningOff;
		public bool IsStateTurningOff
		{
			get { return _isStateTurningOff; }
			set
			{
				_isStateTurningOff = value;
				OnPropertyChanged(() => IsStateTurningOff);
			}
		}

		int GetState()
		{
			var state = 0;
			if (IsStateNorm) state += (1 << (int)GKStateBit.Norm);
			if (IsStateAttention) state += (1 << (int)GKStateBit.Attention);
			if (IsStateFire1) state += (1 << (int)GKStateBit.Fire1);
			if (IsStateFire2) state += (1 << (int)GKStateBit.Fire2);
			if (IsStateTest) state += (1 << (int)GKStateBit.Test);
			if (IsStateFailure) state += (1 << (int)GKStateBit.Failure);
			if (IsStateIgnore) state += (1 << (int)GKStateBit.Ignore);
			if (IsStateOn) state += (1 << (int)GKStateBit.On);
			if (IsStateOff) state += (1 << (int)GKStateBit.Off);
			if (IsStateTurningOn) state += (1 << (int)GKStateBit.TurningOn);
			if (IsStateTurningOff) state += (1 << (int)GKStateBit.TurningOff);
			return state;
		}
	}
}