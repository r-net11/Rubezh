using System.Collections.Generic;
using FiresecAPI;
using SKDDriver;
using System;
using System.Linq;
using XFiresecAPI;

namespace FiresecService.Service
{
    public partial class FiresecService : IFiresecService
    {
		SKDDatabaseService _skdService = new SKDDatabaseService();
		
		#region IFiresecService Members
		
		#region Get
		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			return _skdService.GetEmployees(filter);
		}
		public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			return _skdService.GetDepartments(filter);
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return _skdService.GetPositions(filter);
		}
		public IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return _skdService.GetSKDJournalItems(filter);
		}
		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return _skdService.GetFrames(filter);
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return _skdService.GetCards(filter);
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return _skdService.GetCardZones(filter);
		}
		public IEnumerable<Organization> GetOrganizations(OrganizationFilter filter)
		{
			return _skdService.GetOrganizations(filter);
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return _skdService.GetDocuments(filter);
		}
		#endregion

		#region Save
		public void SaveEmployees(IEnumerable<Employee> Employees)
		{
			_skdService.SaveEmployees(Employees);
		}
		public void SaveDepartments(IEnumerable<Department> Departments)
		{
			_skdService.SaveDepartments(Departments);
		}
		public OperationResult SavePositions(IEnumerable<Position> Positions)
		{
			return _skdService.SavePositions(Positions);
		}
		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			_skdService.SaveSKDJournalItems(journalItems);
		}
		public void SaveFrames(IEnumerable<Frame> frames)
		{
			_skdService.SaveFrames(frames);
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return _skdService.SaveCards(items);
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return _skdService.SaveCardZones(items);
		}
		public void SaveOrganizations(IEnumerable<Organization> items)
		{
			_skdService.SaveOrganizations(items);
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return _skdService.SaveDocuments(items);
		}
		#endregion

		#region MarkDeleted
		public void MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			_skdService.MarkDeletedEmployees(Employees);
		}
		public void MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			_skdService.MarkDeletedDepartments(Departments);
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> Positions)
		{
			return _skdService.MarkDeletedPositions(Positions);
		}
		public void MarkDeletedSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			_skdService.MarkDeletedSKDJournalItems(journalItems);
		}
		public void MarkDeletedFrames(IEnumerable<Frame> frames)
		{
			_skdService.MarkDeletedFrames(frames);
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return _skdService.MarkDeletedCards(items);
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return _skdService.MarkDeletedCardZones(items);
		}
		public void MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			_skdService.MarkDeletedOrganizations(items);
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return _skdService.MarkDeletedDocuments(items);
		}
		#endregion
		#endregion

		#region Devices
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