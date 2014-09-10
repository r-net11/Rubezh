using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDayInterval DayInterval { get; private set; }

		public DayIntervalDetailsViewModel(SKDDayInterval dayInterval)
		{
			Title = "Редактирование дневного графика";
			DayInterval = dayInterval;
			Name = DayInterval.Name;
			Description = DayInterval.Description;
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
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
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			return true;
		}
	}
}