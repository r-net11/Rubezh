using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using Common;
using FiresecAPI;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Unsubscribe(OnDeviceStateChanged);
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);

			PlaySoundCommand = new RelayCommand(OnPlaySound);
			CurrentStateType = StateType.No;
			IsSoundOn = true;
			IsEnabled = false;
			OnDeviceStateChanged(Guid.Empty);
		}

		public StateType CurrentStateType { get; private set; }

		bool _isSoundOn;
		public bool IsSoundOn
		{
			get { return _isSoundOn; }
			set
			{
				_isSoundOn = value;
				OnPropertyChanged("IsSoundOn");
			}
		}
		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}

		List<Sound> Sounds
		{
			get { return FiresecClient.FiresecManager.SystemConfiguration.Sounds; }
		}

		public void OnDeviceStateChanged(Guid deviceUID)
		{
			var minState = StateType.No;

			foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
				if (deviceState.StateType < minState)
					minState = deviceState.StateType;

			if (CurrentStateType != minState)
				CurrentStateType = minState;

			IsSoundOn = true;
			if (minState == StateType.Norm)
				IsEnabled = false;
			else
				IsEnabled = true;
			StopPlayAlarm();
			PlayAlarm();
		}

		public void PlayAlarm()
		{
			if (Sounds.IsNotNullOrEmpty() == false)
			{
				IsSoundOn = false;
				return;
			}
			foreach (var sound in Sounds)
			{
				if (sound.StateType == CurrentStateType)
				{
					AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(sound.SoundName), sound.BeeperType, sound.IsContinious);
					return;
				}
			}
		}

		public void StopPlayAlarm()
		{
			AlarmPlayerHelper.Stop();
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsSoundOn)
			{
				StopPlayAlarm();
				IsSoundOn = false;
			}
			else
			{
				PlayAlarm();
				IsSoundOn = true;
			}
		}
	}
}
