using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using StrazhDAL;

namespace FiresecService.Report.Operations
{
	public abstract class BaseReportSearchOperation
	{
		private readonly SKDDatabaseService _dataService;

		protected BaseReportSearchOperation(SKDDatabaseService service)
		{
			if(service == null)
				throw new ArgumentNullException("service");

			_dataService = service;
		}

		protected CardTemplatePrintData CreateDataSetItem(Employee employee)
		{
			var printData = new CardTemplatePrintData();

			var card = employee.Cards.FirstOrDefault();
			if (card != null)
				printData.CardNo = card.Number;

			if (employee.Department != null)
			{
				var department = _dataService.DepartmentTranslator.GetSingle(employee.Department.UID).Result;

				if (department != null)
				{
					printData.DepartmentLogo = department.Photo.Data;
					printData.DepartmentName = department.Name;
				}
			}

			printData.Description = employee.Description;

			var escortEmployeeOperation = _dataService.EmployeeTranslator.GetSingle(employee.EscortUID);
			var escortEmployee = escortEmployeeOperation.Result;

			if (escortEmployee != null)
			{
				printData.ExcortName = escortEmployee.Name;
			}
			printData.FirstName = employee.FirstName;
			printData.LastName = employee.LastName;
			printData.MiddleName = employee.SecondName;

			var organisationOperation = _dataService.OrganisationTranslator.GetSingle(employee.OrganisationUID);
			var organisation = organisationOperation.Result;

			if (organisation != null)
			{
				var organisationPhoto = _dataService.PhotoTranslator.GetSingle(organisation.PhotoUID).Result;
				if (organisationPhoto != null)
					printData.OrganisationLogo = organisationPhoto.Data;

				printData.OrganisationName = organisation.Name;
			}

			printData.Phone = employee.Phone;

			if(employee.Photo != null)
				printData.Photo = employee.Photo.Data;

			if (employee.Position != null)
			{
				var position = _dataService.PositionTranslator.GetSingle(employee.Position.UID).Result;
				if (position != null)
				{
					printData.PositionLogo = position.Photo.Data;
					printData.PositionName = position.Name;
				}
			}

			printData.ScheduleName = employee.ScheduleName;
			printData.TabelNo = employee.TabelNo;

			return printData;
		}

		public List<ReportDTO> Result { get; protected set; }

		public abstract void Execute();
	}
}
