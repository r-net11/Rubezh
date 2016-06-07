using StrazhAPI.Models;
using System;
using System.IO;
using System.Media;
using System.Threading;

namespace Infrastructure.Common
{
	public static class AlarmPlayerHelper
	{
		private static SoundPlayer _soundPlayer;
		private static bool _isContinious;
		private static Thread _beepThread;
		private static int _beepFrequency;
		private static bool _isBeepThreadStopping;

		static AlarmPlayerHelper()
		{
			_soundPlayer = new SoundPlayer();
		}

		private static void PlayBeep()
		{
			try
			{
				do
				{
					if (_isBeepThreadStopping)
						break;
					Console.Beep(_beepFrequency, 600);
					if (_isBeepThreadStopping)
						break;
					Thread.Sleep(1000);
				}
				while (_isContinious);
				_isBeepThreadStopping = false;
				_beepThread = null;
			}
			catch (Exception e)
			{
				return;
			}
		}

		private static void StopPlayPCSpeaker()
		{
			_isBeepThreadStopping = true;
		}

		private static void PlayPCSpeaker(BeeperType beeperType, bool isContinious)
		{
			if (beeperType != BeeperType.None)
			{
				_beepFrequency = (int)beeperType;
				_isContinious = isContinious;

				_isBeepThreadStopping = false;
				if (_beepThread == null)
				{
					_beepThread = new Thread(PlayBeep);
					_beepThread.Name = "PC Speaker";
					_beepThread.Start();
				}
			}
		}

		private static void PlaySound(string filePath, bool isContinious)
		{
			if (_soundPlayer != null)
			{
				if (File.Exists(filePath))
				{
					_soundPlayer.SoundLocation = filePath;
					_soundPlayer.Load();
					if (_soundPlayer.IsLoadCompleted)
					{
						if (isContinious)
							_soundPlayer.PlayLooping();
						else
							_soundPlayer.Play();
					}
				}
				else
				{
					_soundPlayer.Stop();
				}
			}
		}

		private static void StopPlaySound()
		{
			if (_soundPlayer != null)
			{
				_soundPlayer.Stop();
			}
		}

		public static void Play(string filePath, bool isContinious)
		{
			PlaySound(filePath, isContinious);
		}

		public static void Stop()
		{
			StopPlaySound();
			StopPlayPCSpeaker();
		}

		public static void Dispose()
		{
			Stop();
			if (_soundPlayer != null)
				_soundPlayer.Dispose();
			_soundPlayer = null;
		}
	}
}