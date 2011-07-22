using System.Collections.ObjectModel;
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

            var items = new ObservableCollection<StateViewModel>();
            for (int classId = 0; classId < 9; ++classId)
            {
                if (!Parent.States.Any(x => x.Class == classId.ToString()
                    && !x.IsAdditional))
                {
                    items.Add(new StateViewModel(classId.ToString(), Parent));
                }
            }
            Items = new ObservableCollection<StateViewModel>(
                        from state in items
                        orderby state.Name
                        select state);
        }
    }
}
