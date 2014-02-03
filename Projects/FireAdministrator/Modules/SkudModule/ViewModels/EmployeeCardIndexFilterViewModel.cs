using System.Collections.ObjectModel;
using FiresecAPI.Models.SKDDatabase;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Properties;

namespace SKDModule.ViewModels
{
	public class EmployeeCardIndexFilterViewModel : SaveCancelDialogViewModel
	{
		public EmployeeCardIndexFilter Filter { get; private set; }
		public ObservableCollection<EmployeePosition> Positions { get; private set; }
		public ObservableCollection<EmployeeDepartment> Departments { get; private set; }
		public ObservableCollection<EmployeeGroup> Groups { get; private set; }

		public EmployeeCardIndexFilterViewModel(EmployeeCardIndexFilter filter)
		{
			Title = Resources.EmployeeCardIndexFilterTitle;
			Filter = filter;
			Initialize();
		}

		private void Initialize()
		{
			//Positions = new ObservableCollection<EmployeePosition>(FiresecManager.GetEmployeePositions());
			Positions.Insert(0, new EmployeePosition() { Id = 0 });
			//Departments = new ObservableCollection<EmployeeDepartment>(FiresecManager.GetEmployeeDepartments());
			Departments.Insert(0, new EmployeeDepartment() { Id = 0 });
			//Groups = new ObservableCollection<EmployeeGroup>(FiresecManager.GetEmployeeGroups());
			Groups.Insert(0, new EmployeeGroup() { Id = 0 });
		}

		public void Update()
		{
			OnPropertyChanged("Filter");
		}

		protected override bool Save()
		{
			bool res = base.Save();
			if (res)
				Update();
			return res;
		}
	}
}