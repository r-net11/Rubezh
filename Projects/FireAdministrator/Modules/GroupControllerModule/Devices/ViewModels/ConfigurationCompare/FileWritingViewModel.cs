using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class FileWritingViewModel : DialogViewModel
	{
		public	FileWritingViewModel()
		{
			Title = "Запись конфигурационного файла  в ГК";
			YesCommand = new RelayCommand(OnYes);
			NoCommand = new RelayCommand(OnNo);
		}

		public bool IsDisabled
		{
			get
			{
				return GlobalSettingsHelper.GlobalSettings.DoNotShowWriteFileToGKDialog;
			}
			set
			{
				GlobalSettingsHelper.GlobalSettings.DoNotShowWriteFileToGKDialog = value;
				OnPropertyChanged("IsDisabled");
			}
		}

		public RelayCommand YesCommand { get; private set; }
		void OnYes()
		{
			GlobalSettingsHelper.GlobalSettings.WriteFileToGK = true;
			GlobalSettingsHelper.Save();
			Close(true);
		}

		public RelayCommand NoCommand { get; private set; }
		void OnNo()
		{
			GlobalSettingsHelper.GlobalSettings.WriteFileToGK = false;
			GlobalSettingsHelper.Save();
			Close(true);
		}
	}
}
