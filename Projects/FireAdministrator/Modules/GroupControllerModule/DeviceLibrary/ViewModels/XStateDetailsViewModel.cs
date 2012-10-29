using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class XStateDetailsViewModel : SaveCancelDialogViewModel
    {
        public XStateDetailsViewModel(LibraryXDevice libraryXDevice)
            : base()
        {
            Title = "Добавить состояние";

            var libraryXStates = new List<LibraryXState>();
            foreach (XStateType xstateType in Enum.GetValues(typeof(XStateType)))
            {
                if (!libraryXDevice.XStates.Any(x => x.XStateType == xstateType && x.Code == null))
                {
                    var libraryXState = new LibraryXState()
                    {
                        XStateType = xstateType
                    };
                    libraryXStates.Add(libraryXState);
                }
            }

            var availableStates = from XStateType xdriverState in libraryXDevice.XDriver.AvailableStates orderby xdriverState select xdriverState;
            foreach (var availableState in availableStates)
            {
                if (availableState != null && !libraryXDevice.XStates.Any(x => x.XStateType == availableState))
                {
                    var libraryXState = new LibraryXState()
                    {
                        XStateType = availableState,
                    };
                    libraryXStates.Add(libraryXState);
                }
            }

            XStates = new List<XStateViewModel>();
            foreach (var libraryXState in libraryXStates)
            {
                var stateViewModel = new XStateViewModel(libraryXState, libraryXDevice.XDriver);
                XStates.Add(stateViewModel);
            }
            SelectedXState = XStates.FirstOrDefault();
        }

        public List<XStateViewModel> XStates { get; private set; }
        public XStateViewModel SelectedXState { get; set; }

        protected override bool CanSave()
        {
            return SelectedXState != null;
        }
    }
}