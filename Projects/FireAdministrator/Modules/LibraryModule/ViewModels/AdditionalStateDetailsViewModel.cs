using System.Linq;

namespace LibraryModule.ViewModels
{
    public class AdditionalStateDetailsViewModel : DetailsBaseViewModel<StateViewModel>
    {
        public AdditionalStateDetailsViewModel(DeviceLibrary.Models.Device device)
        {
            Initialize("Добавить дополнительное состояние");

            var driver = FiresecClient.FiresecManager.Drivers.First(x => x.Id == device.Id);
            foreach (var state in driver.States)
            {
                if (state.Name != null &&
                    !device.States.Any(x => x.Code == state.Code))
                {
                    Items.Add(
                        new StateViewModel(
                            StateViewModel.GetDefaultStateWith(state.Priority.ToString(), state.Code), driver));
                }
            }
        }
    }
}
