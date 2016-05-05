using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Localization.Automation.ViewModels;
using Microsoft.Win32;

namespace AutomationModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static SoundsViewModel Current { get; private set; }
		public SoundsViewModel()
		{
			Current = this;
			Menu = new SoundsMenuViewModel(this);
			PlaySoundCommand = new RelayCommand(OnPlaySound);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
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

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsNowPlaying == false)
			{
				AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath
					(Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, SelectedSound.Sound.Uid.ToString())), BeeperType.Alarm, false);
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
                Filter = CommonViewModel.SoundsViewModel_Extention,
                DefaultExt = CommonViewModel.SoundsViewModel_Extention
			};
			if (openFileDialog.ShowDialog().Value)
			{
				var sound = new AutomationSound();
				sound.Name = Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName);
				sound.Uid = ServiceFactoryBase.ContentService.AddContent(openFileDialog.FileName);
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Add(sound);
				ServiceFactory.SaveService.AutomationChanged = true;
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
				SelectedSound = soundViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Sounds.IndexOf(SelectedSound);
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Remove(SelectedSound.Sound);
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

		public override void OnShow()
		{
			base.OnShow();
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}