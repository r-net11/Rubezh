using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using System.Collections.Generic;

namespace FiresecService.Report.Templates
{
	public partial class Report412 : BaseSKDReport
	{
		public Report412()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Доступ в зоны сотрудников/посетителей"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter412>();
			var databaseService = new SKDDatabaseService();

			var employees = new List<Employee>();
			var useEmployeesFilter = false;
			if (filter.Employees == null)
				filter.Employees = new List<Guid>();
			if (filter.Departments == null)
				filter.Departments = new List<Guid>();
			if (filter.Positions == null)
				filter.Positions = new List<Guid>();
			if (filter.Organisations == null)
				filter.Organisations = new List<Guid>();
			if (filter.Employees.Count > 0 || filter.Departments.Count > 0 || filter.Positions.Count > 0 || filter.Organisations.Count > 0)
			{
				useEmployeesFilter = true;
				var employeeFilter = new EmployeeFilter();
				employeeFilter.OrganisationUIDs = filter.Organisations;
				employeeFilter.DepartmentUIDs = filter.Departments;
				employeeFilter.PositionUIDs = filter.Positions;
				employeeFilter.UIDs = filter.Employees;
				var employeesResult = databaseService.EmployeeTranslator.Get(employeeFilter);
				employees = employeesResult.Result.ToList();
			}

			var cardFilter = new CardFilter();
			var cardsResult = databaseService.CardTranslator.Get(cardFilter);

			var dataSet = new DataSet412();
			if (!cardsResult.HasError)
			{
				foreach (var card in cardsResult.Result)
				{
					if (useEmployeesFilter)
						if (!employees.Any(x => x.UID == card.EmployeeUID))
							continue;

					if (filter.PassCardPermanent || filter.PassCardTemprorary || filter.PassCardOnceOnly || filter.PassCardForcing || filter.PassCardLocked)
					{
						if (filter.PassCardPermanent && card.CardType != CardType.Constant)
							continue;
						if (filter.PassCardTemprorary && card.CardType != CardType.Temporary)
							continue;
						if (filter.PassCardOnceOnly && card.CardType != CardType.OneTime)
							continue;
						if (filter.PassCardForcing && card.CardType != CardType.Duress)
							continue;
						if (filter.PassCardLocked && card.CardType != CardType.Blocked)
							continue;
					}

					if (filter.PassCardActive && card.IsDeleted)
						continue;

					var employeeResult = databaseService.EmployeeTranslator.GetSingle(card.EmployeeUID);
					var accessTemplateResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);

					var cardDoors = new List<CardDoor>();
					if (accessTemplateResult.Result != null)
					{
						cardDoors = accessTemplateResult.Result.CardDoors.ToList();
					}
					cardDoors.AddRange(card.CardDoors.ToList());

					var zones = new HashSet<SKDZone>();
					foreach (var cardDoor in cardDoors)
					{
						var door = SKDManager.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
						if (door != null)
						{
							if (door.InDevice != null && door.InDevice.Zone != null)
								zones.Add(door.InDevice.Zone);
							if (door.OutDevice != null && door.OutDevice.Zone != null)
								zones.Add(door.OutDevice.Zone);
						}
					}

					foreach (var zone in zones)
					{
						if (filter.Zones != null && filter.Zones.Count > 0)
						{
							if (!filter.Zones.Contains(zone.UID))
							{
								continue;
							}
						}

						var dataRow = dataSet.Data.NewDataRow();
						dataRow.Type = card.CardType.ToDescription();
						dataRow.Number = card.Number.ToString();
						if (employeeResult.Result != null)
						{
							dataRow.Employee = employeeResult.Result.Name;
							var organisationResult = databaseService.OrganisationTranslator.GetSingle(employeeResult.Result.OrganisationUID);
							if (organisationResult.Result != null)
							{
								dataRow.Organisation = organisationResult.Result.Name;
							}
							if (employeeResult.Result.Department != null)
							{
								dataRow.Department = employeeResult.Result.Department.Name;
							}
							if (employeeResult.Result.Position != null)
							{
								dataRow.Position = employeeResult.Result.Position.Name;
							}
						}
						if (accessTemplateResult.Result != null)
						{
							dataRow.Template = accessTemplateResult.Result.Name;
						}
						dataRow.Zone = zone.PresentationName;

						dataSet.Data.Rows.Add(dataRow);
					}
				}
			}
			return dataSet;
		}

		//protected override void UpdateDataSource()
		//{
		//    base.UpdateDataSource();
		//    FillTestData();
		//}
	}
}