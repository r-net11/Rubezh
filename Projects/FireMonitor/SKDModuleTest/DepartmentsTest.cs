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
			var mock = new Mock<ISafeFiresecService>();
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
			ClientManager.FiresecService = mock.Object;
		
			var departmentsViewModel = new DepartmentsViewModel();
			departmentsViewModel.Initialize(new DepartmentFilter());
			var detailsViewModel = new DepartmentDetailsViewModel();
			detailsViewModel.Initialize(organisation, newDepartment, departmentsViewModel);
			Assert.IsFalse(detailsViewModel.ValidateAndSave());
		}
	}
}