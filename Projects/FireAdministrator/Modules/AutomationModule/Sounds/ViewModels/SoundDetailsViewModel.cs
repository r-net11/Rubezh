using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class SoundDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSound Sound { get; private set; }

		public SoundDetailsViewModel(AutomationSound sound)
		{
			Title = Resources.Language.Sounds.ViewModels.SoundDetailsViewModel.Title;
			Sound = sound;
			Name = Sound.Name;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
                MessageBoxService.ShowWarning(Resources.Language.Sounds.ViewModels.SoundDetailsViewModel.EmptyName);
				return false;
			}
			Sound.Name = Name;
			return base.Save();
		}
	}
}