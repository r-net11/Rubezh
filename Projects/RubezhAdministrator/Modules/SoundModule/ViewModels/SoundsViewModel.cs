using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;
using RubezhClient;
using RubezhAPI;
using System;

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

			//ClientManager.SystemConfiguration.Sounds = new List<Sound>();
			Sounds = new SortableObservableCollection<SoundViewModel>();
			var stateClasses = new List<Tuple<XStateClass,SoundType>>();

			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Fire1, SoundType.Fire1));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Fire2, SoundType.Fire2));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Attention, SoundType.Attention));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Fire1, SoundType.Alarm));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Failure, SoundType.Failure));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.Off, SoundType.Off));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.TurningOn, SoundType.TurningOn));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.TurningOff, SoundType.TurningOff));
			stateClasses.Add(new Tuple<XStateClass, SoundType>(XStateClass.AutoOff, SoundType.AutoOff));

			foreach (var stateClass in stateClasses)
			{
				var newSound = new Sound() { StateClass = stateClass.Item1, Type = stateClass.Item2 };

				var sound = ClientManager.SystemConfiguration.Sounds.FirstOrDefault(x => x.Type == stateClass.Item2);
				if (sound == null)
					ClientManager.SystemConfiguration.Sounds.Add(newSound);
				else
					newSound = sound;

				Sounds.Add(new SoundViewModel(newSound));
			}

			Sounds.Sort(x => EnumHelper.GetEnumDescription(x.StateClass));
			SelectedSound = Sounds.FirstOrDefault();

			if (ClientManager.SystemConfiguration.Sounds.RemoveAll(x => !Sounds.Any(y => y.StateClass == x.StateClass)) > 0)
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
					((string.IsNullOrEmpty(SelectedSound.SoundName) == false))));
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsNowPlaying == false)
			{
				AlarmPlayerHelper.Play(RubezhClient.FileHelper.GetSoundFilePath(SelectedSound.SoundName), SelectedSound.IsContinious);
				IsNowPlaying = SelectedSound.IsContinious;
			}
			else
			{
				AlarmPlayerHelper.Stop();
				IsNowPlaying = false;
			}
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