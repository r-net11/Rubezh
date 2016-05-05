using StrazhAPI.SKD;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace StrazhModule.ViewModels
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
				};
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
			Name = DayInterval.Name;
			Description = DayInterval.Description;
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
			if (Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever || Name == TimeIntervalsConfiguration.PredefinedIntervalNameAlways)
			{
				MessageBoxService.ShowWarning("Запрещенное назваине");
				return false;
			}
			DayInterval.Name = Name;
			DayInterval.Description = Description;
			return true;
		}
	}
}