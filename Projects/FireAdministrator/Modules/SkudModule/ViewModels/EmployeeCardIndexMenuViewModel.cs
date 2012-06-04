
using Infrastructure.Common.Windows.ViewModels;
namespace SkudModule.ViewModels
{
    public class EmployeeCardIndexMenuViewModel : BaseViewModel
    {
        public EmployeeCardIndexMenuViewModel(EmployeeCardIndexViewModel employeeCardIndexViewModel)
        {
            Context = employeeCardIndexViewModel;
        }

        public EmployeeCardIndexViewModel Context { get; private set; }
    }
}