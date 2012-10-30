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
            foreach (XStateClass xstateClass in Enum.GetValues(typeof(XStateClass)))
            {
                if ((!libraryXDevice.XStates.Any(x => x.XStateClass == xstateClass && x.Code == null)) && (libraryXDevice.XDriver.AvailableStateClasses.Exists(x=>x==xstateClass)))
                {
                    var libraryXState = new LibraryXState()
                    {
                        XStateClass = xstateClass
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