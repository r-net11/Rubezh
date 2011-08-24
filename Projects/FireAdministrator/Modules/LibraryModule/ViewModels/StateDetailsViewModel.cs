using System;
using System.Linq;
using FiresecAPI.Models;

namespace LibraryModule.ViewModels
{
    public class StateDetailsViewModel : DetailsBaseViewModel<StateViewModel>
    {
        public StateDetailsViewModel(LibraryDevice device)
            : base()
        {
            Title = "Добавить состояние";

            var driver = FiresecClient.FiresecManager.Drivers.First(x => x.Id == device.DriverId);
            foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            {
                if (!device.States.Any(x => x.StateType == stateType && x.Code == null))
                {
                    Items.Add(new StateViewModel(StateViewModel.GetDefaultStateWith(stateType), driver));
                }
            }
        }
    }
}