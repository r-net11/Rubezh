using FiresecAPI.Models.SKDDatabase;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public EmployeeCard EmployeeCard { get; private set; }

		public EmployeeCardViewModel(EmployeeCard employeeCard)
		{
			EmployeeCard = employeeCard;
		}

		public void Update()
		{
			OnPropertyChanged("EmployeeCard");
		}
	}
}