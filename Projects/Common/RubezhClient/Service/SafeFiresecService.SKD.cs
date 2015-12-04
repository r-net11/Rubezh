using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		#region Employee
		public OperationResult<List<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortEmployee>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeList(filter);
			});
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Employee>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeDetails(uid);
			});
		}
		public OperationResult<bool> SaveEmployee(Employee item, bool isNew)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployee(item, isNew);
			});
		}
		public OperationResult MarkDeletedEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedEmployee(uid, name, isEmployee);
			});
		}
		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracks(filter, startDate, endDate);
			});
		}
		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var result = SafeContext.Execute<Stream>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTracksStream(filter, startDate, endDate);
			});
			return result;
		}
		public OperationResult SaveEmployeeDepartment(Guid uid, Guid? departmentUid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeeDepartment(uid, departmentUid, name);
			});
		}
		public OperationResult SaveEmployeePosition(Guid uid, Guid? positionUid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveEmployeePosition(uid, positionUid, name);
			});
		}
		public OperationResult RestoreEmployee(Guid uid, string name, bool isEmployee)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreEmployee(uid, name, isEmployee);
			});
		}
		#endregion

		#region Department
		public OperationResult<List<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortDepartment>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentList(filter);
			});
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Department>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetDepartmentDetails(uid);
			});
		}
		public OperationResult<bool> SaveDepartment(Department item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartment(item, isNew);
			});
		}
		public OperationResult MarkDeletedDepartment(ShortDepartment item)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDepartment(item);
			});
		}
		public OperationResult SaveDepartmentChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveDepartmentChief(uid, chiefUID, name);
			});
		}
		public OperationResult RestoreDepartment(ShortDepartment item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreDepartment(item);
			});
		}
		public OperationResult<List<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetChildEmployeeUIDs(uid);
			});
		}
		public OperationResult<List<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetParentEmployeeUIDs(uid);
			});
		}
		#endregion

		#region Position
		public OperationResult<List<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortPosition>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPositionList(filter);
			});
		}

		public OperationResult<List<Guid>> GetPositionEmployees(Guid uid)
		{
			return SafeContext.Execute<OperationResult<List<Guid>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPositionEmployees(uid);
			});
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<Position>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPositionDetails(uid);
			});
		}
		public OperationResult<bool> SavePosition(Position item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SavePosition(item, isNew);
			});
		}
		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPosition(uid, name);
			});
		}
		public OperationResult RestorePosition(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestorePosition(uid, name);
			});
		}
		#endregion

		#region Journal
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetMinJournalDateTime();
			});
		}
		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<JournalItem>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetFilteredJournalItems(filter);
			});
		}
		public OperationResult<bool> BeginGetJournal(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.BeginGetJournal(filter);
			});
		}
		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddJournalItem(journalItem);
			});
		}
		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.BeginGetArchivePage(filter, page);
			});
		}
		public OperationResult<int> GetArchiveCount(JournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<int>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetArchiveCount(filter);
			});
		}
		#endregion

		#region Card
		public OperationResult<List<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<SKDCard>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetCards(filter);
			});
		}
		public OperationResult<SKDCard> GetSingleCard(Guid uid)
		{
			return SafeContext.Execute<OperationResult<SKDCard>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetSingleCard(uid);
			});
		}
		public OperationResult<List<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			return SafeContext.Execute<OperationResult<List<SKDCard>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetEmployeeCards(employeeUID);
			});
		}
		public OperationResult<bool> AddCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddCard(item, employeeName);
			});
		}
		public OperationResult<bool> EditCard(SKDCard item, string employeeName)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.EditCard(item, employeeName);
			});
		}
		public OperationResult<bool> DeleteCardFromEmployee(SKDCard item, string employeeName, string reason = null)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeleteCardFromEmployee(item, employeeName, reason);
			});
		}
		public OperationResult DeletedCard(SKDCard card)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeletedCard(card);
			});
		}
		public OperationResult SaveCardTemplate(SKDCard item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveCardTemplate(item);
			});
		}
		#endregion

		#region AccessTemplate
		public OperationResult<List<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<AccessTemplate>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetAccessTemplates(filter);
			});
		}
		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveAccessTemplate(item, isNew);
			});
		}
		public OperationResult MarkDeletedAccessTemplate(AccessTemplate item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAccessTemplate(item);
			});
		}
		public OperationResult RestoreAccessTemplate(AccessTemplate item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreAccessTemplate(item);
			});
		}
		#endregion

		#region Organisation
		public OperationResult<List<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<Organisation>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisations(filter);
			});
		}
		public OperationResult<bool> SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisation(item, isNew);
			});
		}
		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedOrganisation(uid, name);
			});
		}
		public OperationResult AddOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddOrganisationDoor(item, doorUID);
			});
		}
		public OperationResult RemoveOrganisationDoor(Organisation item, Guid doorUID)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RemoveOrganisationDoor(item, doorUID);
			});
		}
		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationUsers(item);
			});
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<OrganisationDetails>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetOrganisationDetails(uid);
			});
		}
		public OperationResult SaveOrganisationChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationChief(uid, chiefUID, name);
			});
		}
		public OperationResult SaveOrganisationHRChief(Guid uid, Guid? chiefUID, string name)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveOrganisationHRChief(uid, chiefUID, name);
			});
		}
		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreOrganisation(uid, name);
			});
		}
		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.IsAnyOrganisationItems(uid);
			});
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<List<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<AdditionalColumnType>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetAdditionalColumnTypes(filter);
			});
		}
		public OperationResult<bool> SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveAdditionalColumnType(item, isNew);
			});
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedAdditionalColumnType(uid, name);
			});
		}
		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreAdditionalColumnType(uid, name);
			});
		}
		#endregion

		#region NightSettings
		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<NightSettings>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetNightSettingsByOrganisation(organisationUID);
			});
		}
		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveNightSettings(nightSettings);
			});
		}
		#endregion

		#region PassCardTemplate
		public OperationResult<List<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<List<ShortPassCardTemplate>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateList(filter);
			});
		}
		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			return SafeContext.Execute<OperationResult<PassCardTemplate>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPassCardTemplateDetails(uid);
			});
		}
		public OperationResult<bool> SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SavePassCardTemplate(item, isNew);
			});
		}
		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedPassCardTemplate(uid, name);
			});
		}
		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestorePassCardTemplate(uid, name);
			});
		}
		#endregion

		public OperationResult GenerateEmployeeDays()
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GenerateEmployeeDays();
			});
		}

		public OperationResult GenerateJournal()
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GenerateJournal();
			});
		}

		public OperationResult GenerateTestData(bool isAscending)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GenerateTestData(isAscending);
			});
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalVideoUID(journalItemUID, videoUID, cameraUID);
			});
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveJournalCameraUID(journalItemUID, CameraUID);
			});
		}

		#region GKSchedule
		public OperationResult<List<GKSchedule>> GetGKSchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKSchedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetGKSchedules();
			});
		}

		public OperationResult<bool> SaveGKSchedule(GKSchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveGKSchedule(item, isNew);
			});
		}

		public OperationResult<bool> DeleteGKSchedule(GKSchedule item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKSchedule(item);
			});
		}
		#endregion

		#region GKDaySchedule
		public OperationResult<List<GKDaySchedule>> GetGKDaySchedules()
		{
			return SafeContext.Execute<OperationResult<List<GKDaySchedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetGKDaySchedules();
			});
		}

		public OperationResult<bool> SaveGKDaySchedule(GKDaySchedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveGKDaySchedule(item, isNew);
			});
		}

		public OperationResult<bool> DeleteGKDaySchedule(GKDaySchedule item)
		{
			return SafeContext.Execute<OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeleteGKDaySchedule(item);
			});
		}
		#endregion

		#region Export
		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisation(filter);
			});
		}
		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisation(filter);
			});
		}
		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ExportOrganisationList(filter);
			});
		}
		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ImportOrganisationList(filter);
			});
		}
		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ExportJournal(filter);
			});
		}
		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.ExportConfiguration(filter);
			});
		}
		#endregion

		#region CurrentConsumption
		public OperationResult SaveCurrentConsumption(CurrentConsumption item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveCurrentConsumption(item);
			});
		}
		public OperationResult<List<CurrentConsumption>> GetCurrentConsumption(CurrentConsumptionFilter item)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetCurrentConsumption(item);
			});
		}
		#endregion
	}
}