using FiresecAPI.Journal;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public JournalFilter Filter { get; private set; }
		public FilterNamesViewModel FilterNamesViewModel { get; private set; }
		public FilterObjectsViewModel FilterObjectsViewModel { get; private set; }

		public FilterDetailsViewModel(JournalFilter filter = null)
		{
			if (filter == null)
			{
				Title = "Добавить фильтр";
				Filter = new JournalFilter();
				Name = "Новый фильтр";
			}
			else
			{
				Title = "Свойства фильтра";
				Filter = filter;
			}
			FilterNamesViewModel = new FilterNamesViewModel(Filter);
			FilterObjectsViewModel = new FilterObjectsViewModel(Filter);
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Filter.Name;
			Description = Filter.Description;
			LastItemsCount = Filter.LastItemsCount;
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

		int _lastItemsCount;
		public int LastItemsCount
		{
			get { return _lastItemsCount; }
			set
			{
				_lastItemsCount = value;
				OnPropertyChanged(() => LastItemsCount);
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
			Filter.Description = Description;
			Filter.LastItemsCount = LastItemsCount;

			var namesFilter = FilterNamesViewModel.GetModel();
			Filter.JournalSubsystemTypes = namesFilter.JournalSubsystemTypes;
			Filter.JournalEventNameTypes = namesFilter.JournalEventNameTypes;
			Filter.JournalEventDescriptionTypes = namesFilter.JournalEventDescriptionTypes;

			var objectsFilter = FilterObjectsViewModel.GetModel();
			foreach (var journalSubsystemTypes in objectsFilter.JournalSubsystemTypes)
			{
				if (!Filter.JournalSubsystemTypes.Contains(journalSubsystemTypes))
					Filter.JournalSubsystemTypes.Add(journalSubsystemTypes);
			}
			Filter.JournalObjectTypes = objectsFilter.JournalObjectTypes;
			Filter.ObjectUIDs = objectsFilter.ObjectUIDs;

			return base.Save();
		}
	}
}