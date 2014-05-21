using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class ProcedureInputObjectViewModel : BaseViewModel
	{
		public ProcedureInputObject ProcedureInputObject { get; private set; }

		public ProcedureInputObjectViewModel(ProcedureInputObject procedureInputObject)
		{
			ProcedureInputObject = procedureInputObject;

			switch(procedureInputObject.ProcedureObjectType)
			{
				case ProcedureObjectType.XDevice:
					Name = "Неизвестное устройство";
					if (ProcedureInputObject.UID != Guid.Empty)
					{
						var device = XManager.Devices.FirstOrDefault(x => x.UID == ProcedureInputObject.UID);
						if (device != null)
						{
							Name = device.PresentationName;
						}
					}
					break;

				case ProcedureObjectType.Camera:
					Name = "Камера";
					break;
			}
		}

		public string Name { get; private set; }
	}
}