using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class Category : BaseViewModel
    {
        public Category(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}
