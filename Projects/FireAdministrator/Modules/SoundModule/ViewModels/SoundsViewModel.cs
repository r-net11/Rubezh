using Common;
using StrazhAPI.GK;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Linq;

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

			Sounds = new SortableObservableCollection<SoundViewModel>();
			var stateClasses = new List<XStateClass>();

			stateClasses.Add(XStateClass.Attention);
			stateClasses.Add(XStateClass.ConnectionLost);
			stateClasses.Add(XStateClass.Off);
			stateClasses.Add(XStateClass.On);

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

			Sounds.Sort(x => EnumHelper.GetEnumDescription(x.StateClass));
			SelectedSound = Sounds.FirstOrDefault();

			if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.RemoveAll(x => !Sounds.Any(y => y.StateClass == x.StateClass)) > 0)
				ServiceFactory.SaveService.SoundsChanged = true;
		}

		SortableObservableCollection<SoundViewModel> _sounds;
		public SortableObservableCollection<SoundViewModel> Sounds
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
			if (Sounds != null)
				Sounds.Sort(x => x.SoundName);
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
				new RibbonMenuItemViewModel("Проверить звук", PlaySoundCommand, "BPlay") { Order = 2 },
				new RibbonMenuItemViewModel("Остановить", PlaySoundCommand, "BStop") { Order = 3 }
			};
		}
	}
}