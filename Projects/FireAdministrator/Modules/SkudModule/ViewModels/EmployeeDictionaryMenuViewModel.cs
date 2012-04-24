using Infrastructure.Common;

namespace SkudModule.ViewModels
{
    public class EmployeeDictionaryMenuViewModel
    {
		public EmployeeDictionaryMenuViewModel(RegionViewModel dictionaryViewModel)
        {
			Context = dictionaryViewModel;
        }

		public RegionViewModel Context { get; private set; }
    }
}