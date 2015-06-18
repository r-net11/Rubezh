using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKImitator.ViewModels
{
	public class ParametersViewModel : SaveCancelDialogViewModel
	{
		GKBase GKBase;

		public ParametersViewModel(GKBase gkBase)
		{
			Title = "Параметры";
			GKBase = gkBase;

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				foreach (var property in device.Properties)
				{
					if (property.DriverProperty.IsAUParameter)
					{

					}
				}
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}