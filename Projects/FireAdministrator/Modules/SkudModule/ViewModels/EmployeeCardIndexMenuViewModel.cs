using Infrastructure.Common;

namespace SkudModule.ViewModels
{
    public class EmployeeCardIndexMenuViewModel
    {
		public EmployeeCardIndexMenuViewModel(EmployeeCardIndexViewModel employeeCardIndexViewModel)
        {
			Context = employeeCardIndexViewModel;
        }

		public EmployeeCardIndexViewModel Context { get; private set; }
    }
}
