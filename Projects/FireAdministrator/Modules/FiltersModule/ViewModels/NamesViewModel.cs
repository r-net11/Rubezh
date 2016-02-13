using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Common;
using FiresecAPI.Journal;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class NamesViewModel : BaseViewModel
	{
		public NamesViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		private IEnumerable<NameViewModel> GetEventsByType(JournalSubsystemType inputType)
		{
			return (Enum.GetValues(typeof(JournalEventNameType))
				.Cast<JournalEventNameType>()
				.Where(x => x != JournalEventNameType.NULL)
				.Where(journalEventNameType => journalEventNameType.GetAttributeOfType<EventNameAttribute>().JournalSubsystemType == inputType)
				.Select(journalEventNameType => new NameViewModel(journalEventNameType)))
				.OrderBy(x => x.Name)
				.ToList();
		}

		void BuildTree()
		{
			RootNames = new ObservableCollection<NameViewModel>
			{
				new NameViewModel(JournalSubsystemType.System) {IsExpanded = true},
				new NameViewModel(JournalSubsystemType.SKD) {IsExpanded = true},
				new NameViewModel(JournalSubsystemType.Video) {IsExpanded = true},
			};

			RootNames[0].AddChildren(GetEventsByType(JournalSubsystemType.System));
			RootNames[1].AddChildren(GetEventsByType(JournalSubsystemType.SKD));
			RootNames[2].AddChildren(GetEventsByType(JournalSubsystemType.Video));
		}

		void Initialize(JournalFilter filter)
		{
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootNames.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == journalSubsystemTypes);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var rootFilter in RootNames)
			{
				if (rootFilter.IsChecked)
				{
					filter.JournalSubsystemTypes.Add(rootFilter.JournalSubsystemType);
				}
				else
				{
					foreach (var nameViewModel in rootFilter.Children)
					{
						if (nameViewModel.IsChecked)
						{
							filter.JournalEventNameTypes.Add(nameViewModel.JournalEventNameType);
						}
					}
				}
			}
			return filter;
		}

		public ObservableCollection<NameViewModel> RootNames { get; private set; }

		NameViewModel _selectedName;
		public NameViewModel SelectedName
		{
			get { return _selectedName; }
			set
			{
				_selectedName = value;
				OnPropertyChanged(() => SelectedName);
			}
		}
	}
}