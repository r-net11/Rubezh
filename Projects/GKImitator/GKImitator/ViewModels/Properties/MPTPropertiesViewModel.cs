using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class MPTPropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKMPT Mpt { get; private set; }

		public MPTPropertiesViewModel(GKMPT mpt)
		{
			Title = "Параметры МПТ";
			Mpt = mpt;
			Delay = Mpt.Delay;
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

		protected override bool Save()
		{
			Mpt.Delay = Delay;
			return base.Save();
		}
	}
}