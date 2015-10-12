using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseStepViewModel
	{
		public SoundArguments SoundArguments { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public SoundStepViewModel(StepViewModel stepViewModel): base(stepViewModel)
		{
			SoundArguments = stepViewModel.Step.SoundArguments;
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public override string Description
		{
			get 
			{ 
				return "Звук: " + (SelectedSound != null ? SelectedSound.Name : "<пусто>") ; 
			}
		}

		bool _isServerContext;
		public bool IsServerContext
		{
			get { return _isServerContext; }
			set
			{
				_isServerContext = value;
				OnPropertyChanged(() => IsServerContext);
			}
		}

		public override void UpdateContent()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			foreach (var sound in ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundArguments.SoundUid))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundArguments.SoundUid);
			else
				SelectedSound = null;

			IsServerContext = Procedure.ContextType == ContextType.Server;
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