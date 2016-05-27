namespace StrazhService.Monitor
{
	public enum ServiceState
	{
		/// <summary>
		/// Сервер остановлен
		/// </summary>
		Stoped = 0,
		
		/// <summary>
		/// Сервер запускается
		/// </summary>
		Starting = 1,
		
		/// <summary>
		/// Сервер запущен
		/// </summary>
		Started = 2
	}
}