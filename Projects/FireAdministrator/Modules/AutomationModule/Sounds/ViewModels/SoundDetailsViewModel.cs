using Localization.Automation.Common;
using StrazhAPI.Automation;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundDetailsViewModel : SaveCancelDialogViewModel
	{
		public AutomationSound Sound { get; private set; }

		public SoundDetailsViewModel(AutomationSound sound)
		{
            Title = CommonViewModels.SoundDetailsViewModel_Title;
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
				MessageBoxService.ShowWarning(CommonResources.SaveEmpty);
				return false;
			}
			Sound.Name = Name;
			return base.Save();
		}
	}
}