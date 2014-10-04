using FiresecAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDayInterval DayInterval { get; private set; }

		public DayIntervalDetailsViewModel(SKDDayInterval dayInterval = null)
		{
			if (dayInterval == null)
			{
				Title = "Создание нового дневного графика";
				DayInterval = new SKDDayInterval()
				{
					Name = "Новый дневной график",
					No = 1,
				};
				if (SKDManager.TimeIntervalsConfiguration.DayIntervals.Count != 0)
					DayInterval.No = (SKDManager.TimeIntervalsConfiguration.DayIntervals.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства дневного графика: {0}", dayInterval.Name);
				DayInterval = dayInterval;
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			No = DayInterval.No;
			Name = DayInterval.Name;
			Description = DayInterval.Description;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged(() => No);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			DayInterval.No = No;
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			return true;
		}
	}
}