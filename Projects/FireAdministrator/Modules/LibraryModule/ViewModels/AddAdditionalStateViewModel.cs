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

            var items = new ObservableCollection<StateViewModel>();
            foreach (var state in Parent.Driver.States)
            {
                if (!Parent.States.Any(x => x.Code == state.code))
                {
                    items.Add(new StateViewModel(state, Parent));
                }
            }
            Items = new ObservableCollection<StateViewModel>(
                        from state in items
                        orderby state.Name
                        orderby state.ClassName
                        select state);
        }
    }
}
