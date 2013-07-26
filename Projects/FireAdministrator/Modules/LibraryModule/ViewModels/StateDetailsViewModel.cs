using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
    public class StateDetailsViewModel : SaveCancelDialogViewModel
	{
		public StateDetailsViewModel(DeviceViewModel deviceViewModel)
			: base()
		{
			Title = "Добавить состояние";

            var libraryStates = new List<LibraryState>();
			foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
			{
				if (!deviceViewModel.States.Any(x => x.StateType == stateType && x.Code == null))
                {
                    var libraryState = new LibraryState()
                    {
                        StateType = stateType
                    };
                    libraryStates.Add(libraryState);
                }
			}

			var driverStates = from DriverState driverState in deviceViewModel.Driver.States orderby driverState.StateType select driverState;
            foreach (var driverState in driverStates)
            {
				if (driverState.Name != null && !deviceViewModel.States.Any(x => x.Code == driverState.Code))
                {
                    var libraryState = new LibraryState()
                    {
                        StateType = driverState.StateType,
                        Code = driverState.Code
                    };
                    libraryStates.Add(libraryState);
                }
            }

            States = new List<StateViewModel>();
            foreach (var libraryState in libraryStates)
            {
				var stateViewModel = new StateViewModel(libraryState, deviceViewModel.Driver);
                States.Add(stateViewModel);
            }
            SelectedState = States.FirstOrDefault();
		}

        public List<StateViewModel> States { get; private set; }
        public StateViewModel SelectedState { get; set; }

        protected override bool CanSave()
        {
            return SelectedState != null;
        }
	}
}