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
		public CodeReaderSettingsViewModel(XCodeReaderSettingsPart codeReaderSettingsPart)
		{
			SelectCodeCommand = new RelayCommand(OnSelectCode);
			EnterTypes = Enum.GetValues(typeof(XCodeReaderEnterType)).Cast<XCodeReaderEnterType>().ToList();
			SelectedEnterType = EnterTypes.FirstOrDefault(x => x == codeReaderSettingsPart.CodeReaderEnterType);
			Code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeReaderSettingsPart.CodeUID);
		}

		public List<XCodeReaderEnterType> EnterTypes { get; private set; }

		XCodeReaderEnterType _selectedEnterType;
		public XCodeReaderEnterType SelectedEnterType
		{
			get { return _selectedEnterType; }
			set
			{
				_selectedEnterType = value;
				OnPropertyChanged(() => SelectedEnterType);
			}
		}

		public XCode Code { get; private set; }

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

		public XCodeReaderSettingsPart GetCodeReaderSettingsPart()
		{
			var codeReaderSettingsPart = new XCodeReaderSettingsPart();
			codeReaderSettingsPart.CodeReaderEnterType = SelectedEnterType;
			codeReaderSettingsPart.CodeUID = Code != null ? Code.UID : Guid.Empty;
			return codeReaderSettingsPart;
		}
	}
}