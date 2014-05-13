using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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