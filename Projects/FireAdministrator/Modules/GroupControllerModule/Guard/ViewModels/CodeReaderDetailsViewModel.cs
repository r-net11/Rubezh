using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace GKModule.ViewModels
{
	public class CodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public XCodeReaderSettings CodeReaderSettings { get; private set; }

		public CodeReaderDetailsViewModel(XCodeReaderSettings codeReaderSettings)
		{
			Title = "Настройка кодонаборника";
			SelectAlarmCodeCommand = new RelayCommand(OnSelectAlarmCode);

			CodeReaderSettings = codeReaderSettings;

			SetAlarmEnterTypes = Enum.GetValues(typeof(XCodeReaderEnterType)).Cast<XCodeReaderEnterType>().ToList();
			SelectedSetAlarmEnterType = SetAlarmEnterTypes.FirstOrDefault(x => x == codeReaderSettings.AlarmSettings.CodeReaderEnterType);

			SetGuardEnterTypes = Enum.GetValues(typeof(XCodeReaderEnterType)).Cast<XCodeReaderEnterType>().ToList();
			SelectedSetGuardEnterType = SetGuardEnterTypes.FirstOrDefault(x => x == codeReaderSettings.SetGuardSettings.CodeReaderEnterType);

			ResetGuardEnterTypes = Enum.GetValues(typeof(XCodeReaderEnterType)).Cast<XCodeReaderEnterType>().ToList();
			SelectedResetGuardEnterType = ResetGuardEnterTypes.FirstOrDefault(x => x == codeReaderSettings.ResetGuardSettings.CodeReaderEnterType);
		}

		public List<XCodeReaderEnterType> SetAlarmEnterTypes { get; private set; }

		XCodeReaderEnterType _selectedSetAlarmEnterType;
		public XCodeReaderEnterType SelectedSetAlarmEnterType
		{
			get { return _selectedSetAlarmEnterType; }
			set
			{
				_selectedSetAlarmEnterType = value;
				OnPropertyChanged(() => SelectedSetAlarmEnterType);
			}
		}

		public List<XCodeReaderEnterType> SetGuardEnterTypes { get; private set; }

		XCodeReaderEnterType _selectedSetGuardEnterType;
		public XCodeReaderEnterType SelectedSetGuardEnterType
		{
			get { return _selectedSetGuardEnterType; }
			set
			{
				_selectedSetGuardEnterType = value;
				OnPropertyChanged(() => SelectedSetGuardEnterType);
			}
		}

		public List<XCodeReaderEnterType> ResetGuardEnterTypes { get; private set; }

		XCodeReaderEnterType _selectedResetGuardEnterType;
		public XCodeReaderEnterType SelectedResetGuardEnterType
		{
			get { return _selectedResetGuardEnterType; }
			set
			{
				_selectedResetGuardEnterType = value;
				OnPropertyChanged(() => SelectedResetGuardEnterType);
			}
		}

		public RelayCommand SelectAlarmCodeCommand { get; private set; }
		void OnSelectAlarmCode()
		{

		}

		protected override bool Save()
		{
			CodeReaderSettings.AlarmSettings.CodeReaderEnterType = SelectedSetAlarmEnterType;
			CodeReaderSettings.SetGuardSettings.CodeReaderEnterType = SelectedSetGuardEnterType;
			CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType = SelectedResetGuardEnterType;
			return base.Save();
		}
	}
}