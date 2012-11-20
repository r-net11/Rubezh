using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);

			PlaySoundCommand = new RelayCommand(OnPlaySound);
			CurrentStateType = StateType.Norm;
			IsSoundOn = true;
			IsEnabled = false;
			OnDevicesStateChanged(Guid.Empty);
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

		public void OnDevicesStateChanged(object obj)
		{
			var minState = StateType.Norm;

			foreach (var device in FiresecManager.Devices)
				if (device.DeviceState.StateType < minState)
					minState = device.DeviceState.StateType;

			if (CurrentStateType != minState)
				CurrentStateType = minState;

			IsSoundOn = true;
			if (minState == StateType.Norm)
				IsEnabled = false;
			else
				IsEnabled = true;
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
				AlarmPlayerHelper.Stop();
			}
		}

		public RelayCommand PlaySoundCommand { get; private set; }
		void OnPlaySound()
		{
			if (IsSoundOn)
			{
				AlarmPlayerHelper.Stop();
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
