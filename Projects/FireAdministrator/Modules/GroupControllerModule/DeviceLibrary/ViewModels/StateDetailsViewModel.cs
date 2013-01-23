using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StateDetailsViewModel : SaveCancelDialogViewModel
	{
		public StateDetailsViewModel(LibraryXDevice libraryDevice)
			: base()
		{
			Title = "Добавить состояние";

			var libraryStates = new List<LibraryXState>();
			foreach (XStateClass xstateClass in Enum.GetValues(typeof(XStateClass)))
			{
				if ((!libraryDevice.XStates.Any(x => x.XStateClass == xstateClass && x.Code == null)) && (libraryDevice.Driver.AvailableStateClasses.Exists(x => x == xstateClass)))
				{
					var libraryState = new LibraryXState()
					{
						XStateClass = xstateClass
					};
					libraryStates.Add(libraryState);
				}
			}

			States = new List<StateViewModel>();
			foreach (var libraryState in libraryStates)
			{
				var stateViewModel = new StateViewModel(libraryState, libraryDevice.Driver);
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