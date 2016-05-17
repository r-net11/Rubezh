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
	public class DepartmentsTest
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
			ClientManager.CurrentUser.PermissionStrings.Add("Oper_SKD_Departments_Etit");

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
		}

		[Test]
		public void AddSameNameTest()
		{
			var organisation = new Organisation();
			var department = new ShortDepartment { Name = "Name", OrganisationUID = organisation.UID };
			var newDepartment = new ShortDepartment { Name = "Name", OrganisationUID = organisation.UID };
			var mock = new Mock<ISafeRubezhService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns<OrganisationFilter>(filter =>
			{
				return new OperationResult<List<Organisation>>(new List<Organisation> { organisation });
			});
			mock.Setup(x => x.GetDepartmentList(It.IsAny<DepartmentFilter>())).Returns<DepartmentFilter>(filter =>
			{
				return new OperationResult<List<ShortDepartment>>(new List<ShortDepartment> { department });
			});
			mock.Setup(x => x.GetDepartmentDetails(It.IsAny<Guid>())).Returns<Guid>(uid =>
			{
				if (uid == department.UID)
					return new OperationResult<Department>(new Department { UID = department.UID, OrganisationUID = department.UID, Name = department.Name });
				return null;
			});
			mock.Setup(x => x.GetEmployeeList(It.IsAny<EmployeeFilter>())).Returns(() =>
			{
				return new OperationResult<List<ShortEmployee>>();
			});
			ClientManager.RubezhService = mock.Object;
			
		
			var departmentsViewModel = new DepartmentsViewModel();
			departmentsViewModel.Initialize(new DepartmentFilter());
			var detailsViewModel = new DepartmentDetailsViewModel();
			detailsViewModel.Initialize(organisation, newDepartment, departmentsViewModel);
			Assert.IsFalse(detailsViewModel.ValidateAndSave());
		}

		[Test]
		public void DeleteChild()
		{
			var organisation = new Organisation();
			var department = new ShortDepartment { UID = Guid.NewGuid(), Name = "Name1", OrganisationUID = organisation.UID };
			var childDepartment = new ShortDepartment 
				{ 
					UID = Guid.NewGuid(),
					Name = "Name2", 
					OrganisationUID = organisation.UID, 
					ParentDepartmentUID = department.UID, 
					IsDeleted = true 
				};
			department.ChildDepartments.Add(new TinyDepartment { Name = childDepartment.Name, UID = childDepartment.UID });
			var mock = new Mock<ISafeRubezhService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns(() =>
			{
				return new OperationResult<List<Organisation>>(new List<Organisation> { organisation });
			});
			mock.Setup(x => x.GetDepartmentList(It.IsAny<DepartmentFilter>())).Returns(() =>
			{
				return new OperationResult<List<ShortDepartment>>(new List<ShortDepartment> { department, childDepartment });
			});
			mock.Setup(x => x.GetEmployeeList(It.IsAny<EmployeeFilter>())).Returns(() =>
			{
				return new OperationResult<List<ShortEmployee>>();
			});
			mock.Setup(x => x.RestoreDepartment(It.IsAny<ShortDepartment>())).Returns<ShortDepartment>(shortDeaprtment =>
			{
				if (shortDeaprtment.UID == department.UID)
					department.IsDeleted = false;
				if (shortDeaprtment.UID == childDepartment.UID)
					childDepartment.IsDeleted = false;
				return new OperationResult<bool>(true);
			});
			mock.Setup(x => x.GetParentEmployeeUIDs(It.IsAny<Guid>())).Returns<Guid>(uid =>
			{
				var result = new List<Guid>();
				if(uid == childDepartment.UID)
					result.Add(department.UID);
				return new OperationResult<List<Guid>>(result);
			});
			ClientManager.RubezhService = mock.Object;
			(ServiceFactory.MessageBoxService as MockMessageBoxService).ShowConfirmationResult = true;
			var departmentsViewModel = new DepartmentsViewModel();
			departmentsViewModel.Initialize(new DepartmentFilter());
			departmentsViewModel.SelectedItem = departmentsViewModel.Organisations.SelectMany(x  => x.GetAllChildren()).FirstOrDefault(x => x.UID == childDepartment.UID);
			if (departmentsViewModel.RestoreCommand.CanExecute(null))
				departmentsViewModel.RestoreCommand.ForceExecute();
			else
				Assert.IsTrue(false);
			departmentsViewModel.Initialize(new DepartmentFilter());
			var department2 = departmentsViewModel.Models.FirstOrDefault(x => x.UID == department.UID);
			var childDepartment2 = departmentsViewModel.Models.FirstOrDefault(x => x.UID == childDepartment.UID);
			Assert.IsFalse(department2.IsDeleted || childDepartment2.IsDeleted);
		}
	}
}