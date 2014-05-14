using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class LibraryStateDetailsViewModel : SaveCancelDialogViewModel
	{
		public LibraryStateDetailsViewModel(SKDLibraryDevice skdLibraryDevice)
			: base()
		{
			Title = "Добавить состояние";

			var skdLibraryStates = new List<SKDLibraryState>();
			foreach (XStateClass stateClass in Enum.GetValues(typeof(XStateClass)))
			{
				if ((!skdLibraryDevice.States.Any(x => x.StateClass == stateClass)) && (skdLibraryDevice.Driver.AvailableStateClasses.Exists(x => x == stateClass)))
				{
					var skdLibraryState = new SKDLibraryState()
					{
						StateClass = stateClass
					};
					skdLibraryStates.Add(skdLibraryState);
				}
			}

			States = new List<StateViewModel>();
			foreach (var libraryState in skdLibraryStates)
			{
				var stateViewModel = new StateViewModel(libraryState, skdLibraryDevice.Driver);
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