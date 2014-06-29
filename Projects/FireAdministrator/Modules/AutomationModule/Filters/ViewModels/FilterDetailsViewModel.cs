using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using System;

namespace AutomationModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationFilter Filter { get; private set; }
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }

		public FilterDetailsViewModel(AutomationFilter filter)
		{
			Title = "Свойства фильтра";
			Filter = filter;
			FilterNamesViewModel = new FilterNamesViewModel();
			CopyProperties();
		}

		public FilterDetailsViewModel()
		{
			Title = "Добавить фильтр";
			Filter = new AutomationFilter();
			FilterNamesViewModel = new FilterNamesViewModel();
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Filter.Name;
			StartDateTime = new DateTimePairViewModel(Filter.StartDate);
			EndDateTime = new DateTimePairViewModel(Filter.EndDate);
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

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		DateTimePairViewModel _startDateTime;
		public DateTimePairViewModel StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged("StartDateTime");
			}
		}

		DateTimePairViewModel _endDateTime;
		public DateTimePairViewModel EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged("EndDateTime");
			}
		}

		bool _useDeviceDateTime;
		public bool UseDeviceDateTime
		{
			get { return _useDeviceDateTime; }
			set
			{
				_useDeviceDateTime = value;
				OnPropertyChanged("UseDeviceDateTime");
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}

			Filter.Name = Name;
			Filter.StartDate = StartDateTime.DateTime;
			Filter.EndDate = EndDateTime.DateTime;
			Filter.UseDeviceDateTime = UseDeviceDateTime;
			return base.Save();
		}
	}
}