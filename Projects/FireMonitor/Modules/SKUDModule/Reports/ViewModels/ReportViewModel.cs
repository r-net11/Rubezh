using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReportViewModel : BaseViewModel
	{
		public ReportViewModel(ShortEmployee employee)
		{
			Employee = employee;
			DepartmentName = employee.DepartmentName;
			PositionName = employee.PositionName;
			AppointedString = employee.Appointed;
			DismissedString = employee.Dismissed;
		}

		public ShortEmployee Employee { get; set; }
		public string DepartmentName { get; set; }
		public string PositionName { get; set; }
		public string AppointedString { get; set; }
		public string DismissedString { get; set; }

		public void Update(ShortEmployee employee)
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