using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using System;
using FiresecAPI.Models;
using FiresecAPI;
using FiresecAPI.Journal;

namespace FiltersModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public JournalFilter Filter { get; private set; }
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }

		public FilterDetailsViewModel(JournalFilter filter)
		{
			Title = "Свойства фильтра";
			Filter = filter;
			FilterNamesViewModel = new FilterNamesViewModel(Filter);
			CopyProperties();
		}

		public FilterDetailsViewModel()
		{
			Title = "Добавить фильтр";
			Filter = new JournalFilter();
			FilterNamesViewModel = new FilterNamesViewModel(Filter);
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Filter.Name;
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

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}

			Filter.Name = Name;

			foreach (var filterNameViewModel in FilterNamesViewModel.AllFilters)
			{
				if (filterNameViewModel.IsChecked)
				{
					Filter.JournalEventNameTypes.Add(filterNameViewModel.JournalEventNameType);
				}
			}

			return base.Save();
		}
	}
}