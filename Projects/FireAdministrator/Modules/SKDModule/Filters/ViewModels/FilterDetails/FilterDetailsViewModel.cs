using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class FilterDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDJournalFilter JournalFilter { get; set; }

		public FilterDetailsViewModel(SKDJournalFilter journalFilter = null)
		{
			if (journalFilter == null)
			{
				Title = "Создание нового фильтра";

				JournalFilter = new SKDJournalFilter()
				{
					Name = "Новый фильтр",
					Description = "Описание фильтра"
				};
			}
			else
			{
				Title = string.Format("Свойства фильтра: {0}", journalFilter.Name);
				JournalFilter = journalFilter;
			}

			EventNames = new ObservableCollection<EventFilterViewModel>();
			foreach (var skdEvent in SKDEventsHelper.SKDEvents.Where(x => x.DriverType == SKDDriverType.Controller || x.DriverType == SKDDriverType.Reader))
			{
				var eventFilterViewModel = new EventFilterViewModel(skdEvent.Name, skdEvent.StateClass);
				EventNames.Add(eventFilterViewModel);
			}

			Devices = new ObservableCollection<DeviceFilterViewModel>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller || device.DriverType == SKDDriverType.Reader)
				{
					var deviceFilterViewModel = new DeviceFilterViewModel(device);
					Devices.Add(deviceFilterViewModel);
				}
			}

			CopyProperties();
		}

		void CopyProperties()
		{
			Name = JournalFilter.Name;
			Description = JournalFilter.Description;

			foreach (var eventName in JournalFilter.EventNames)
			{
				var eventFilterViewModel = EventNames.FirstOrDefault(x => x.EventNameEnum == eventName);
				if (eventFilterViewModel != null)
				{
					eventFilterViewModel.IsChecked = true;
				}
			}

			foreach (var deviceUID in JournalFilter.DeviceUIDs)
			{
				var deviceViewModel = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
				{
					deviceViewModel.IsChecked = true;
				}
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		int _lastRecordsCount;
		public int LastRecordsCount
		{
			get { return _lastRecordsCount; }
			set
			{
				_lastRecordsCount = value;
				OnPropertyChanged("LastRecordsCount");
			}
		}

		public ObservableCollection<DeviceFilterViewModel> Devices { get; private set; }
		public ObservableCollection<EventFilterViewModel> EventNames { get; private set; }

		protected override bool Save()
		{
			JournalFilter.Name = Name;
			JournalFilter.Description = Description;
			JournalFilter.EventNames = EventNames.Where(x => x.IsChecked).Select(x => x.EventNameEnum).ToList();
			JournalFilter.DeviceUIDs = Devices.Where(x => x.IsChecked).Select(x => x.Device.UID).ToList();
			return base.Save();
		}

		protected override bool CanSave()
		{
			return EventNames.Where(x => x.IsChecked == true).ToList().Count > 0;
		}
	}
}