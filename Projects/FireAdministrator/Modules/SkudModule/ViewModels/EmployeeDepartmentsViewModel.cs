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
	public class EmployeeDepartmentsViewModel : EmployeeDictionaryViewModel<EmployeeDepartment>
	{
		protected override IEnumerable<EmployeeDepartment> GetDictionary()
		{
			return FiresecManager.GetEmployeeDepartments();
		}
	}
}