using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class ControlVisualStepViewModel : BaseStepViewModel
	{
		public ControlVisualArguments ControlVisualArguments { get; private set; }

		public ControlVisualStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlVisualArguments = stepViewModel.Step.ControlVisualArguments;
			Layouts = new ObservableCollection<LayoutViewModel>();
			foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
			{
				var layoutViewModel = new LayoutViewModel(layout);
				Layouts.Add(layoutViewModel);
			}
			SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ControlVisualArguments.LayoutUid);
		}


		public ObservableCollection<LayoutViewModel> Layouts { get; private set; }
		LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				_selectedLayout = value;
				if (_selectedLayout != null)
					ControlVisualArguments.LayoutUid = _selectedLayout.Layout.UID;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override string Description
		{
			get { return ""; }
		}

		public override void UpdateContent()
		{
			//Sounds = new ObservableCollection<SoundViewModel>();
			//foreach (var sound in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds)
			//{
			//    var soundViewModel = new SoundViewModel(sound);
			//    Sounds.Add(soundViewModel);
			//}
			//if (FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.Any(x => x.Uid == SoundArguments.SoundUid))
			//    SelectedSound = Sounds.FirstOrDefault(x => x.Sound.Uid == SoundArguments.SoundUid);
			//else
			//    SelectedSound = null;

			//ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(SoundArguments.ProcedureLayoutCollection);
			//OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
			//OnPropertyChanged(() => Sounds);
		}
	}
}