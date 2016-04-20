using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class DirectionPropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKDirection Direction { get; private set; }

		public DirectionPropertiesViewModel(GKDirection direction)
		{
			Title = "Параметры направления";
			Direction = direction;

			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();
			DelayRegime = Direction.DelayRegime;
			Delay = Direction.Delay;
			Hold = Direction.Hold;
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

		public List<DelayRegime> DelayRegimes { get; private set; }

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
			Direction.Delay = Delay;
			Direction.Hold = Hold;
			Direction.DelayRegime = DelayRegime;
			return base.Save();
		}
	}
}