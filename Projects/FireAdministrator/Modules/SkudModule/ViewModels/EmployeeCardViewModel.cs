using FiresecAPI.Models.Skud;
using Infrastructure.Common;

namespace SkudModule.ViewModels
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