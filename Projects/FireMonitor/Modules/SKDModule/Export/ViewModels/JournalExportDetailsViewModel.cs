
using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
namespace SKDModule.ViewModels
{
	public class JournalExportDetailsViewModel:BaseViewModel
	{
		public JournalExportDetailsViewModel(JournalExportFilter filter)
		{
			IsWithJournal = filter.IsExportJournal;
			IsWithPassJournal = filter.IsExportPassJournal;
			MinDate = filter.MinDate.Date;
			MinTime = filter.MinDate.TimeOfDay;
			MaxDate = filter.MaxDate.Date;
			MaxTime = filter.MaxDate.TimeOfDay;
			Path = filter.Path;
		}

		DateTime _MinDate;
		public DateTime MinDate
		{
			get { return _MinDate; }
			set
			{
				_MinDate = value;
				OnPropertyChanged(() => MinDate);
			}
		}

		TimeSpan _MinTime;
		public TimeSpan MinTime
		{
			get { return _MinTime; }
			set
			{
				_MinTime = value;
				OnPropertyChanged(() => MinTime);
			}
		}
		
		DateTime _MaxDate;
		public DateTime MaxDate
		{
			get { return _MaxDate; }
			set
			{
				_MaxDate = value;
				OnPropertyChanged(() => MaxDate);
			}
		}

		TimeSpan _MaxTime;
		public TimeSpan MaxTime
		{
			get { return _MaxTime; }
			set
			{
				_MaxTime = value;
				OnPropertyChanged(() => MaxTime);
			}
		}

		bool _IsWithJournal;
		public bool IsWithJournal
		{
			get { return _IsWithJournal; }
			set
			{
				_IsWithJournal = value;
				OnPropertyChanged(() => IsWithJournal);
			}
		}

		bool _IsWithPassJournal;
		public bool IsWithPassJournal
		{
			get { return _IsWithPassJournal; }
			set
			{
				_IsWithPassJournal = value;
				OnPropertyChanged(() => IsWithPassJournal);
			}
		}

		string _Path;
		public string Path
		{
			get { return _Path; }
			set
			{
				_Path = value;
				OnPropertyChanged(() => Path);
			}
		}

		public JournalExportFilter Filter
		{
			get
			{
				return new JournalExportFilter
				{
					IsExportJournal = IsWithJournal,
					IsExportPassJournal = IsWithPassJournal,
					Path = Path,
					MinDate = MinDate.Add(MinTime),
					MaxDate = MaxDate.Add(MaxTime)
				};
			}
		}
	}
}
