using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class ReportViewModel : BaseViewModel
	{
		public ReportViewModel(Employee employee)
		{
			Employee = employee;
			var department = DepartmentHelper.GetSingle(employee.DepartmentUID);
			DepartmentName = (department != null) ? department.Name : "";
			var position = PositionHelper.GetSingle(employee.PositionUID);
			PositionName = (position != null) ? position.Name : "";
			AppointedString = employee.Appointed.ToString("d MMM yyyy");
			DismissedString = employee.Dismissed.ToString("d MMM yyyy");
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