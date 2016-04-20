using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;

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
			CodeReaderSettingsPart = codeReaderSettings.MPTSettings;
			switch (mptDeviceType)
			{
				case GKMPTDeviceType.HandAutomaticOn:
					CodeName = "Действие для постановки на автоматику";
					break;
				case GKMPTDeviceType.HandAutomaticOff:
					CodeName = "Действие для снятия с автоматики";
					break;
				case GKMPTDeviceType.HandStart:
					CodeName = "Действие для пуска";
					break;
				case GKMPTDeviceType.HandStop:
					CodeName = "Действие для останова";
					break;
			}

			CodeReaderSettingsViewModel = new CodeReaderSettingsViewModel(CodeReaderSettingsPart);
		}

		protected override bool Save()
		{
			CodeReaderSettings.MPTSettings = CodeReaderSettingsViewModel.GetCodeReaderSettingsPart();
			return base.Save();
		}
	}
}