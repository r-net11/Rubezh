using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Input;
using Localization.FireMonitor.ViewModels;

namespace FireMonitor.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public SoundViewModel()
		{
			SoundOnOffCommand = new RelayCommand(OnSoundOnOff);
			StopPlayingCommand = new RelayCommand(OnStopPlaying);
			IsSoundOn = true;
		}

		private bool _isSoundOn;
		public bool IsSoundOn
		{
			get { return _isSoundOn; }
			set
			{
				_isSoundOn = value;
				OnPropertyChanged(() => IsSoundOn);
			}
		}

		public ICommand SoundOnOffCommand { get; private set; }
		private void OnSoundOnOff()
		{
			// Выключить звук
			if (IsSoundOn)
			{
				if (MessageBoxService.ShowConfirmation(CommonViewModels.DisableSounds))
				{
					AlarmPlayerHelper.IsMuted = true;
					IsSoundOn = false;
				}
			}
			// Включить звук
			else
			{
				AlarmPlayerHelper.IsMuted = false;
				IsSoundOn = true;
			}
		}

		public ICommand StopPlayingCommand { get; private set; }
		private void OnStopPlaying()
		{
			AlarmPlayerHelper.Stop();
		}
	}
}