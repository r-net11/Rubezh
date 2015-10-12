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
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public void InitializeFire()
		{
			SetFireSmokeCommand = new RelayCommand(OnSetFireSmoke);
			SetFireTemperatureCommand = new RelayCommand(OnSetFireTemperature);
			SetFireTemperatureGradientCommand = new RelayCommand(OnSetFireTemperatureGradient);
			SetFireHeandDetectorCommand = new RelayCommand(OnSetFireHeandDetector);
			ResetFireCommand = new RelayCommand(OnResetFire);

			SetFire1Command = new RelayCommand(OnSetFire1);
			SetFire2Command = new RelayCommand(OnSetFire2);
			ResetFire12Command = new RelayCommand(OnResetFire12);
		}

		public bool HasSetFireSmoke { get; private set; }
		public bool HasSetFireTemperature { get; private set; }
		public bool HasSetFireTemperatureGradient { get; private set; }
		public bool HasSetFireHeandDetector { get; private set; }
		public bool HasResetFire { get; private set; }

		public RelayCommand SetFireSmokeCommand { get; private set; }
		void OnSetFireSmoke()
		{
			SetStateBit(GKStateBit.Fire1, true);
			var journalItem = new ImitatorJournalItem(2, 2, 2, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireTemperatureCommand { get; private set; }
		void OnSetFireTemperature()
		{
			SetStateBit(GKStateBit.Fire1, true);
			var journalItem = new ImitatorJournalItem(2, 2, 3, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireTemperatureGradientCommand { get; private set; }
		void OnSetFireTemperatureGradient()
		{
			SetStateBit(GKStateBit.Fire1, true);
			var journalItem = new ImitatorJournalItem(2, 2, 4, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireHeandDetectorCommand { get; private set; }
		void OnSetFireHeandDetector()
		{
			SetStateBit(GKStateBit.Fire2, true);
			var journalItem = new ImitatorJournalItem(2, 3, 1, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			SetStateBit(GKStateBit.Attention, false);
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public bool HasReset { get; private set; }

		public RelayCommand SetFire1Command { get; private set; }
		void OnSetFire1()
		{
			SetStateBit(GKStateBit.Fire1, true);
			SetStateBit(GKStateBit.Fire2, false);
			var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFire2Command { get; private set; }
		void OnSetFire2()
		{
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, true);
			var journalItem = new ImitatorJournalItem(2, 3, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand ResetFire12Command { get; private set; }
		void OnResetFire12()
		{
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public bool HasFire12 { get; private set; }
	}
}