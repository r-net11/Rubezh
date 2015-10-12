using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class DelayPropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKDelay Delay { get; private set; }

		public DelayPropertiesViewModel(GKDelay delay)
		{
			Title = "Параметры задержки";
			Delay = delay;

			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();
			DelayRegime = delay.DelayRegime;
			DelayTime = delay.DelayTime;
			Hold = delay.Hold;
		}

		ushort _delayTime;
		public ushort DelayTime
		{
			get { return _delayTime; }
			set
			{
				_delayTime = value;
				OnPropertyChanged(() => DelayTime);
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
			Delay.DelayTime = DelayTime;
			Delay.Hold = Hold;
			Delay.DelayRegime = DelayRegime;
			return base.Save();
		}
	}
}