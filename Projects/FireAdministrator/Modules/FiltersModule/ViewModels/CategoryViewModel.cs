using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
	public class CategoryViewModel : BaseViewModel
	{
		public CategoryViewModel(DeviceCategoryType deviceCategoryType)
		{
			DeviceCategoryType = deviceCategoryType;
		}

		public DeviceCategoryType DeviceCategoryType { get; private set; }
		public bool IsChecked { get; set; }
	}
}