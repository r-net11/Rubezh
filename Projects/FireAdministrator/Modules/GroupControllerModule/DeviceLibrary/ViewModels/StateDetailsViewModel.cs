using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class StateDetailsViewModel : SaveCancelDialogViewModel
	{
		public StateDetailsViewModel(GKLibraryDevice glLibraryDevice)
			: base()
		{
			Title = "Добавить состояние";

			var libraryStates = new List<GKLibraryState>();
			foreach (XStateClass xstateClass in Enum.GetValues(typeof(XStateClass)))
			{
				if ((!glLibraryDevice.States.Any(x => x.StateClass == xstateClass)) && (glLibraryDevice.Driver.AvailableStateClasses.Exists(x => x == xstateClass)))
				{
					var libraryState = new GKLibraryState()
					{
						StateClass = xstateClass
					};
					libraryState.Frames.Add(new GKLibraryFrame() { Id = 0 });
					libraryStates.Add(libraryState);
				}
			}

			States = new List<StateViewModel>();
			foreach (var libraryState in libraryStates)
			{
				var stateViewModel = new StateViewModel(libraryState, glLibraryDevice.Driver);
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