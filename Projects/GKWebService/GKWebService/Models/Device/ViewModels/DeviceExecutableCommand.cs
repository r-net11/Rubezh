using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models.ViewModels
{
	public class DeviceExecutableCommand
	{
		/// <summary>
		/// Надпись на кнопке управления
		/// </summary>
		public String ButtonName
		{
			get { return _command.ToDescription(); }
		}

		/// <summary>
		/// Имя функции, отвечающей за кнопку управления
		/// </summary>
		public String Name
		{
			get { return _command.ToString(); }
		}


		public DeviceExecutableCommand(GKStateBit command)
		{
			_command = command;
		}

		private GKStateBit _command { get; set; }

	}
}
