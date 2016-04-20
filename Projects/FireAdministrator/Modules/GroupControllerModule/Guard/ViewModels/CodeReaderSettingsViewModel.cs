using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhClient;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class CodeReaderSettingsViewModel : BaseViewModel
	{
		GKCodeReaderSettingsPart CodeReaderSettingsPart { get; set; }
		public CodeReaderSettingsViewModel(GKCodeReaderSettingsPart codeReaderSettingsPart)
		{
			SelectCodeCommand = new RelayCommand(OnSelectCode);
			CodeReaderSettingsPart = codeReaderSettingsPart;
			EnterTypes = Enum.GetValues(typeof(GKCodeReaderEnterType)).Cast<GKCodeReaderEnterType>().ToList();
			SelectedEnterType = EnterTypes.FirstOrDefault(x => x == codeReaderSettingsPart.CodeReaderEnterType);
			AccessLevel = CodeReaderSettingsPart.AccessLevel;
			Codes = new List<GKCode>();
			foreach (var codeUID in codeReaderSettingsPart.CodeUIDs)
			{
				var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
				if (code != null)
				{
					Codes.Add(code);
				}
			}
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

		int _accessLevel;
		public int AccessLevel
		{
			get { return _accessLevel; }
			set
			{
				_accessLevel = value;
				OnPropertyChanged(() => AccessLevel);
			}
		}

		public List<GKCode> Codes { get; private set; }

		public RelayCommand SelectCodeCommand { get; private set; }
		void OnSelectCode()
		{
			var codesSelectationViewModel = new CodesSelectationViewModel(Codes);
			if (DialogService.ShowModalWindow(codesSelectationViewModel))
			{
				Codes = codesSelectationViewModel.Codes;
				CodeReaderSettingsPart.CodeUIDs = Codes.Select(x => x.UID).ToList();
			}
			OnPropertyChanged(() => PresentationCode);
		}

		public string PresentationCode
		{
			get
			{
				if (Codes != null && Codes.Count > 0)
				{
					return GKManager.GetCommaSeparatedObjects(new List<ModelBase>(Codes), new List<ModelBase>(GKManager.DeviceConfiguration.Codes));
				}
				return "Нажмите для выбора кодов";
			}
		}

		public GKCodeReaderSettingsPart GetCodeReaderSettingsPart()
		{
			var codeReaderSettingsPart = new GKCodeReaderSettingsPart();
			codeReaderSettingsPart.CodeReaderEnterType = SelectedEnterType;
			codeReaderSettingsPart.AccessLevel = AccessLevel;
			codeReaderSettingsPart.CodeUIDs = (from code in Codes select code.UID).ToList();
			return codeReaderSettingsPart;
		}
	}
}