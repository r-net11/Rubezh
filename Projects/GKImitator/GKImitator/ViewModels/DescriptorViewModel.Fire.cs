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
		}

		public bool HasSetFireSmoke { get; private set; }
		public bool HasSetFireTemperature { get; private set; }
		public bool HasSetFireTemperatureGradient { get; private set; }
		public bool HasSetFireHeandDetector { get; private set; }
		public bool HasResetFire { get; private set; }

		public RelayCommand SetFireSmokeCommand { get; private set; }
		void OnSetFireSmoke()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 2, 2, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireTemperatureCommand { get; private set; }
		void OnSetFireTemperature()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 2, 3, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireTemperatureGradientCommand { get; private set; }
		void OnSetFireTemperatureGradient()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 2, 4, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand SetFireHeandDetectorCommand { get; private set; }
		void OnSetFireHeandDetector()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire2).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 3, 1, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			var attentionStateBit = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Attention);
			if (attentionStateBit != null)
				attentionStateBit.IsActive = false;
			var fire1StateBit = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1);
			if (fire1StateBit != null)
				fire1StateBit.IsActive = false;
			var fire2StateBit = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire2);
			if (fire2StateBit != null)
				fire2StateBit.IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public bool HasReset { get; private set; }
	}
}