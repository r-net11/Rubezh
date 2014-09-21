using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
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
		}

		void Initialize(JournalFilter filter)
		{
			AllNames.ForEach(x => x.IsChecked = false);
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var nameViewModel = AllNames.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (nameViewModel != null)
				{
					nameViewModel.IsChecked = true;
				}
			}
			foreach (var journalEventDescriptionType in filter.JournalEventDescriptionTypes)
			{
				var descriptionViewModel = AllNames.FirstOrDefault(x => x.JournalEventDescriptionType == journalEventDescriptionType);
				if (descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
					descriptionViewModel.Parent.IsExpanded = true;
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
						else
						{
							foreach (var descriptionViewModel in nameViewModel.Children)
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