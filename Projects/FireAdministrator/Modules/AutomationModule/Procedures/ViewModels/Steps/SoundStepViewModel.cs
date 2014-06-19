using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseViewModel
	{
		public ObservableCollection<SoundViewModel> Sounds { get; private set; }
		public ProcedureStep ProcedureStep { get; private set; }
		public SoundArguments SoundArguments { get; private set; }
		public SoundStepViewModel(ProcedureStep procedureStep)
		{
			ProcedureStep = procedureStep;
			SoundArguments = procedureStep.SoundArguments;
			UpdateContent();
		}

		public void UpdateContent()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Sounds = new ObservableCollection<SoundViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundArguments.SoundUid))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundArguments.SoundUid);
			else
			    SelectedSound = Sounds.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			OnPropertyChanged(() => Sounds);
		}

		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				if (value != null)
					SoundArguments.SoundUid = value.Sound.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedSound);
			}
		}
	}
}