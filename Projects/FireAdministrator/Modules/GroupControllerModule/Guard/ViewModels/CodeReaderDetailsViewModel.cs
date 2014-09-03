using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class CodeReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public XCodeReaderSettings CodeReaderSettings { get; private set; }

		public CodeReaderDetailsViewModel(XCodeReaderSettings codeReaderSettings)
		{
			Title = "Настройка кодонаборника";
			CodeReaderSettings = codeReaderSettings;

			AttentionActionTypes = new ObservableCollection<XGuardZoneDeviceActionType>();
			AttentionActionTypes.Add(XGuardZoneDeviceActionType.SetGuard);
			AttentionActionTypes.Add(XGuardZoneDeviceActionType.ResetGuard);
			AttentionActionTypes.Add(XGuardZoneDeviceActionType.SetAlarm);
			SelectedAttentionActionType = AttentionActionTypes.FirstOrDefault(x => x == codeReaderSettings.AttentionSettings.GuardZoneDeviceActionType);

			Fire1ActionTypes = new ObservableCollection<XGuardZoneDeviceActionType>();
			Fire1ActionTypes.Add(XGuardZoneDeviceActionType.SetGuard);
			Fire1ActionTypes.Add(XGuardZoneDeviceActionType.ResetGuard);
			Fire1ActionTypes.Add(XGuardZoneDeviceActionType.SetAlarm);
			SelectedFire1ActionType = Fire1ActionTypes.FirstOrDefault(x => x == codeReaderSettings.Fire1Settings.GuardZoneDeviceActionType);

			Fire2ActionTypes = new ObservableCollection<XGuardZoneDeviceActionType>();
			Fire2ActionTypes.Add(XGuardZoneDeviceActionType.SetGuard);
			Fire2ActionTypes.Add(XGuardZoneDeviceActionType.ResetGuard);
			Fire2ActionTypes.Add(XGuardZoneDeviceActionType.SetAlarm);
			SelectedFire2ActionType = Fire2ActionTypes.FirstOrDefault(x => x == codeReaderSettings.Fire2Settings.GuardZoneDeviceActionType);
		}

		public ObservableCollection<XGuardZoneDeviceActionType> AttentionActionTypes { get; private set; }

		XGuardZoneDeviceActionType _selectedAttentionActionType;
		public XGuardZoneDeviceActionType SelectedAttentionActionType
		{
			get { return _selectedAttentionActionType; }
			set
			{
				_selectedAttentionActionType = value;
				OnPropertyChanged(() => SelectedAttentionActionType);
			}
		}

		public ObservableCollection<XGuardZoneDeviceActionType> Fire1ActionTypes { get; private set; }

		XGuardZoneDeviceActionType _selectedFire1ActionType;
		public XGuardZoneDeviceActionType SelectedFire1ActionType
		{
			get { return _selectedFire1ActionType; }
			set
			{
				_selectedFire1ActionType = value;
				OnPropertyChanged(() => SelectedFire1ActionType);
			}
		}

		public ObservableCollection<XGuardZoneDeviceActionType> Fire2ActionTypes { get; private set; }

		XGuardZoneDeviceActionType _selectedFire2ActionType;
		public XGuardZoneDeviceActionType SelectedFire2ActionType
		{
			get { return _selectedFire2ActionType; }
			set
			{
				_selectedFire2ActionType = value;
				OnPropertyChanged(() => SelectedFire2ActionType);
			}
		}

		protected override bool Save()
		{
			CodeReaderSettings.AttentionSettings.GuardZoneDeviceActionType = SelectedAttentionActionType;
			CodeReaderSettings.Fire1Settings.GuardZoneDeviceActionType = SelectedFire1ActionType;
			CodeReaderSettings.Fire2Settings.GuardZoneDeviceActionType = SelectedFire2ActionType;
			return base.Save();
		}
	}
}