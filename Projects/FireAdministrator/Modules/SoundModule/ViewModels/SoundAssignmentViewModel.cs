using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace SoundsModule.ViewModels
{
	public class SoundAssignmentViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";

		public Sound Sound { get; private set; }

		public SoundAssignmentViewModel(Sound sound, ObservableCollection<SoundFileViewModel> soundFileViewModels)
		{
			Sound = sound;
			AvailableSoundFileViewModels = soundFileViewModels;
			SelectedSoundFileViewModel = AvailableSoundFileViewModels.FirstOrDefault(x => x.Name == SoundName);
			AvailableSoundFileViewModels.CollectionChanged += AvailableSoundFileViewModels_CollectionChanged;
		}

		private void AvailableSoundFileViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
				SelectedSoundFileViewModel = AvailableSoundFileViewModels.FirstOrDefault(x => x.Name == SoundName);
		}

		public JournalEventNameType JournalEventNameType
		{
			get { return Sound.JournalEventNameType; }
		}

		public string SoundName
		{
			get { return Sound.SoundName; }
			set
			{
				if (Sound.SoundName == value)
					return;
				Sound.SoundName = value;
				OnPropertyChanged(() => SoundName);
				ServiceFactory.SaveService.SoundsChanged = true;
			}
		}

		public bool IsContinious
		{
			get { return Sound.IsContinious; }
			set
			{
				if (Sound.IsContinious == value)
					return;
				Sound.IsContinious = value;
				OnPropertyChanged(() => IsContinious);
				ServiceFactory.SaveService.SoundsChanged = true;
			}
		}

		public ObservableCollection<SoundFileViewModel> AvailableSoundFileViewModels { get; private set; }

		private SoundFileViewModel _selectedSoundFileViewModel;
		public SoundFileViewModel SelectedSoundFileViewModel
		{
			get { return _selectedSoundFileViewModel; }
			set
			{
				_selectedSoundFileViewModel = value;
				OnPropertyChanged(() => SelectedSoundFileViewModel);
				SoundName = _selectedSoundFileViewModel == null ? null : _selectedSoundFileViewModel.Name;
			}
		}
	}
}