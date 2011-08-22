using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        public EventViewModel(StateType id)
        {
            Id = id;
        }

        public StateType Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}