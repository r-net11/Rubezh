using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.IO;
using Microsoft.Windows.Controls;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnDevicesStateChanged);

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
			var minState = (StateType)Math.Min((int)GetMinASStateType(), (int)GetMinGKStateType());

			if (CurrentStateType != minState)
				CurrentStateType = minState;

			IsSoundOn = true;
			if (minState == StateType.Norm)
				IsEnabled = false;
			else
				IsEnabled = true;
			PlayAlarm();
		}

		StateType GetMinASStateType()
		{
			var minStateType = StateType.Norm;
			foreach (var device in FiresecManager.Devices)
				if (device.DeviceState.StateType < minStateType)
					minStateType = device.DeviceState.StateType;
			return minStateType;
		}

		StateType GetMinGKStateType()
		{
			var minStateType = StateType.Norm;
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.DeviceState != null)
				{
					var stateType = device.DeviceState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState != null)
				{
					var stateType = zone.ZoneState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (direction.DirectionState != null)
				{
					var stateType = direction.DirectionState.GetStateType();
					if (stateType < minStateType)
						minStateType = stateType;
				}
			}
			return minStateType;
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