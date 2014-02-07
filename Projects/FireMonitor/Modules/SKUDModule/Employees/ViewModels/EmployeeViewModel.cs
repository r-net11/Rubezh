using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		public EmployeeViewModel(Employee employee)
		{
			Employee = employee;
			var department = FiresecManager.GetDepartment(employee.DepartmentUid);
			DepartmentName = (department != null) ? department.Name : "";
			var position = FiresecManager.GetPosition(employee.PositionUid);
			PositionName = (position != null) ? position.Name : "";
			if (employee.Appointed.HasValue)
				AppointedString = employee.Appointed.Value.ToString("d MMM yyyy");
			if (employee.Dismissed.HasValue)
				DismissedString = employee.Dismissed.Value.ToString("d MMM yyyy");
		}

		public Employee Employee { get; set; }
		public string DepartmentName { get; set; }
		public string PositionName { get; set; }
		public string AppointedString { get; set; }
		public string DismissedString { get; set; }

		public void Update(Employee employee)
		{
			Employee = employee;
			OnPropertyChanged("Employee");
			OnPropertyChanged("DepartmentName");
			OnPropertyChanged("PositionName");
			OnPropertyChanged("AppointedString");
			OnPropertyChanged("DismissedString");
		}
	}
}