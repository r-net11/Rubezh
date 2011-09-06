using System.Linq;
using FiresecAPI.Models;

namespace LibraryModule.ViewModels
{
    public class AdditionalStateDetailsViewModel : DetailsBaseViewModel<StateViewModel>
    {
        public AdditionalStateDetailsViewModel(LibraryDevice device)
            : base()
        {
            Title = "Добавить дополнительное состояние";

            var driver = FiresecClient.FiresecManager.Drivers.First(x => x.UID == device.DriverId);
            foreach (var state in driver.States)
            {
                if (state.Name != null &&
                    !device.States.Any(x => x.Code == state.Code))
                {
                    Items.Add(
                        new StateViewModel(StateViewModel.GetDefaultStateWith(state.StateType, state.Code), driver));
                }
            }
        }
    }
}