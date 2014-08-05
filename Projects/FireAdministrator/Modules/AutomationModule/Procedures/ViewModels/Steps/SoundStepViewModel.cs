using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseViewModel, IStepViewModel
	{
		public SoundArguments SoundArguments { get; private set; }
		public ObservableCollection<SoundLayoutViewModel> Layouts { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public SoundStepViewModel(SoundArguments soundArguments, Action updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			SoundArguments = soundArguments;
			UpdateContent();
		}

		public string Description
		{
			get
			{
				return SelectedSound == null ? "нет" : SelectedSound.Name;
			}
		}

		public void UpdateContent()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			foreach (var sound in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundArguments.SoundUid))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundArguments.SoundUid);
			else
				SelectedSound = null;
			Layouts = new ObservableCollection<SoundLayoutViewModel>();
			foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
			{
				var soundLayoutViewModel = new SoundLayoutViewModel(SoundArguments, layout);
				soundLayoutViewModel.IsChecked = SoundArguments.LayoutsUids.Contains(layout.UID);
				Layouts.Add(soundLayoutViewModel);
			}

			OnPropertyChanged(() => Layouts);
			OnPropertyChanged(() => Sounds);
		}

		public ObservableCollection<SoundViewModel> Sounds { get; private set; }
		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				if (value != null)
					SoundArguments.SoundUid = value.Sound.Uid;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedSound);
			}
		}
	}
}