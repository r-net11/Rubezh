﻿using System.ComponentModel;

namespace StrazhAPI.Automation.Enums
{
	/// <summary>
	/// Описывает статус соединения в автоматизации
	/// </summary>
	public enum ConnectionStatus
	{
		/// <summary>
		/// Норма
		/// </summary>
		[Description("Норма")]
		Connected,
		
		/// <summary>
		/// Потеря связи
		/// </summary>
		[Description("Потеря связи")]
		Disconnected
	}
}