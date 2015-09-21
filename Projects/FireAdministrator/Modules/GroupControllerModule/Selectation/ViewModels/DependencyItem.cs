using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.Selectation.ViewModels
{
	public class DependencyItemViewModel
	{
		public string Name { get; set; }
		public string ImageSource { get; set; }

		GKBase gkBase;

		public DependencyItemViewModel(GKBase gkBase)
		{
			ShowCommand =  new RelayCommand(OnShow);

			this.gkBase = gkBase;
			Name = gkBase.PresentationName;
			ImageSource = gkBase.ImageSource;
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			if (gkBase is GKDoor)
				ServiceFactoryBase.Events.GetEvent<ShowGKDoorEvent>().Publish(gkBase.UID);
			if (gkBase is GKDirection)
				ServiceFactoryBase.Events.GetEvent<ShowGKDirectionEvent>().Publish(gkBase.UID);
		}

	}
}
