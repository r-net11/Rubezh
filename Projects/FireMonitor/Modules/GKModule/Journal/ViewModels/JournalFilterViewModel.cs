using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
	public class JournalFilterViewModel : BaseViewModel
	{
		public XJournalFilter JournalFilter { get; private set; }
		public List<StateClassViewModel> StateClasses { get; private set; }

		public JournalFilterViewModel(XJournalFilter journalFilter)
		{
			JournalFilter = journalFilter;
			StateClasses = new List<StateClassViewModel>();
			JournalFilter.StateClasses.ForEach(x => StateClasses.Add(new StateClassViewModel(x)));

			EventNames = new List<EventName>();
			foreach (var stringEventName in journalFilter.EventNames)
			{
				var eventName = EventNameHelper.EventNames.FirstOrDefault(x => x.Name == stringEventName);
				if (eventName != null)
					EventNames.Add(eventName);
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

		public bool FilterStateClass(JournalItem journalItem)
		{
			if (JournalFilter.StateClasses.Count > 0)
			{
				return JournalFilter.StateClasses.Contains(journalItem.StateClass);
			}
			return true;
		}

		public bool FilterEventName(JournalItem journalItem)
		{
			if (JournalFilter.EventNames.Count > 0)
			{
				return JournalFilter.EventNames.Any(x => x == journalItem.Name);
			}
			return true;
		}

		public List<EventName> EventNames { get; private set; }
	}
}