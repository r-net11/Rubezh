using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class CodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKCodeReaderSettings CodeReaderSettings { get; private set; }
		public CodeReaderSettingsViewModel AlarmSettingsViewModel { get; private set; }
		public CodeReaderSettingsViewModel SetGuardSettingsViewModel { get; private set; }
		public CodeReaderSettingsViewModel ResetGuardSettingsViewModel { get; private set; }

		public CodeReaderDetailsViewModel(GKCodeReaderSettings codeReaderSettings)
		{
			Title = "Настройка кодонаборника";
			CodeReaderSettings = codeReaderSettings;
			AlarmSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.AlarmSettings);
			SetGuardSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.SetGuardSettings);
			ResetGuardSettingsViewModel = new CodeReaderSettingsViewModel(codeReaderSettings.ResetGuardSettings);
		}

		protected override bool Save()
		{
			CodeReaderSettings.AlarmSettings = AlarmSettingsViewModel.GetCodeReaderSettingsPart();
			CodeReaderSettings.SetGuardSettings = SetGuardSettingsViewModel.GetCodeReaderSettingsPart();
			CodeReaderSettings.ResetGuardSettings = ResetGuardSettingsViewModel.GetCodeReaderSettingsPart();
			return base.Save();
		}
	}
}