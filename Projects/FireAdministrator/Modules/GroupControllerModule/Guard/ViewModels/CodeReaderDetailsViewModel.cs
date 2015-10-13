using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class CodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKCodeReaderSettings CodeReaderSettings { get; private set; }
		public CodeReaderSettingsViewModel SetGuardSettingsViewModel { get; private set; }
		public CodeReaderSettingsViewModel ResetGuardSettingsViewModel { get; private set; }
		public CodeReaderSettingsViewModel ChangeGuardSettingsViewModel { get; private set; }
		public CodeReaderSettingsViewModel AlarmSettingsViewModel { get; private set; }

		public CodeReaderDetailsViewModel(GKCodeReaderSettings codeReaderSettings)
		{
			Title = "Настройка кодонаборника";
			CodeReaderSettings = codeReaderSettings;
			SetGuardSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.SetGuardSettings);
			ResetGuardSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.ResetGuardSettings);
			ChangeGuardSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.ChangeGuardSettings);
			AlarmSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.AlarmSettings);
		}

		protected override bool Save()
		{
			CodeReaderSettings.SetGuardSettings = SetGuardSettingsViewModel.GetCodeReaderSettingsPart();
			CodeReaderSettings.ResetGuardSettings = ResetGuardSettingsViewModel.GetCodeReaderSettingsPart();
			CodeReaderSettings.ChangeGuardSettings = ChangeGuardSettingsViewModel.GetCodeReaderSettingsPart();
			CodeReaderSettings.AlarmSettings = AlarmSettingsViewModel.GetCodeReaderSettingsPart();
			return base.Save();
		}
	}
}