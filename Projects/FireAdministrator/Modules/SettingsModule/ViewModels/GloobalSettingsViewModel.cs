using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class GloobalSettingsViewModel : SaveCancelDialogViewModel
	{
		public GloobalSettingsViewModel()
		{
			Title = "Настройки";
			GlobalSettingsHelper.Load();
		}

		public GlobalSettings GlobalSettings
		{
			get { return GlobalSettingsHelper.GlobalSettings; }
		}

		protected override bool Cancel()
		{
			GlobalSettingsHelper.Load();
			return false;
		}

		protected override bool Save()
		{
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}