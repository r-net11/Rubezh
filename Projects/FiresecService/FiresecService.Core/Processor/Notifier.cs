
using RubezhAPI.Models;
using System;
namespace FiresecService
{
	public static class Notifier
	{
		static IFiresecNotifier _notifier;

		public static void SetNotifier(IFiresecNotifier notifier)
		{
			_notifier = notifier;
		}

		public static void OnApplicationClosing()
		{
			GKProcessor.Stop();
		}

		internal static void UILog(string message, bool isError = false)
		{
			if (_notifier != null)
				_notifier.UILog(message, isError);
		}

		internal static void BalloonShowFromServer(string text)
		{
			if (_notifier != null)
				_notifier.BalloonShowFromServer(text);
		}

		internal static void OnPoll(Guid clientUID)
		{
			if (_notifier != null)
				_notifier.OnPoll(clientUID);
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

		internal static void AddServerTask(ServerTask serverTask)
		{
			if (_notifier != null)
				_notifier.AddServerTask(serverTask);
		}

		internal static void RemoveServerTask(ServerTask serverTask)
		{
			if (_notifier != null)
				_notifier.RemoveServerTask(serverTask);
		}

		internal static void SetLocalAddress(string address)
		{
			if (_notifier != null)
				_notifier.SetLocalAddress(address);
		}

		internal static void SetRemoteAddress(string address)
		{
			if (_notifier != null)
				_notifier.SetRemoteAddress(address);
		}
	}
}
