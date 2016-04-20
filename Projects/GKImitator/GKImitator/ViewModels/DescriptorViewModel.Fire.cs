using System;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public void InitializeFire()
		{
			SetFireSmokeCommand = new RelayCommand(OnSetFireSmoke, CanSetFire);
			SetFireTemperatureCommand = new RelayCommand(OnSetFireTemperature, CanSetFire);
			SetFireTemperatureGradientCommand = new RelayCommand(OnSetFireTemperatureGradient, CanSetFire);
			SetFireHeandDetectorCommand = new RelayCommand(OnSetFireHeandDetector, CanSetFire);
			ResetFireCommand = new RelayCommand(OnResetFire, CanSetFire);
			SetFire1Command = new RelayCommand(OnSetFire1, CanSetFire);
			SetFire2Command = new RelayCommand(OnSetFire2, CanSetFire);
		}

		public bool HasSetFireSmoke { get; private set; }
		public bool HasSetFireTemperature { get; private set; }
		public bool HasSetFireTemperatureGradient { get; private set; }
		public bool HasSetFireHandDetector { get; private set; }
		public bool HasResetFire { get; private set; }

		public RelayCommand SetFireSmokeCommand { get; private set; }
		void OnSetFireSmoke()
		{
			var journalItem = new ImitatorJournalItem(2, 2, 2, 0);
			SetStateBit(GKStateBit.Fire1, true, journalItem);
		}

		public RelayCommand SetFireTemperatureCommand { get; private set; }
		void OnSetFireTemperature()
		{
			var journalItem = new ImitatorJournalItem(2, 2, 3, 0);
			SetStateBit(GKStateBit.Fire1, true, journalItem);
		}

		public RelayCommand SetFireTemperatureGradientCommand { get; private set; }
		void OnSetFireTemperatureGradient()
		{
			var journalItem = new ImitatorJournalItem(2, 2, 4, 0);
			SetStateBit(GKStateBit.Fire1, true, journalItem);
		}

		public RelayCommand SetFireHeandDetectorCommand { get; private set; }
		void OnSetFireHeandDetector()
		{
			var journalItem = new ImitatorJournalItem(2, 3, 1, 0);
			SetStateBit(GKStateBit.Fire2, true, journalItem);
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			SetStateBit(GKStateBit.Attention, false);
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			SetStateBit(GKStateBit.Norm, true);
			RecalculateOutputLogic();
		}

		public bool HasReset { get; private set; }

		public RelayCommand SetFire1Command { get; private set; }
		void OnSetFire1()
		{
			if(CanDo(GKStateBit.Fire1))
			{
				SetStateBit(GKStateBit.Attention, false);
				SetStateBit(GKStateBit.Fire2, false);
				SetStateBit(GKStateBit.Fire1, true);
			}
		}

		public RelayCommand SetFire2Command { get; private set; }
		void OnSetFire2()
		{
			if (CanDo(GKStateBit.Fire2))
			{
				SetStateBit(GKStateBit.Attention, false);
				SetStateBit(GKStateBit.Fire1, false);
				SetStateBit(GKStateBit.Fire2, true);
			}
		}

		bool CanSetFire()
		{
			var ignoreState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore);
			return ignoreState == null || !ignoreState.IsActive;
		}
		public bool HasFire1 { get; private set; }
		public bool HasFire12 { get; private set; }
		public bool HasAlarm { get; private set; }
	}
}