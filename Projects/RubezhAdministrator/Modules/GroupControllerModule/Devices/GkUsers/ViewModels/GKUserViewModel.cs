using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class GKUserViewModel
	{
		public GKUserViewModel(GKUser user, bool isDevice)
		{
			User = user;
			IsDevice = isDevice;
		}
		public GKUser User { get; private set; }
		public bool IsDevice { get; private set; }
		public bool IsPresent { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsDescriptorMissmatch { get; set; }
		public bool IsNonStructureMissmatch { get; set; }
		public bool IsMissmatch { get { return IsPresent || IsAbsent || IsDescriptorMissmatch || IsNonStructureMissmatch; } }

		public string MissmatchText 
		{ 
			get 
			{ 
				if((IsPresent && IsDevice) || (IsAbsent && !IsDevice))
					return "Отсутствует в системе";
				if ((IsPresent && !IsDevice) || (IsAbsent && IsDevice))
					return "Отсутствует в приборе";
				if (IsDescriptorMissmatch)
					return "Не совпадает набор дескрипторов";
				if (IsNonStructureMissmatch)
					return "Не совпадают свойства пользователей";
				return "";
			} 
		}
	}
}
