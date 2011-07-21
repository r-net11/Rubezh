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

            Items = new ObservableCollection<StateViewModel>();
            for (int stateId = 0; stateId < 9; ++stateId)
            {
                string id = stateId.ToString();
                if (Parent.States.Any(x => x.Id == id && !x.IsAdditional) == false)
                {
                    Items.Add(new StateViewModel(id, Parent));
                }
            }
            Items = new ObservableCollection<StateViewModel>(
                        from state in Items
                        orderby state.Name
                        select state);
        }
    }
}
