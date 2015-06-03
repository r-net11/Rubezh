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

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch(device.DriverType)
				{
					case GKDriverType.RSR2_SmokeDetector:
						HasSetFireSmoke = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						break;
					case GKDriverType.RSR2_CombinedDetector:
						HasSetFireTemperatureGradient = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						break;
					case GKDriverType.RSR2_HeatDetector:
						HasSetFireTemperature = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						break;
					case GKDriverType.RSR2_HandDetector:
						HasSetFireHeandDetector = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
						break;
					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_MAP4:
						break;
				}
			}
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
		}

		public RelayCommand SetFireTemperatureCommand { get; private set; }
		void OnSetFireTemperature()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 2, 3, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand SetFireTemperatureGradientCommand { get; private set; }
		void OnSetFireTemperatureGradient()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 2, 4, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand SetFireHeandDetectorCommand { get; private set; }
		void OnSetFireHeandDetector()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire2).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 3, 1, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire2).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			AddJournalItem(journalItem);
		}
	}
}