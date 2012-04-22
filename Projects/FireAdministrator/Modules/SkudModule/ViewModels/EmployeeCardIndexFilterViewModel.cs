using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using FiresecAPI.Models.Skud;
using Controls.MessageBox;
using System.Windows;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexFilterViewModel : SaveCancelDialogContent
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
			Positions = new ObservableCollection<EmployeePosition>(FiresecManager.GetEmployeePositions());
			Positions.Insert(0, new EmployeePosition() { Id = 0 });
			Departments = new ObservableCollection<EmployeeDepartment>(FiresecManager.GetEmployeeDepartments());
			Departments.Insert(0, new EmployeeDepartment() { Id = 0 });
			Groups = new ObservableCollection<EmployeeGroup>(FiresecManager.GetEmployeeGroups());
			Groups.Insert(0, new EmployeeGroup() { Id = 0 });
		}

		public void Update()
		{
			OnPropertyChanged("Filter");
		}

		protected override void Save(ref bool cancel)
		{
			base.Save(ref cancel);
			if (cancel)
				return;
			Update();
		}
	}
}