using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public CategoryViewModel(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}