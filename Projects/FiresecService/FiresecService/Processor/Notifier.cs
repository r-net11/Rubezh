
using StrazhAPI.Models;
using System;

namespace StrazhService
{
	public static class Notifier
	{
		private static IStrazhNotifier _notifier;

		public static void SetNotifier(IStrazhNotifier notifier)
		{
			_notifier = notifier;
		}

		public static string GetLogs()
		{
			return _notifier == null ? string.Empty : _notifier.Logs;
		}

		public static void Log(string message)
		{
			if (_notifier != null)
				_notifier.Log(message);
		}

		internal static void BalloonShowFromServer(string text)
		{
			if (_notifier != null)
				_notifier.BalloonShowFromServer(text);
		}

		internal static void AddClient(ClientCredentials clientCredentials)
		{
			if (_notifier != null)
				_notifier.AddClient(clientCredentials);
		}
		internal static void RemoveClient(Guid uid)
		{
			if (_notifier != null)
				_notifier.RemoveClient(uid);
		}

		internal static void EditClient(Guid uid, string login)
		{
			if (_notifier != null)
				_notifier.EditClient(uid, login);
		}
	}
}
