using Infrastructure;
using Moq;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using SKDModule.ViewModels;
using SKDModuleTest.Mocks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SKDModuleTest
{
	[TestFixture]
	public class EmployeesTest
	{
		MockDialogService MockDialogService;
		MockMessageBoxService MockMessageBoxService;
		User User;

		[SetUp]
		public void Initialize()
		{
			ClientManager.SecurityConfiguration = new SecurityConfiguration();
			User = new User { Login = "adm" };
			ClientManager.SecurityConfiguration.Users.Add(User);
			ClientManager._userLogin = "adm";

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
		}

		[Test]
		public void AdditionalColumns()
		{
			var organisation1 = new Organisation();
			var organisation2 = new Organisation();
			var employee1 = new ShortEmployee { OrganisationUID = organisation1.UID };
			var employee2 = new ShortEmployee { OrganisationUID = organisation2.UID };
			var additionalColumnType1 = new AdditionalColumnType { OrganisationUID = organisation1.UID, IsInGrid = true, DataType = AdditionalColumnDataType.Text };
			var additionalColumnType2 = new AdditionalColumnType { OrganisationUID = organisation1.UID, IsInGrid = true, DataType = AdditionalColumnDataType.Text };
			var mock = new Mock<ISafeRubezhService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns<OrganisationFilter>(filter =>
				{
					var result = new List<Organisation>();
					if (filter.UIDs.Count == 0)
					{
						result.Add(organisation1);
						result.Add(organisation2);
					}
					if (filter.UIDs.Any(x => x == organisation1.UID))
						result.Add(organisation1);
					if (filter.UIDs.Any(x => x == organisation2.UID))
						result.Add(organisation2);
					return new OperationResult<List<Organisation>>(result);
				});
			mock.Setup(x => x.GetEmployeeList(It.IsAny<EmployeeFilter>())).Returns<EmployeeFilter>(filter =>
				{
					var result = new List<ShortEmployee>();
					if(filter.OrganisationUIDs.Count == 0)
					{
						result.Add(employee1);
						result.Add(employee2);
					}
					if (filter.OrganisationUIDs.Any(x => x == organisation1.UID))
						result.Add(employee1);
					if (filter.OrganisationUIDs.Any(x => x == organisation2.UID))
						result.Add(employee2);
					return new OperationResult<List<ShortEmployee>>(result);
				});
			mock.Setup(x => x.GetAdditionalColumnTypes(It.IsAny<AdditionalColumnTypeFilter>())).Returns<AdditionalColumnTypeFilter>(filter =>
			{
				var result = new List<AdditionalColumnType>();
				if (filter.OrganisationUIDs.Count == 0)
				{
					result.Add(additionalColumnType1);
					result.Add(additionalColumnType2);
				}
				if (filter.OrganisationUIDs.Any(x => x == organisation1.UID))
					result.Add(additionalColumnType1);
				if (filter.OrganisationUIDs.Any(x => x == organisation2.UID))
					result.Add(additionalColumnType2);
				return new OperationResult<List<AdditionalColumnType>>(result);
			});
			ClientManager.RubezhService = mock.Object;

			var employeesViewModel = new EmployeesViewModel();
			employeesViewModel.Initialize(new EmployeeFilter());
			Assert.IsTrue(employeesViewModel.AdditionalColumnTypes.Count == 2);
			employeesViewModel.Initialize(new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisation1.UID } });
			Assert.IsTrue(employeesViewModel.AdditionalColumnTypes.Count == 1);
		}

		[Test]
		public void IsShowDeletedPosition()
		{
			var organisation = new Organisation();
			var department = new ShortDepartment { Name = "DeletedDepartment", OrganisationUID = organisation.UID, IsDeleted = true };
			var position = new ShortPosition { Name = "DeletedPosition", OrganisationUID = organisation.UID, IsDeleted = true };
			var employee = new Employee 
			{
				FirstName = "FName",
				SecondName = "SName",
				LastName = "LName",
				OrganisationUID = organisation.UID, 
				DepartmentName = department.Name, 
				DepartmentUID = department.UID, 
				IsDepartmentDeleted = department.IsDeleted,
				PositionName = position.Name,
				PositionUID = position.UID,
				IsPositionDeleted = position.IsDeleted
			};
			var shortEmployee = new ShortEmployee
			{
				UID = employee.UID,
				FirstName = employee.FirstName,
				SecondName = employee.SecondName,
				LastName = employee.LastName,
				OrganisationUID = organisation.UID,
				DepartmentName = department.Name,
				IsDepartmentDeleted = department.IsDeleted,
				PositionName = position.Name,
				IsPositionDeleted = position.IsDeleted
			};
			ClientManager.CurrentUser.PermissionStrings.Add("Oper_SKD_Employees_Edit");
			var mock = new Mock<ISafeRubezhService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns(() =>
			{
				return new OperationResult<List<Organisation>>(new List<Organisation> { organisation });
			});
			mock.Setup(x => x.GetEmployeeList(It.IsAny<EmployeeFilter>())).Returns(() =>
			{
				return new OperationResult<List<ShortEmployee>>(new List<ShortEmployee> { shortEmployee});
			});
			mock.Setup(x => x.GetEmployeeDetails(It.IsAny<Guid>())).Returns(() =>
			{
				return new OperationResult<Employee>(employee);
			});
			mock.Setup(x => x.GetAdditionalColumnTypes(It.IsAny<AdditionalColumnTypeFilter>())).Returns(() =>
			{
				return new OperationResult<List<AdditionalColumnType>>();
			});
			mock.Setup(x => x.SaveEmployee(It.IsAny<Employee>(), It.IsAny<bool>())).Returns(() =>
			{
				return new OperationResult<bool>(true);
			});
			ClientManager.RubezhService = mock.Object;
			(ServiceFactory.DialogService as MockDialogService).OnShowModal += window => (window as EmployeeDetailsViewModel).SaveCommand.Execute();

			var employeesViewModel = new EmployeesViewModel();
			employeesViewModel.Initialize(new EmployeeFilter());
			employeesViewModel.SelectedItem = employeesViewModel.Organisations.FirstOrDefault().Children.FirstOrDefault();
			employeesViewModel.EditCommand.Execute();
			Assert.IsTrue(employeesViewModel.SelectedItem.DepartmentName == null || employeesViewModel.SelectedItem.DepartmentName == "");
			Assert.IsTrue(employeesViewModel.SelectedItem.PositionName == null || employeesViewModel.SelectedItem.PositionName == "");
		}
	}
}
