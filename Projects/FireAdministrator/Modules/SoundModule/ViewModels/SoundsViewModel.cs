using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;

namespace SoundsModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel
	{
		public SoundsViewModel()
		{
			Menu = new SoundsMenuViewModel(this);
			PlaySoundCommand = new RelayCommand(OnPlaySound, CanPlaySound);
			SetRibbonItems();
		}

		public void Initialize()
		{
			IsNowPlaying = false;

			Sounds = new ObservableCollection<SoundViewModel>();
			var stateClasses = new List<XStateClass>();
			stateClasses.Add(XStateClass.Attention);
			stateClasses.Add(XStateClass.Fire1);
			stateClasses.Add(XStateClass.Fire2);
			stateClasses.Add(XStateClass.AutoOff);
			stateClasses.Add(XStateClass.ConnectionLost);
			stateClasses.Add(XStateClass.Failure);
			stateClasses.Add(XStateClass.Ignore);
			stateClasses.Add(XStateClass.Off);
			stateClasses.Add(XStateClass.On);
			stateClasses.Add(XStateClass.TurningOff);
			stateClasses.Add(XStateClass.TurningOn);
			foreach (var stateClass in stateClasses)
			{
				var newSound = new Sound() { StateClass = stateClass };

				var sound = FiresecClient.FiresecManager.SystemConfiguration.Sounds.FirstOrDefault(x => x.StateClass == stateClass);
				if (sound == null)
					FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
				else
					newSound = sound;

				Sounds.Add(new SoundViewModel(newSound));
			}
			SelectedSound = Sounds.FirstOrDefault();

			if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.RemoveAll(x => !Sounds.Any(y => y.StateClass == x.StateClass)) > 0)
				ServiceFactory.SaveService.SoundsChanged = true;
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
				UpdateRibbonItems();
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

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0].IsVisible = !IsNowPlaying;
			RibbonItems[1].IsVisible = IsNowPlaying;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Проверить звук", PlaySoundCommand, "/Controls;component/Images/BPlay.png") { Order = 2 },
				new RibbonMenuItemViewModel("Остановить", PlaySoundCommand, "/Controls;component/Images/BStop.png") { Order = 3 }
			};
		}
	}
}