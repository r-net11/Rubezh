using RubezhAPI.Automation;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class SoundStepViewModel : BaseStepViewModel
	{
		public SoundStep SoundStep { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public SoundStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			SoundStep = (SoundStep)stepViewModel.Step;
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public override string Description
		{
			get
			{
				return "Звук: " + (SelectedSound != null ? SelectedSound.Name : "<пусто>");
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
			if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundStep.SoundUid))
				SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundStep.SoundUid);
			else
				SelectedSound = null;

			IsServerContext = Procedure.ContextType == ContextType.Server;
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(SoundStep.LayoutFilter);
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
					SoundStep.SoundUid = value.Sound.Uid;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				OnPropertyChanged(() => SelectedSound);
			}
		}

		public bool ForAllClients
		{
			get { return SoundStep.ForAllClients; }
			set
			{
				SoundStep.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
	}
}