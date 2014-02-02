using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
    public class EmployeeDictionaryMenuViewModel : BaseViewModel
    {
		public EmployeeDictionaryMenuViewModel(BaseViewModel dictionaryViewModel)
        {
			Context = dictionaryViewModel;
        }

		public BaseViewModel Context { get; private set; }
    }
}