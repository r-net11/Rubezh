using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        public EventViewModel(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}
