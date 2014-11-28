using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class CodeReaderSettingsViewModel : BaseViewModel
	{
		public CodeReaderSettingsViewModel(GKCodeReaderSettingsPart codeReaderSettingsPart)
		{
			SelectCodeCommand = new RelayCommand(OnSelectCode);
			EnterTypes = Enum.GetValues(typeof(GKCodeReaderEnterType)).Cast<GKCodeReaderEnterType>().ToList();
			SelectedEnterType = EnterTypes.FirstOrDefault(x => x == codeReaderSettingsPart.CodeReaderEnterType);
			Code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeReaderSettingsPart.CodeUID);
		}

		public List<GKCodeReaderEnterType> EnterTypes { get; private set; }

		GKCodeReaderEnterType _selectedEnterType;
		public GKCodeReaderEnterType SelectedEnterType
		{
			get { return _selectedEnterType; }
			set
			{
				_selectedEnterType = value;
				OnPropertyChanged(() => SelectedEnterType);
			}
		}

		public GKCode Code { get; private set; }

		public RelayCommand SelectCodeCommand { get; private set; }
		void OnSelectCode()
		{
			var codeSelectionViewModel = new CodeSelectionViewModel(Code);
			if (DialogService.ShowModalWindow(codeSelectionViewModel))
			{
				Code = codeSelectionViewModel.SelectedCode;
			}
			OnPropertyChanged(() => PresentationCode);
		}

		public string PresentationCode
		{
			get
			{
				if (Code != null)
				{
					return Code.PresentationName;
				}
				return "Нажмите для выбора кода";
			}
		}

		public GKCodeReaderSettingsPart GetCodeReaderSettingsPart()
		{
			var codeReaderSettingsPart = new GKCodeReaderSettingsPart();
			codeReaderSettingsPart.CodeReaderEnterType = SelectedEnterType;
			codeReaderSettingsPart.CodeUID = Code != null ? Code.UID : Guid.Empty;
			return codeReaderSettingsPart;
		}
	}
}