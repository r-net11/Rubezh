﻿using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		#region Get
		public OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Employee>>>(() => FiresecService.GetEmployees(filter));
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Position>>>(() => FiresecService.GetPositions(filter));
		}
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Department>>>(() => FiresecService.GetDepartments(filter));
		}
		public OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDJournalItem>>>(() => FiresecService.GetSKDJournalItems(filter));
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<SKDCard>>>(() => FiresecService.GetCards(filter));
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<CardZone>>>(() => FiresecService.GetCardZones(filter));
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Organization>>>(() => FiresecService.GetOrganizations(filter));
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Document>>>(() => FiresecService.GetDocuments(filter));
		}
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AccessTemplate>>>(() => FiresecService.GetAccessTemplates(filter));
		}
		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumnType>>>(() => FiresecService.GetAdditionalColumnTypes(filter));
		}
		public OperationResult<IEnumerable<AdditionalColumn>> GetAdditionalColumns(AdditionalColumnFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<AdditionalColumn>>>(() => FiresecService.GetAdditionalColumns(filter));
		}
		public OperationResult<IEnumerable<Photo>> GetPhotos(PhotoFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Photo>>>(() => FiresecService.GetPhotos(filter));
		}
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<EmployeeReplacement>>>(() => FiresecService.GetEmployeeReplacements(filter));
		}
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> Employees)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployees(Employees));
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardTemplate(card));
		}
		public OperationResult SaveDepartments(IEnumerable<Department> Departments)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDepartments(Departments));
		}
		public OperationResult SavePositions(IEnumerable<Position> Positions)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePositions(Positions));
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> SKDJournalItems)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSKDJournalItems(SKDJournalItems));
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCards(items));
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveCardZones(items));
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> Organizations)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganizations(Organizations));
		}
		public OperationResult SaveOrganizationZones(Organization organization)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveOrganizationZones(organization));
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDocuments(items));
		}
		public OperationResult SaveAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAccessTemplates(items));
		}
		public OperationResult SaveAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumnTypes(items));
		}
		public OperationResult SaveAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveAdditionalColumns(items));
		}
		public OperationResult SavePhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SavePhotos(items));
		}
		public OperationResult SaveEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveEmployeeReplacements(items));
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedEmployees(Employees));
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDepartments(Departments));
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> Positions)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPositions(Positions));
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedCards(items));
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedCardZones(items));
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> Organizations)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedOrganizations(Organizations));
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDocuments(items));
		}
		public OperationResult MarkDeletedAccessTemplates(IEnumerable<AccessTemplate> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAccessTemplates(items));
		}
		public OperationResult MarkDeletedAdditionalColumnTypes(IEnumerable<AdditionalColumnType> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumnTypes(items));
		}
		public OperationResult MarkDeletedAdditionalColumns(IEnumerable<AdditionalColumn> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedAdditionalColumns(items));
		}
		public OperationResult MarkDeletedPhotos(IEnumerable<Photo> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedPhotos(items));
		}
		public OperationResult MarkDeletedEmployeeReplacements(IEnumerable<EmployeeReplacement> items)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedEmployeeReplacements(items));
		}
		#endregion

		public OperationResult<SKDStates> SKDGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetStates(); }, "SKDGetStates");
		}
		public OperationResult<string> SKDGetDeviceInfo(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDGetDeviceInfo(deviceUID); }, "SKDGetDeviceInfo");
		}
		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDSyncronyseTime(deviceUID); }, "SKDSyncronyseTime");
		}
		public OperationResult<bool> SKDWriteConfiguration(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.SKDWriteConfiguration(deviceUID); }, "SKDWriteConfiguration");
		}
		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.SKDUpdateFirmware(deviceUID, fileName); }, "SKDUpdateFirmware");
		}
		public void SKDSetRegimeOpen(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeOpen(deviceUID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeClose(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeClose(deviceUID); }, "SKDSetIgnoreRegime");
		}
		public void SKDSetRegimeControl(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeControl(deviceUID); }, "SKDSetRegimeControl");
		}
		public void SKDSetRegimeConversation(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDSetRegimeConversation(deviceUID); }, "SKDSetRegimeConversation");
		}
		public void SKDOpenDevice(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDOpenDevice(deviceUID); }, "SKDOpenDevice");
		}
		public void SKDCloseDevice(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDCloseDevice(deviceUID); }, "SKDCloseDevice");
		}
		public void SKDAllowReader(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDAllowReader(deviceUID); }, "SKDAllowReader");
		}

		public void SKDDenyReader(Guid deviceUID)
		{
			SafeOperationCall(() => { FiresecService.SKDDenyReader(deviceUID); }, "SKDDenyReader");
		}
	}
}
