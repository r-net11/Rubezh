using System.Linq;

namespace LibraryModule.ViewModels
{
    public class AdditionalStateDetailsViewModel : DetailsBaseViewModel<StateViewModel>
    {
        public AdditionalStateDetailsViewModel(FiresecAPI.Models.DeviceLibrary.Device device)
            : base()
        {
            Title = "Добавить дополнительное состояние";

            var driver = FiresecClient.FiresecManager.Drivers.First(x => x.Id == device.Id);
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