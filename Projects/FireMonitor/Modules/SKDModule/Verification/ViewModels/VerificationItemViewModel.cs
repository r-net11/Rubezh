using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class VerificationItemViewModel : BaseViewModel
	{
		public VerificationItemViewModel()
		{
			PersonTypeName = "Сотрудник";
			SKDSystemConfiguration = SKDManager.SKDConfiguration.SKDSystemConfiguration;
		}

		public string PersonTypeName { get; set; }
		public SKDSystemConfiguration SKDSystemConfiguration { get; private set; }

		public string EmployeeCardID { get; set; }
		public string EmployeeName { get; set; }
		public string EmployeePassport { get; set; }
		public string EmployeeTime { get; set; }
		public string EmployeeNo { get; set; }
		public string EmployeePosition { get; set; }
		public string EmployeeShedule { get; set; }
		public string EmployeeDepartment { get; set; }
		public string GuestCardID { get; set; }
		public string GuestName { get; set; }
		public string GuestWhere { get; set; }
		public string GuestConvoy { get; set; }
	}
}