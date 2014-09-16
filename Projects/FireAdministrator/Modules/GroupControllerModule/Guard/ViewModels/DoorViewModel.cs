using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public XDoor Door { get; set; }

		public DoorViewModel(XDoor door)
		{
			Door = door;
			Update();
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public string Description
		{
			get { return Door.Description; }
			set
			{
				Door.Description = value;
				Door.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public void Update(XDoor door)
		{
			Door = door;
			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
		}
	}
}