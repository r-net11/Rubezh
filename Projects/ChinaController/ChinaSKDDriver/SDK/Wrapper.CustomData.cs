using StrazhDeviceSDK.NativeAPI;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Получает произвольные пользовательские данные с контроллера
		/// </summary>
		/// <returns>Произвольные пользовательские данные</returns>
		public string GetCustomData()
		{
			NativeWrapper.WRAP_CustomData customData;
			return NativeWrapper.WRAP_GetCustomData(LoginID, out customData) ? customData.CustomData : null;
		}

		/// <summary>
		/// Сохнаняет произвольные пользовательские данные на контроллер
		/// </summary>
		/// <param name="data">Произвольные пользовательские данные</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetCustomData(string data)
		{
			if (data == null)
				return false;

			var customData = new NativeWrapper.WRAP_CustomData { CustomDataLength = data.Length, CustomData = data };
			return NativeWrapper.WRAP_SetCustomData(LoginID, ref customData);
		}
	}
}