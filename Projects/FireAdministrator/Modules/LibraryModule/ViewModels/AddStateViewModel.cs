using System.Linq;

namespace LibraryModule.ViewModels
{
    public class AddStateViewModel : AddViewModel<DeviceViewModel, StateViewModel>
    {
        public AddStateViewModel(DeviceViewModel parentDevice)
            : base(parentDevice) { }

        override public void Initialize()
        {
            Title = "Добавить состояние";

            Items = new System.Collections.ObjectModel.ObservableCollection<StateViewModel>();
            for (int classId = 0; classId < 9; ++classId)
            {
                if (!Parent.States.Any(x => x.Class == classId.ToString()
                    && !x.IsAdditional))
                {
                    Items.Add(new StateViewModel(classId.ToString(), Parent));
                }
            }
        }
    }
}
