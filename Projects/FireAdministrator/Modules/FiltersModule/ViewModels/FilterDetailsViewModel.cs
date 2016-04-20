using RubezhAPI.Journal;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public JournalFilter Filter { get; private set; }
		public NamesViewModel NamesViewModel { get; private set; }
		public ObjectsViewModel ObjectsViewModel { get; private set; }

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
			NamesViewModel = new NamesViewModel(Filter);
			ObjectsViewModel = new ObjectsViewModel(Filter);
			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Filter.Name;
			Description = Filter.Description;
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
		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}

			Filter.Name = Name;
			Filter.Description = Description;

			var namesFilter = NamesViewModel.GetModel();
			Filter.JournalEventNameTypes = namesFilter.JournalEventNameTypes;
			Filter.JournalEventDescriptionTypes = namesFilter.JournalEventDescriptionTypes;

			var objectsFilter = ObjectsViewModel.GetModel();
			Filter.JournalObjectTypes = objectsFilter.JournalObjectTypes;
			Filter.ObjectUIDs = objectsFilter.ObjectUIDs;

			return base.Save();
		}
	}
}