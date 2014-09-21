using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
using System.Reflection;

namespace FiltersModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterNameViewModel>();
			AllFilters = new List<FilterNameViewModel>();

			var systemViewModel = new FilterNameViewModel(JournalSubsystemType.System);
			systemViewModel.IsExpanded = true;
			RootFilters.Add(systemViewModel);

			var gkViewModel = new FilterNameViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var skdViewModel = new FilterNameViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			foreach (JournalEventNameType journalEventNameType in Enum.GetValues(typeof(JournalEventNameType)))
			{
				var filterNameViewModel = new FilterNameViewModel(journalEventNameType);
				if (filterNameViewModel.JournalEventNameType == JournalEventNameType.NULL)
					continue;

				AllFilters.Add(filterNameViewModel);

				switch (filterNameViewModel.JournalSubsystemType)
				{
					case JournalSubsystemType.System:
						systemViewModel.AddChild(filterNameViewModel);
						break;

					case JournalSubsystemType.GK:
						gkViewModel.AddChild(filterNameViewModel);
						break;

					case JournalSubsystemType.SKD:
						skdViewModel.AddChild(filterNameViewModel);
						break;
				}
			}

			foreach (JournalEventDescriptionType journalEventDescriptionType in Enum.GetValues(typeof(JournalEventDescriptionType)))
			{
				FieldInfo fieldInfo = journalEventDescriptionType.GetType().GetField(journalEventDescriptionType.ToString());
				if (fieldInfo != null)
				{
					EventDescriptionAttribute[] eventDescriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
					if (eventDescriptionAttributes.Length > 0)
					{
						EventDescriptionAttribute eventDescriptionAttribute = eventDescriptionAttributes[0];
						foreach (var journalEventNameType in eventDescriptionAttribute.JournalEventNameTypes)
						{
							var filterViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
							if (filterViewModel != null)
							{
								var descriptionViewModel = new FilterNameViewModel(journalEventDescriptionType, eventDescriptionAttribute.Name);
								filterViewModel.AddChild(descriptionViewModel);
								AllFilters.Add(descriptionViewModel);
							}
						}
					}
				}
			}
		}

		public void Initialize(JournalFilter filter)
		{
			AllFilters.ForEach(x => x.IsChecked = false);
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var journalEventDescriptionType in filter.JournalEventDescriptionTypes)
			{
				var descriptionViewModel = AllFilters.FirstOrDefault(x => x.JournalEventDescriptionType == journalEventDescriptionType);
				if (descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
					descriptionViewModel.IsExpanded = true;
				}
			}
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var subsystemViewModel = RootFilters.FirstOrDefault(x => x.JournalSubsystemType == journalSubsystemTypes);
				if (subsystemViewModel != null)
				{
					subsystemViewModel.IsChecked = true;
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var rootFilter in RootFilters)
			{
				if (rootFilter.IsChecked)
				{
					filter.JournalSubsystemTypes.Add(rootFilter.JournalSubsystemType);
				}
				else
				{
					foreach (var filterViewModel in rootFilter.Children)
					{
						if (filterViewModel.IsChecked)
						{
							filter.JournalEventNameTypes.Add(filterViewModel.JournalEventNameType);
						}
						else
						{
							foreach (var descriptionViewModel in filterViewModel.Children)
							{
								if (descriptionViewModel.IsChecked)
								{
									filter.JournalEventDescriptionTypes.Add(descriptionViewModel.JournalEventDescriptionType);
								}
							}
						}
					}
				}
			}
			return filter;
		}

		public List<FilterNameViewModel> AllFilters;

		public ObservableCollection<FilterNameViewModel> RootFilters { get; private set; }

		FilterNameViewModel _selectedFilter;
		public FilterNameViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}
	}
}