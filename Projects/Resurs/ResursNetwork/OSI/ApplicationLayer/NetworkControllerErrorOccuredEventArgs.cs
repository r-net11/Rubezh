using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer
{
	public class NetworkControllerErrorOccuredEventArgs : EventArgs
	{
		/// <summary>
		/// 
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Структура с флагами ошибок устройтсва
		/// </summary>
		public NetworkControllerErrors Errors { get; set; }
	}
}
