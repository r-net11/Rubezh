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

		protected override bool Save()
		{
			PumpStation.Delay = Delay;
			PumpStation.Hold = Hold;
			return base.Save();
		}
	}
}