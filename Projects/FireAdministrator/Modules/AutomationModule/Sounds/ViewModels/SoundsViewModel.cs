using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using RubezhClient;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace AutomationModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public SoundsViewModel()
		{
			PlaySoundCommand = new RelayCommand(OnPlaySound, CanPlaySound);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			Menu = new SoundsMenuViewModel(this);

			RegisterShortcuts();
		}

		
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.P, ModifierKeys.Control), PlaySoundCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}
		public void Initialize()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds == null)
				ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			SelectedSound = Sounds.FirstOrDefault();
		}

		ObservableCollection<SoundViewModel> _sounds;
		public ObservableCollection<SoundViewModel> Sounds
		{
			get { return _sounds; }
			set
			{
				_sounds = value;
				OnPropertyChanged(() => Sounds);
			}
		}

		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				OnPropertyChanged(() => SelectedSound);
			}
		}

		bool _isNowPlaying;
		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			set
			{
				_isNowPlaying = value;
				OnPropertyChanged(() => IsNowPlaying);
			}
		}

		private bool CanPlaySound(object obj)
		{
			return SelectedSound != null;
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsNowPlaying == false)
			{
				AlarmPlayerHelper.Play(RubezhClient.FileHelper.GetSoundFilePath(Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, SelectedSound.Sound.Uid.ToString())), BeeperType.None, false);
				IsNowPlaying = false;
			}
			else
			{
				AlarmPlayerHelper.Stop();
				IsNowPlaying = false;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Звуковой файл |*.wav",
				DefaultExt = "Звуковой файл |*.wav"
			};
			if (openFileDialog.ShowDialog().Value)
			{
				var sound = new AutomationSound();
				using (new WaitWrapper())
				{
					sound.Name = Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName);
					sound.Uid = ServiceFactoryBase.ContentService.AddContent(openFileDialog.FileName);
				}
				ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Add(sound);
				ServiceFactory.SaveService.AutomationChanged = true;
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
				SelectedSound = soundViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Sounds.IndexOf(SelectedSound);
			ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Remove(SelectedSound.Sound);
			Sounds.Remove(SelectedSound);
			index = Math.Min(index, Sounds.Count - 1);
			if (index > -1)
				SelectedSound = Sounds[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var soundDetailsViewModel = new SoundDetailsViewModel(SelectedSound.Sound);
			if (DialogService.ShowModalWindow(soundDetailsViewModel))
			{
				SelectedSound.Sound = soundDetailsViewModel.Sound;
				SelectedSound.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedSound != null;
		}

		public void Select(Guid soundUid)
		{
			if (soundUid != Guid.Empty)
			{
				SelectedSound = Sounds.FirstOrDefault(item => item.Sound.Uid == soundUid);
			}
		}
	}
}