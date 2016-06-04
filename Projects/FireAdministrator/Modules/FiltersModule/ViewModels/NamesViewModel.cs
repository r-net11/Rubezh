using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Journal;
using Infrastructure.Common.TreeList;
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

		private void BuildTree()
		{
			RootNames = new ObservableCollection<NameViewModel>
			{
				new NameViewModel(JournalSubsystemType.System),
				new NameViewModel(JournalSubsystemType.SKD),
				new NameViewModel(JournalSubsystemType.Video),
			};

			RootNames[0].AddChildren(GetEventsByType(JournalSubsystemType.System));
			RootNames[1].AddChildren(GetEventsByType(JournalSubsystemType.SKD));
			RootNames[2].AddChildren(GetEventsByType(JournalSubsystemType.Video));
		}

		private void Initialize(JournalFilter filter)
		{
			// Тип события
			var allJournalEventNameTypes = new List<NameViewModel>();
			foreach (var rootName in RootNames)
			{
				allJournalEventNameTypes.AddRange(rootName.Children.ToList());	
			}
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = allJournalEventNameTypes.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
					ExpandParent(filterNameViewModel);
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var rootName in RootNames)
			{
				foreach (var nameViewModel in rootName.Children)
				{
					if (nameViewModel.IsChecked)
						filter.JournalEventNameTypes.Add(nameViewModel.JournalEventNameType);
				}
			}
			return filter;
		}

		public ObservableCollection<NameViewModel> RootNames { get; private set; }

		private NameViewModel _selectedName;
		public NameViewModel SelectedName
		{
			get { return _selectedName; }
			set
			{
				_selectedName = value;
				OnPropertyChanged(() => SelectedName);
			}
		}

		private void ExpandParent(TreeNodeViewModel<NameViewModel> child)
		{
			if (child.Parent == null)
				return;

			child.Parent.IsExpanded = true;
			ExpandParent(child.Parent);
		}
	}
}