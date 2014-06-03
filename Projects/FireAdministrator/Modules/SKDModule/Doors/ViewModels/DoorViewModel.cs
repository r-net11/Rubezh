using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public Door Door { get; set; }

		public DoorViewModel(Door door)
		{
			SelectInpitDeviceCommand = new RelayCommand(OnSelectInpitDevice);
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
				OnPropertyChanged("Name");
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
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand SelectInpitDeviceCommand { get; private set; }
		void OnSelectInpitDevice()
		{
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(Door door)
		{
			Door = door;
			OnPropertyChanged("Door");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		public void Update()
		{
			if (Door.PlanElementUIDs == null)
				Door.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Door.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Door.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}
	}
}