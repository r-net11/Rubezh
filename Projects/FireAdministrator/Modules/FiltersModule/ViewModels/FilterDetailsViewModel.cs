using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public static readonly int DefaultDaysCount = 10;

		public FilterDetailsViewModel()
		{
			Title = "Добавить фильтр";
			JournalFilter = new JournalFilter();
			Initialize();
		}

		public FilterDetailsViewModel(JournalFilter journalFilter)
		{
			Title = "Редактировать фильтр";

			JournalFilter = new JournalFilter()
			{
				Name = journalFilter.Name,
				LastRecordsCount = journalFilter.LastRecordsCount,
				LastDaysCount = journalFilter.LastDaysCount,
				IsLastDaysCountActive = journalFilter.IsLastDaysCountActive
			};

			Initialize();

			StateTypes.Where(
				eventViewModel => journalFilter.StateTypes.Any(
					x => x == eventViewModel.StateType)).All(x => x.IsChecked = true);

			Categories.Where(
				categoryViewModel => journalFilter.Categories.Any(
					x => x == categoryViewModel.DeviceCategoryType)).All(x => x.IsChecked = true);
		}

		void Initialize()
		{
			_existingNames = FiresecClient.FiresecManager.SystemConfiguration.JournalFilters.
				Where(journalFilter => journalFilter.Name != JournalFilter.Name).Select(journalFilter => journalFilter.Name).ToList();

			if (_existingNames == null)
				_existingNames = new List<string>();

			StateTypes = new ObservableCollection<StateTypeViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				StateTypes.Add(new StateTypeViewModel(stateType));
			}

			Categories = new ObservableCollection<CategoryViewModel>();
			foreach (DeviceCategoryType deviceCategoryType in Enum.GetValues(typeof(DeviceCategoryType)))
			{
				Categories.Add(new CategoryViewModel(deviceCategoryType));
			}
		}

		public JournalFilter JournalFilter { get; private set; }
		List<string> _existingNames;

		public string FilterName
		{
			get { return JournalFilter.Name; }
			set
			{
				JournalFilter.Name = value;
				OnPropertyChanged("FilterName");
			}
		}

		public ObservableCollection<StateTypeViewModel> StateTypes { get; private set; }
		public ObservableCollection<CategoryViewModel> Categories { get; private set; }

		public JournalFilter GetModel()
		{
			JournalFilter.StateTypes = StateTypes.Where(x => x.IsChecked).Select(x => x.StateType).Cast<StateType>().ToList();
			JournalFilter.Categories = Categories.Where(x => x.IsChecked).Select(x => x.DeviceCategoryType).Cast<DeviceCategoryType>().ToList();
			return JournalFilter;
		}

		protected override bool Save()
		{
			JournalFilter.Name = JournalFilter.Name.Trim();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return this["FilterName"] == null;
		}

		public string Error { get { return null; } }

		public string this[string propertyName]
		{
			get
			{
				if (propertyName != "FilterName")
					throw new ArgumentException();

				if (string.IsNullOrWhiteSpace(FilterName))
					return "Нужно задать имя";

				var name = FilterName.Trim();
				if (_existingNames.IsNotNullOrEmpty() && _existingNames.Any(x => x == name))
					return "Фильтр с таким именем уже существует";
				return null;
			}
		}
	}
}