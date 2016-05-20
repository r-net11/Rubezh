using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FiltersModule.ViewModels
{
	public class NamesViewModel : BaseViewModel
	{
		public NamesViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		void BuildTree()
		{
			RootNames = new ObservableCollection<NameViewModel>();
			AllNames = new List<NameViewModel>();

			var systemViewModel = new NameViewModel(JournalSubsystemType.System);
			systemViewModel.IsExpanded = true;
			RootNames.Add(systemViewModel);

			var gkViewModel = new NameViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootNames.Add(gkViewModel);

			var skdViewModel = new NameViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootNames.Add(skdViewModel);

			var videoViewModel = new NameViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootNames.Add(videoViewModel);

			foreach (JournalEventNameType journalEventNameType in Enum.GetValues(typeof(JournalEventNameType)))
			{
				var nameViewModel = new NameViewModel(journalEventNameType);
				if (nameViewModel.JournalEventNameType == JournalEventNameType.NULL)
					continue;

				AllNames.Add(nameViewModel);

				switch (nameViewModel.JournalSubsystemType)
				{
					case JournalSubsystemType.System:
						systemViewModel.AddChild(nameViewModel);
						break;

					case JournalSubsystemType.GK:
						gkViewModel.AddChild(nameViewModel);
						break;

					case JournalSubsystemType.SKD:
						skdViewModel.AddChild(nameViewModel);
						break;

					case JournalSubsystemType.Video:
						videoViewModel.AddChild(nameViewModel);
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
						var journalEventNameType = eventDescriptionAttribute.JournalEventNameType;
						var eventViewModel = AllNames.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
						if (eventViewModel != null)
						{
							var descriptionViewModel = new NameViewModel(journalEventDescriptionType, eventDescriptionAttribute.Name);
							eventViewModel.AddChild(descriptionViewModel);
							AllNames.Add(descriptionViewModel);
						}
					}
				}
			}
		}

		void Initialize(JournalFilter filter)
		{
			AllNames.ForEach(x => x.IsChecked = false);
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var nameViewModel = AllNames.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (nameViewModel != null)
				{
					nameViewModel.SetIsChecked(true);
					nameViewModel.ExpandToThis();
				}
			}
			foreach (var description in filter.JournalEventDescriptionTypes)
			{
				var nameViewModel = AllNames.FirstOrDefault(x => x.JournalEventDescriptionType == description);
				if (nameViewModel != null)
				{
					nameViewModel.SetIsChecked(true);
					nameViewModel.ExpandToThis();
				}
			}
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var subsystemViewModel = RootNames.FirstOrDefault(x => x.JournalSubsystemType == journalSubsystemTypes);
				if (subsystemViewModel != null)
				{
					subsystemViewModel.IsChecked = true;

				}
			}
		}

		public JournalFilter GetModel()
		{
			var filter = new JournalFilter();
			foreach (var eventViewModel in RootNames.SelectMany(x => x.Children))
			{
				if (eventViewModel.IsChecked)
					filter.JournalEventNameTypes.Add(eventViewModel.JournalEventNameType);
				var descriptions = new List<JournalEventDescriptionType>(eventViewModel.Children.Where(x => x.IsChecked).Select(x => x.JournalEventDescriptionType));
				filter.JournalEventDescriptionTypes.AddRange(descriptions);
			}
			return filter;
		}

		public List<NameViewModel> AllNames;

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