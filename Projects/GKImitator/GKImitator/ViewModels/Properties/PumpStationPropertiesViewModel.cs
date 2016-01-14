using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class PumpStationPropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKPumpStation PumpStation { get; private set; }

		public PumpStationPropertiesViewModel(GKPumpStation pumpStation)
		{
			Title = "Параметры НС";
			PumpStation = pumpStation;
			Delay = PumpStation.Delay;
			Hold = PumpStation.Hold;
			DelayRegime = PumpStation.DelayRegime;
			AvailableDelayRegimeTypes = new List<DelayRegime>(Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>());
		}

		ushort _delay;
		public ushort Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged(() => Hold);
			}
		}

		public List<DelayRegime> AvailableDelayRegimeTypes { get; private set; }
		DelayRegime _delayRegime;
		public DelayRegime DelayRegime
		{
			get { return _delayRegime; }
			set
			{
				_delayRegime = value;
				OnPropertyChanged(() => DelayRegime);
			}
		}

		protected override bool Save()
		{
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			PumpStation.DelayRegime = DelayRegime;
			return base.Save();
		}
	}
}