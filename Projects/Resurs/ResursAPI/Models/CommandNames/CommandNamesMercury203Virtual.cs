using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.CommandNames
{
	/// <summary>
	/// Команды для устройтсва Mercury203Virtual
	/// </summary>
	public class CommandNamesMercury203Virtual: CommandNamesMercury203
	{
		/// <summary>
		/// Симулирует (устанавливает ошибку поступа к удалённому устройству)
		/// </summary>
		public const string SetCommunicationError = "SetCommunicationError";
		/// <summary>
		/// Симулирует (сбрасывает ошибку поступа к удалённому устройству)
		/// </summary>
		public const string ResetCommunicationError = "ResetCommunicationError";
		/// <summary>
		/// Симулирует (устанавливает ошибку конфигурации устройства)
		/// </summary>
		public const string SetConfigurationError = "SetConfigurationError";
		/// <summary>
		/// Симулирует (сбрасывает ошибку конфигурации устройства)
		/// </summary>
		public const string ResetConfigurationError = "ResetConfigurationError";
		/// <summary>
		/// Симулирует (устанавливает ошибку часов в устройстве)
		/// </summary>
		public const string SetRtcError = "SetRtcError";
		/// <summary>
		/// Симулирует (сбрасывает ошибку ошибку часов в устройстве)
		/// </summary>
		public const string ResetRtcError = "ResetRtcError";
	}
}
