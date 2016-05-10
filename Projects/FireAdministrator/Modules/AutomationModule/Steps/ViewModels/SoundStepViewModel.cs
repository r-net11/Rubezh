using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Localization.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseStepViewModel
	{
		public SoundArguments SoundArguments { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public SoundStepViewModel(StepViewModel stepViewModel): base(stepViewModel)
		{
			SoundArguments = stepViewModel.Step.SoundArguments;
		}

		public override string Description
		{
			get 
			{ 
				return string.Format(StepCommonViewModel.Sound, SelectedSound != null ? SelectedSound.Name : CommonResources.Empty) ; 
			}
		}

		public override void UpdateContent()
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

			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(SoundArguments.LayoutFilter);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
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
				OnPropertyChanged(() => SelectedSound);
			}
		}

		public bool ForAllClients
		{
			get { return SoundArguments.ForAllClients; }
			set
			{
				SoundArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
	}
}