using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using System.Linq;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnStateChanged);

			PlaySoundCommand = new RelayCommand(OnPlaySound);
			CurrentStateClass = XStateClass.Norm;
			IsSoundOn = true;
			IsEnabled = false;
			OnStateChanged(null);
		}

		public XStateClass CurrentStateClass { get; private set; }

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

		public void OnStateChanged(object obj)
		{
			var minStateClass = XManager.GetMinStateClass();

			if (CurrentStateClass != minStateClass)
				CurrentStateClass = minStateClass;

			IsSoundOn = true;
			if (minStateClass == XStateClass.Norm)
				IsEnabled = false;
			else
				IsEnabled = true;
			PlayAlarm();
		}

		void PlayAlarm()
		{
			if (Sounds.IsNotNullOrEmpty() == false)
			{
				IsSoundOn = false;
				return;
			}
			foreach (var sound in Sounds)
			{
				if (sound.StateClass == CurrentStateClass)
				{
					AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(sound.SoundName), sound.BeeperType, sound.IsContinious);
					return;
				}
			}
			AlarmPlayerHelper.Stop();
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