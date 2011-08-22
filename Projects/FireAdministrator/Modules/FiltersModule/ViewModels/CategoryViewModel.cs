using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public CategoryViewModel(DeviceCategoryType id)
        {
            Id = id;
        }

        public DeviceCategoryType Id { get; private set; }
        public bool IsChecked { get; set; }
    }
}