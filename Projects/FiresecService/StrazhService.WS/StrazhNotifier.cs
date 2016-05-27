using System;
using System.Text;
using StrazhAPI.Models;

namespace StrazhService.WS
{
	public class StrazhNotifier : IStrazhNotifier
	{
		private readonly StringBuilder _stringBuilder;

		public StrazhNotifier()
		{
			_stringBuilder = new StringBuilder();
		}

		public string Logs
		{
			get { return _stringBuilder.ToString(); }
		}

		public void Log(string message)
		{
			if (string.IsNullOrEmpty(message))
				return;

			_stringBuilder.AppendLine(message);
		}

		public void BalloonShowFromServer(string text)
		{
			//throw new NotImplementedException();
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			//throw new NotImplementedException();
		}

		public void RemoveClient(Guid uid)
		{
			//throw new NotImplementedException();
		}

		public void EditClient(Guid uid, string login)
		{
			//throw new NotImplementedException();
		}
	}
}