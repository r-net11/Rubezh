using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common
{
	public static class LoadingErrorManager
	{
		static List<string> LoadingErrors = new List<string>();

		public static void Clear()
		{
			LoadingErrors = new List<string>();
		}

		public static void Add(string message)
		{
			LoadingErrors.Add(message);
		}

		public static void Add(Exception e)
		{
			LoadingErrors.Add(e.Message);
		}

		public static bool HasError
		{
			get { return LoadingErrors.Count > 0; }
		}

		public static new string ToString()
		{
			if (LoadingErrors.Count == 0)
				return null;

			var stringBuilder = new StringBuilder();
			foreach (var loadingError in LoadingErrors)
			{
				stringBuilder.AppendLine(loadingError);
			}
			return stringBuilder.ToString();
		}
	}
}