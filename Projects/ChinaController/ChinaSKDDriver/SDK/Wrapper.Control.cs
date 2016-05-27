using ChinaSKDDriverNativeApi;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Открывает замок
		/// </summary>
		/// <param name="channelNo">Идентификатор замка</param>
		/// <returns>true - операция завершилась успеншо,
		/// false - операция завершилась с ошибкой</returns>
		public bool OpenDoor(int channelNo)
		{
			return NativeWrapper.WRAP_OpenDoor(LoginID, channelNo);
		}

		/// <summary>
		/// Закрывает замок
		/// </summary>
		/// <param name="channelNo">Идентификатор замка</param>
		/// <returns>true - операция завершилась успеншо,
		/// false - операция завершилась с ошибкой</returns>
		public bool CloseDoor(int channelNo)
		{
			return NativeWrapper.WRAP_CloseDoor(LoginID, channelNo);
		}

		/// <summary>
		/// Получает состояние замка (открыт/закрыт)
		/// </summary>
		/// <param name="channelNo">Идентификатор замка</param>
		/// <returns>Состояние замка</returns>
		public int GetDoorStatus(int channelNo)
		{
			return NativeWrapper.WRAP_GetDoorStatus(LoginID, channelNo);
		}

		/// <summary>
		/// Сбрасывает состояние "Взлом"
		/// </summary>
		/// <param name="channelNo">Идентификатор замка</param>
		/// <returns>true - операция завершилась успеншо,
		/// false - операция завершилась с ошибкой</returns>
		public bool PromptWarning(int channelNo)
		{
			return NativeWrapper.WRAP_PromptWarning(LoginID, channelNo);
		}
	}
}