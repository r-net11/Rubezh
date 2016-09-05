using System.Collections.Generic;
using FiresecService.Report.Helpers;
using FiresecService.Report.Templates;
using Localization.FiresecService.Report.Common;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace UnitTestsProject.Reporting
{
	[TestFixture]
	public class ReportingTests
	{
		[Test]
		public void GetListOfExistingReports()
		{
			//Arrange
			var testCollection = new List<string>
			{
				CommonResources.PasscardsInfo,//"Сведения о пропусках",
				CommonResources.DepartList,//"Список подразделений организации",
				CommonResources.DisciplinaryReport,//"Дисциплинарный отчет",
				CommonResources.DocumentsReport,//"Отчет по оправдательным документам",
				CommonResources.DoorsList,//"Список точек доступа",
				CommonResources.AccessZone+ CommonResources.Employees,//"Доступ в зоны сотрудников",
				CommonResources.AccessPermissions + CommonResources.Employees,//"Права доступа сотрудников",
				CommonResources.InfoAboutEmployee,//"Справка о сотруднике",
				CommonResources.Route + CommonResources.EmployeeNo,//"Маршрут сотрудника",
				CommonResources.Location + CommonResources.EmployeeNo,//"Местонахождение сотрудников",
				CommonResources.SystemEventsReport,//"Отчет по событиям системы контроля доступа",
				CommonResources.PositionList,//"Список должностей организации",
				CommonResources.WorkScheduleReport,//"Отчет по графикам работы",
				CommonResources.InfoAboutWorkTime//"Справка по отработанному времени"
			}.OrderBy(x => x)
			.ToList();

			//Act
			var result = ReportingHelpers.GetReportNames().OrderBy(x => x).ToList();


			//Assert
			Assert.AreEqual(result.Count, 14);
			Assert.True(result.SequenceEqual(testCollection));
		}
	}
}
