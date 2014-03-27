using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		#region Get
		public OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter)
		{
			return SKDDatabaseService.EmployeeTranslator.Get(filter);
		}
		public OperationResult<EmployeeDetails> GetEmployeeDetails(Guid uid)
		{
			return SKDDatabaseService.EmployeeTranslator.GetDetails(uid);
		}
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return SKDDatabaseService.DepartmentTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return SKDDatabaseService.PositionTranslator.Get(filter);
		}
		public  OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SKDDatabaseService.JournalItemTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SKDDatabaseService.CardTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return SKDDatabaseService.CardZoneTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return SKDDatabaseService.OrganizationTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return SKDDatabaseService.DocumentTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter)
		{
			return SKDDatabaseService.AdditionalColumnTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter)
		{
			return SKDDatabaseService.PhotoTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Get(filter);
		}
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> Employees)
		{
			return SKDDatabaseService.EmployeeTranslator.Save(Employees);
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			return SKDDatabaseService.CardTranslator.SaveTemplate(card);
		}
		public OperationResult SaveDepartments(IEnumerable<Department> Departments)
		{
			return SKDDatabaseService.DepartmentTranslator.Save(Departments);
		}
		public OperationResult SavePositions(IEnumerable<Position> Positions)
		{
			return SKDDatabaseService.PositionTranslator.Save(Positions);
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			return SKDDatabaseService.JournalItemTranslator.Save(journalItems);
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return SKDDatabaseService.CardTranslator.Save(items);
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return SKDDatabaseService.CardZoneTranslator.Save(items);
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> Organizations)
		{
			return SKDDatabaseService.OrganizationTranslator.Save(Organizations);
		}
		public OperationResult SaveOrganizationZones(Organization organization)
		{
			return SKDDatabaseService.OrganizationTranslator.SaveZones(organization);
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return SKDDatabaseService.DocumentTranslator.Save(items);
		}
		public OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Save(items);
		}
		public OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.Save(items);
		}
		public OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SKDDatabaseService.AdditionalColumnTranslator.Save(items);
		}
		public OperationResult SavePhotos(IEnumerable<Photo> items)
		{
			return SKDDatabaseService.PhotoTranslator.Save(items);
		}
		public OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Save(items);
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			return SKDDatabaseService.EmployeeTranslator.MarkDeleted(Employees);
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			return SKDDatabaseService.DepartmentTranslator.MarkDeleted(Departments);
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> Positions)
		{
			return SKDDatabaseService.PositionTranslator.MarkDeleted(Positions);
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return SKDDatabaseService.CardTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return SKDDatabaseService.CardZoneTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> Organizations)
		{
			return SKDDatabaseService.OrganizationTranslator.MarkDeleted(Organizations);
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return SKDDatabaseService.DocumentTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SKDDatabaseService.AccessTemplateTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SKDDatabaseService.AdditionalColumnTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedPhotos(IEnumerable<Photo> items)
		{
			return SKDDatabaseService.PhotoTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.MarkDeleted(items);
		}
		#endregion

		#region Devices
		public OperationResult<SKDStates> SKDGetStates()
		{
			return new OperationResult<SKDStates>() { Result = SKDProcessorManager.SKDGetStates() };
		}

		public OperationResult<string> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x=>x.UID == deviceUID);
			if(device != null)
			{
				return new OperationResult<string>() { Result = SKDProcessorManager.SKDGetDeviceInfo(device, UserName) };
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<bool>() { Result = SKDProcessorManager.SKDSyncronyseTime(device, UserName) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.GKWriteConfiguration(device, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public void SKDSetRegimeOpen(Guid deviceUID)
		{

		}

		public void SKDSetRegimeClose(Guid deviceUID)
		{

		}

		public void SKDSetRegimeControl(Guid deviceUID)
		{

		}

		public void SKDSetRegimeConversation(Guid deviceUID)
		{

		}

		public void SKDOpenDevice(Guid deviceUID)
		{

		}

		public void SKDCloseDevice(Guid deviceUID)
		{

		}

		public void SKDAllowReader(Guid deviceUID)
		{

		}

		public void SKDDenyReader(Guid deviceUID)
		{

		}
		#endregion
	}
}