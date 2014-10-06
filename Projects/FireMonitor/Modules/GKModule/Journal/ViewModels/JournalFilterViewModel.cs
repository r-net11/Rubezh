using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class JournalFilterViewModel : BaseViewModel
	{
		public GKJournalFilter JournalFilter { get; private set; }
		public List<StateClassViewModel> StateClasses { get; private set; }

		public JournalFilterViewModel(GKJournalFilter journalFilter)
		{
			JournalFilter = journalFilter;
			StateClasses = new List<StateClassViewModel>();
			JournalFilter.StateClasses.ForEach(x => StateClasses.Add(new StateClassViewModel(x)));

			EventNames = new List<string>();
			foreach (var stringEventName in journalFilter.EventNames)
			{
				EventNames.Add(stringEventName);
			}
		}

		public bool HasEvents
		{
			get { return JournalFilter.EventNames.Count > 0; }
		}

		public bool HasStates
		{
			get { return JournalFilter.StateClasses.Count > 0; }
		}

		public string LastRecordsCountString
		{
			get { return "Последних записей: " + JournalFilter.LastRecordsCount.ToString(); }
		}

		public bool HasDescription
		{
			get { return JournalFilter.Description != null && JournalFilter.Description != ""; }
		}

		public bool FilterStateClass(GKJournalItem journalItem)
		{
			if (JournalFilter.StateClasses.Count > 0)
			{
				return JournalFilter.StateClasses.Contains(journalItem.StateClass);
			}
			return true;
		}

		public bool FilterEventName(GKJournalItem journalItem)
		{
			if (JournalFilter.EventNames.Count > 0)
			{
				return JournalFilter.EventNames.Any(x => x == journalItem.Name);
			}
			return true;
		}

		public List<string> EventNames { get; private set; }
	}
}