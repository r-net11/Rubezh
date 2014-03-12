using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		public EmployeeViewModel(Employee employee)
		{
			Employee = employee;
			var departmentUID = (Employee.IsReplaced) ? Employee.DepartmentUID : Employee.CurrentReplacement.DepartmentUID;
			var department = DepartmentHelper.GetSingle(departmentUID);
			DepartmentName = (department != null) ? department.Name : "";
			var position = PositionHelper.GetSingle(Employee.PositionUID);
			PositionName = (position != null) ? position.Name : "";
			AppointedString = Employee.Appointed.ToString("d MMM yyyy");
			DismissedString = Employee.Dismissed.ToString("d MMM yyyy");
		}

		public Employee Employee { get; set; }
		public string DepartmentName { get; set; }
		public string PositionName { get; set; }
		public string AppointedString { get; set; }
		public string DismissedString { get; set; }

		public void Update(Employee employee)
		{
			Employee = employee;
			OnPropertyChanged(()=>Employee);
			OnPropertyChanged(()=>DepartmentName);
			OnPropertyChanged(()=>PositionName);
			OnPropertyChanged(()=>AppointedString);
			OnPropertyChanged(()=>DismissedString);
		}
	}
}