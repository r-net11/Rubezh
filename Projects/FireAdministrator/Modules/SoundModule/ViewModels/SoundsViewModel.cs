using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.ViewModels;

namespace SoundsModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel
	{
		public SoundsViewModel()
		{
			Menu = new SoundsMenuViewModel(this);
			PlaySoundCommand = new RelayCommand(OnPlaySound, CanPlaySound);
		}

		public void Initialize()
		{
			IsNowPlaying = false;

			Sounds = new ObservableCollection<SoundViewModel>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				if (stateType == StateType.No)
				{
					continue;
				}
				var newSound = new Sound() { StateType = stateType };

				if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.IsNotNullOrEmpty())
				{
					var sound = FiresecClient.FiresecManager.SystemConfiguration.Sounds.FirstOrDefault(x => x.StateType == stateType);
					if (sound == null)
						FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
					else
						newSound = sound;
				}
				else
				{
					FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
				}
				Sounds.Add(new SoundViewModel(newSound));
			}
			if (Sounds.IsNotNullOrEmpty())
				SelectedSound = Sounds[0];
		}

		ObservableCollection<SoundViewModel> _sounds;
		public ObservableCollection<SoundViewModel> Sounds
		{
			get { return _sounds; }
			set
			{
				_sounds = value;
				OnPropertyChanged("Sounds");
			}
		}

		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				OnPropertyChanged("SelectedSound");
			}
		}

		bool _isNowPlaying;
		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			set
			{
				_isNowPlaying = value;
				OnPropertyChanged("IsNowPlaying");
			}
		}

		bool CanPlaySound()
		{
			return ((IsNowPlaying) || (SelectedSound != null &&
					((string.IsNullOrEmpty(SelectedSound.SoundName) == false) ||
					SelectedSound.BeeperType != BeeperType.None)));
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsNowPlaying == false)
			{
				AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(SelectedSound.SoundName), SelectedSound.BeeperType, SelectedSound.IsContinious);
				IsNowPlaying = SelectedSound.IsContinious;
			}
			else
			{
				AlarmPlayerHelper.Stop();
				IsNowPlaying = false;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
		}

		public override void OnHide()
		{
			base.OnHide();
			IsNowPlaying = false;
			AlarmPlayerHelper.Stop();
		}
	}
}