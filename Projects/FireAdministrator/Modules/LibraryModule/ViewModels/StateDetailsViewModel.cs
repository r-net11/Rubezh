using System.Linq;

namespace LibraryModule.ViewModels
{
    public class StateDetailsViewModel : DetailsBaseViewModel<StateViewModel>
    {
        public StateDetailsViewModel(DeviceLibrary.Models.Device device)
        {
            Initialize("Добавить состояние");

            var driver = FiresecClient.FiresecManager.Drivers.First(x => x.Id == device.Id);
            for (int classId = 0; classId < 9; ++classId)
            {
                if (!device.States.Any(x => x.Class == classId.ToString() &&
                    x.Code == null))
                {
                    Items.Add(
                        new StateViewModel(
                            StateViewModel.GetDefaultStateWith(classId.ToString()), driver));
                }
            }
        }
    }
}
