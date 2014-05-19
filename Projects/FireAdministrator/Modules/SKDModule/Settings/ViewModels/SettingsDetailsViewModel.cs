using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SettingsDetailsViewModel : SaveCancelDialogViewModel
	{
		public SettingsDetailsViewModel()
		{
			Title = "Настройка";
		}

		protected override bool Save()
		{
			return true;
		}
	}
}