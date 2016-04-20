using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class DoorPropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKDoor Door { get; private set; }

		public DoorPropertiesViewModel(GKDoor door)
		{
			Title = "Параметры точки доступа";
			Door = door;

			Delay = Door.Delay;
			Hold = Door.Hold;
		}

		int _delay;
		public int Delay
		{
			get { return _delay; }
			set
			{
				_delay = value;
				OnPropertyChanged(() => Delay);
			}
		}

		int _hold;
		public int Hold
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
			Door.Delay = Delay;
			Door.Hold = Hold;
			return base.Save();
		}
	}
}