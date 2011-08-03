using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class Event : BaseViewModel
    {
        public Event(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}
