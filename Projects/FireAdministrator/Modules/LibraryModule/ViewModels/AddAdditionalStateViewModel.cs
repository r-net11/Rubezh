using System.Collections.ObjectModel;
using System.Linq;

namespace LibraryModule.ViewModels
{
    public class AddAdditionalStateViewModel : AddViewModel<DeviceViewModel, StateViewModel>
    {
        public AddAdditionalStateViewModel(DeviceViewModel parentDevice)
            : base(parentDevice) { }

        override public void Initialize()
        {
            Title = "Добавить дополнительное состояние";

            Items = new ObservableCollection<StateViewModel>();
            foreach (var state in Parent.Driver.States)
            {
                if (Parent.States.Any(x => x.Id == state.id && x.IsAdditional) == false)
                {
                    Items.Add(new StateViewModel(state, Parent));
                }
            }
            Items = new ObservableCollection<StateViewModel>(
                        from state in Items
                        orderby state.Name
                        orderby state.ClassName
                        select state);
        }
    }
}
