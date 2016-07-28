using System;

namespace StrazhService.WS
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Сообщения вложенных эксепшенов
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static string InnerExceptionToString(this Exception exception)
		{
			if (exception.InnerException == null)
				return string.Empty;

			var errorMessage = String.Format(Environment.NewLine + "InnerException.Message: {0}"
											 + Environment.NewLine + "InnerException.Source: {1}"
											 + Environment.NewLine + "InnerException.StackTrace: {2}",
				exception.InnerException.Message,
				exception.InnerException.Source,
				exception.InnerException.StackTrace);

			if (exception.InnerException.InnerException != null)
				errorMessage += InnerExceptionToString(exception.InnerException);

			return errorMessage;
		}
	}
}