using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using SKUDModule.Models;

namespace SKUDModule.ViewModels
{
	public class EmployeeViewModel:BaseViewModel
	{
		public EmployeeViewModel(Employee employee)
		{
			Employee = employee;
		}

		public Employee Employee { get; set; }
	}
}
