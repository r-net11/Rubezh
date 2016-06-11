using System.IO;
using System.Media;

namespace Infrastructure.Common
{
	public static class AlarmPlayerHelper
	{
		private static SoundPlayer _soundPlayer;
		private static bool _isMuted;

		static AlarmPlayerHelper()
		{
			_soundPlayer = new SoundPlayer();
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
			if (!IsMuted)
				PlaySound(filePath, isContinious);
		}

		public static void Stop()
		{
			StopPlaySound();
		}

		public static bool IsMuted
		{
			get { return _isMuted; }
			set
			{
				_isMuted = value;
				if (_isMuted)
					StopPlaySound();
			}
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