using System;
using System.IO;
using System.Media;
using System.Threading;
using RubezhAPI.Models;
using Common;

namespace Infrastructure.Common.Windows
{
	public static class AlarmPlayerHelper
	{
		static SoundPlayer _soundPlayer;
		static bool _isContinious;
		static Thread _beepThread;
		static int _beepFrequency;
		static bool _isBeepThreadStopping;

		static AlarmPlayerHelper()
		{
			_soundPlayer = new SoundPlayer();
		}

		static void PlayBeep()
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
			catch (Exception)
			{
				return;
			}
		}

		static void StopPlayPCSpeaker()
		{
			_isBeepThreadStopping = true;
		}

		static void PlayPCSpeaker(BeeperType beeperType, bool isContinious)
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

		static void PlaySound(string filePath, bool isContinious)
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

		static void StopPlaySound()
		{
			if (_soundPlayer != null)
			{
				_soundPlayer.Stop();
			}
		}

		public static void Play(string filePath, BeeperType speakertype, bool isContinious)
		{
			try
			{
				PlaySound(filePath, isContinious);
				PlayPCSpeaker(speakertype, isContinious);
			}
			catch(Exception e)
			{
				Logger.Error(e, "AlarmPlayerHelper.Play");
			}
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