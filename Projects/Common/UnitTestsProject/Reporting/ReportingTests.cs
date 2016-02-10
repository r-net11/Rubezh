using System.Collections.Generic;
using FiresecService.Report.Helpers;
using FiresecService.Report.Templates;
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
				"Сведения о пропусках",
				"Список подразделений организации",
				"Дисциплинарный отчет",
				"Отчет по оправдательным документам",
				"Список точек доступа",
				"Доступ в зоны сотрудников",
				"Права доступа сотрудников",
				"Справка о сотруднике",
				"Маршрут сотрудника",
				"Местонахождение сотрудников",
				"Отчет по событиям системы контроля доступа",
				"Список должностей организации",
				"Отчет по графикам работы",
				"Справка по отработанному времени"
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
