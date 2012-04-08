using Infrastructure.Common;

namespace SkudModule.ViewModels
{
    public class EmployeeCardIndexMenuViewModel
    {
		public EmployeeCardIndexMenuViewModel(EmployeeCardIndexViewModel employeeCardIndexViewModel)
        {
			Contex = employeeCardIndexViewModel;
        }

		public EmployeeCardIndexViewModel Contex { get; private set; }
    }
}
