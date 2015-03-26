using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class MPTCodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKCodeReaderSettingsPart CodeReaderSettingsPart { get; private set; }
		public CodeReaderSettingsViewModel CodeReaderSettingsViewModel { get; private set; }
		public string CodeName { get; private set; }
		public GKMPTDeviceType MPTDeviceType { get; private set; }
		public GKCodeReaderSettings CodeReaderSettings { get; private set; }

		public MPTCodeReaderDetailsViewModel(GKCodeReaderSettings codeReaderSettings, GKMPTDeviceType mptDeviceType)
		{
			Title = "Настройка кодонаборника";
			MPTDeviceType = mptDeviceType;
			CodeReaderSettings = codeReaderSettings;
			switch (mptDeviceType)
			{
				case GKMPTDeviceType.HandAutomaticOn:
					CodeReaderSettingsPart = codeReaderSettings.AutomaticOnSettings;
					CodeName = "Действие для постановки на автоматизацию";
					break;
				case GKMPTDeviceType.HandAutomaticOff:
					CodeReaderSettingsPart = codeReaderSettings.AutomaticOffSettings;
					CodeName = "Действие для снятия с автоматизации";
					break;
				case GKMPTDeviceType.HandStart:
					CodeReaderSettingsPart = codeReaderSettings.StartSettings;
					CodeName = "Действие для пуска";
					break;
				case GKMPTDeviceType.HandStop:
					CodeReaderSettingsPart = codeReaderSettings.StopSettings;
					CodeName = "Действие для останова";
					break;
			}

			CodeReaderSettingsViewModel = new CodeReaderSettingsViewModel(CodeReaderSettingsPart);
		}

		protected override bool Save()
		{
			switch (MPTDeviceType)
			{
				case GKMPTDeviceType.HandAutomaticOn:
					CodeReaderSettings.AutomaticOnSettings = CodeReaderSettingsViewModel.GetCodeReaderSettingsPart();
					break;
				case GKMPTDeviceType.HandAutomaticOff:
					CodeReaderSettings.AutomaticOffSettings = CodeReaderSettingsViewModel.GetCodeReaderSettingsPart();
					break;
				case GKMPTDeviceType.HandStart:
					CodeReaderSettings.StartSettings = CodeReaderSettingsViewModel.GetCodeReaderSettingsPart();
					break;
				case GKMPTDeviceType.HandStop:
					CodeReaderSettings.StopSettings = CodeReaderSettingsViewModel.GetCodeReaderSettingsPart();
					break;
			}
			return base.Save();
		}
	}
}