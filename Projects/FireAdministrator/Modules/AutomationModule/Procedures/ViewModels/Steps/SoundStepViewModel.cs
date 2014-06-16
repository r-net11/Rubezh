using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseViewModel
	{
		public ObservableCollection<SoundViewModel> Sounds;

		public SoundStepViewModel(ProcedureStep procedureStep)
		{
			Initialize();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds.Any(x => x.Uid == procedureStep.UID))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == procedureStep.UID);
		}

		public void Initialize()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			SelectedSound = Sounds.FirstOrDefault();
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
	}
}