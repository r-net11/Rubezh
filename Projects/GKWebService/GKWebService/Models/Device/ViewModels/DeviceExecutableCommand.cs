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
			get
			{
				String buttonName = _command.ToDescription();
				if (_driverType == GKDriverType.RSR2_Valve_DU || _driverType == GKDriverType.RSR2_Valve_KV || _driverType == GKDriverType.RSR2_Valve_KVMV)
				{
					switch (_command)
					{
						case GKStateBit.TurnOn_InManual:
							buttonName = "Открыть";
							break;
						case GKStateBit.TurnOnNow_InManual:
							buttonName = "Открыть немедленно";
							break;
						case GKStateBit.TurnOff_InManual:
							buttonName = "Закрыть";
							break;
						case GKStateBit.Stop_InManual:
							buttonName = "Остановить";
							break;
					}
				}
				return buttonName;
			}
		}

		/// <summary>
		/// Имя функции, отвечающей за кнопку управления
		/// </summary>
		public String Name
		{
			get { return _command.ToString(); }
		}


		public DeviceExecutableCommand(GKDriverType driverType, GKStateBit command)
		{
			_command = command;
			_driverType = driverType;
		}

		private GKStateBit _command { get; set; }
		private GKDriverType _driverType { get; set; }
	}
}
