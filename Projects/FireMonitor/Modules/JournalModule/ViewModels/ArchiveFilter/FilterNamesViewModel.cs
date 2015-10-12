using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Reflection;

namespace JournalModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(ArchiveFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(ArchiveFilter filter)
		{
			AllFilters.ForEach(x => x.IsChecked = false);
			foreach (var eventName in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == eventName);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
            foreach (var journalEventDescriptionType in filter.JournalEventDescriptionTypes)
            {
                var filterNameViewModel = AllFilters.FirstOrDefault(x => x.JournalEventDescriptionType == journalEventDescriptionType);
                if (filterNameViewModel != null)
                {
                    filterNameViewModel.IsChecked = true;
                    filterNameViewModel.Parent.IsExpanded = true;

                }
            }
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.JournalSubsystemType == journalSubsystemTypes);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				    
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

			var videoViewModel = new FilterNameViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootFilters.Add(videoViewModel);
			AllFilters.Add(videoViewModel);

			foreach (JournalEventNameType enumValue in Enum.GetValues(typeof(JournalEventNameType)))
			{
				var filterNameViewModel = new FilterNameViewModel(enumValue);
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

                    //case JournalSubsystemType.Video:
                    //    videoViewModel.AddChild(filterNameViewModel);
                    //    break;
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
                            var eventViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
                            if (eventViewModel != null)
                            {
                                var descriptionViewModel = new FilterNameViewModel(journalEventDescriptionType, eventDescriptionAttribute.Name);
                                eventViewModel.AddChild(descriptionViewModel);
                                AllFilters.Add(descriptionViewModel);
                            }
                        }
                    }
                }
            }
		}
	}
}