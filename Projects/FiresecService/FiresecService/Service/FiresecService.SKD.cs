using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		#region Employee
		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			return SKDDatabaseService.EmployeeTranslator.GetList(filter);
		}
		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			return SKDDatabaseService.EmployeeTranslator.GetSingle(uid);
		}
		public OperationResult SaveEmployee(Employee item)
		{
			return SKDDatabaseService.EmployeeTranslator.Save(item);
		}
		public OperationResult MarkDeletedEmployee(Guid uid)
		{
			return SKDDatabaseService.EmployeeTranslator.MarkDeleted(uid);
		}
		public OperationResult<EmployeeTimeTrack> GetEmployeeTimeTrack(Guid employeeUID, DateTime date)
		{
			return SKDDatabaseService.EmployeeTranslator.GetTimeTrack(employeeUID, date);
		}
		#endregion

		#region Department
		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			return SKDDatabaseService.DepartmentTranslator.GetList(filter);
		}
		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			return SKDDatabaseService.DepartmentTranslator.GetSingle(uid);
		}
		public OperationResult SaveDepartment(Department item)
		{
			return SKDDatabaseService.DepartmentTranslator.Save(item);
		}
		public OperationResult MarkDeletedDepartment(Guid uid)
		{
			return SKDDatabaseService.DepartmentTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Position
		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			return SKDDatabaseService.PositionTranslator.GetList(filter);
		}
		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			return SKDDatabaseService.PositionTranslator.GetSingle(uid);
		}
		public OperationResult SavePosition(Position item)
		{
			return SKDDatabaseService.PositionTranslator.Save(item);
		}
		public OperationResult MarkDeletedPosition(Guid uid)
		{
			return SKDDatabaseService.PositionTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Journal
		public OperationResult<IEnumerable<JournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SKDDatabaseService.JournalItemTranslator.Get(filter);
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<JournalItem> journalItems)
		{
			return SKDDatabaseService.JournalItemTranslator.Save(journalItems);
		}
		#endregion

		#region Card
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return SKDDatabaseService.CardTranslator.Get(filter);
		}
		public OperationResult AddCard(SKDCard item)
		{
			return SKDDatabaseService.CardTranslator.Save(item);
		}
		public OperationResult EditCard(SKDCard item)
		{
			return SKDDatabaseService.CardTranslator.Save(item);
		}
		public OperationResult DeleteCardFromEmployee(SKDCard item, string reason = null)
		{
			item.AccessTemplateUID = null;
			item.CardDoors = new List<CardDoor>();
			item.CardTemplateUID = null;
			item.EmployeeName = null;
			item.HolderUID = null;
			item.IsInStopList = true;
			item.StopReason = reason;
			item.StartDate = DateTime.Now;
			item.EndDate = DateTime.Now;
			return SKDDatabaseService.CardTranslator.Save(item);
		}

		public OperationResult MarkDeletedCard(Guid uid)
		{
			return SKDDatabaseService.CardTranslator.MarkDeleted(uid);
		}
		public OperationResult SaveCardTemplate(SKDCard card)
		{
			return SKDDatabaseService.CardTranslator.SaveTemplate(card);
		}
		#endregion

		#region AccessTemplate
		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Get(filter);
		}
		public OperationResult SaveAccessTemplate(AccessTemplate item)
		{
			return SKDDatabaseService.AccessTemplateTranslator.Save(item);
		}
		public OperationResult MarkDeletedAccessTemplate(Guid uid)
		{
			return SKDDatabaseService.AccessTemplateTranslator.MarkDeleted(uid);
		}
		#endregion

		#region Organisation
		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			return SKDDatabaseService.OrganisationTranslator.Get(filter);
		}
		public OperationResult SaveOrganisation(OrganisationDetails item)
		{
			return SKDDatabaseService.OrganisationTranslator.Save(item);
		}
		public OperationResult MarkDeletedOrganisation(Guid uid)
		{
			return SKDDatabaseService.OrganisationTranslator.MarkDeleted(uid);
		}
		public OperationResult SaveOrganisationDoors(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveDoors(organisation);
		}
		public OperationResult SaveOrganisationZones(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveZones(organisation);
		}
		public OperationResult SaveOrganisationGuardZones(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveGuardZones(organisation);
		}
		public OperationResult SaveOrganisationUsers(Organisation organisation)
		{
			return SKDDatabaseService.OrganisationTranslator.SaveUsers(organisation);
		}
		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			return SKDDatabaseService.OrganisationTranslator.GetDetails(uid);
		}
		#endregion

		#region Document
		public OperationResult<IEnumerable<ShortDocument>> GetDocumentList(DocumentFilter filter)
		{
			return SKDDatabaseService.DocumentTranslator.GetList(filter);
		}
		public OperationResult<Document> GetDocumentDetails(Guid uid)
		{
			return SKDDatabaseService.DocumentTranslator.GetSingle(uid);
		}
		public OperationResult SaveDocument(Document item)
		{
			return SKDDatabaseService.DocumentTranslator.Save(item);
		}
		public OperationResult MarkDeletedDocument(Guid uid)
		{
			return SKDDatabaseService.DocumentTranslator.MarkDeleted(uid);
		}
		#endregion

		#region AdditionalColumnType
		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.GetList(filter);
		}
		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
		}
		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.Save(item);
		}
		public OperationResult MarkDeletedAdditionalColumnType(Guid uid)
		{
			return SKDDatabaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
		}
		#endregion

		#region EmployeeReplacement
		public OperationResult<IEnumerable<EmployeeReplacement>> GetEmployeeReplacements(EmployeeReplacementFilter filter)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Get(filter);
		}
		public OperationResult SaveEmployeeReplacement(EmployeeReplacement item)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.Save(item);
		}
		public OperationResult MarkDeletedEmployeeReplacement(Guid uid)
		{
			return SKDDatabaseService.EmployeeReplacementTranslator.MarkDeleted(uid);
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
				//return new OperationResult<string>() { Result = SKDProcessorManager.SKDGetDeviceInfo(device, UserName) };
				return new OperationResult<string>() { Result = ChinaSKDDriver.Processor.GetdeviceInfo(deviceUID) };
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSyncronyseTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				//return new OperationResult<bool>() { Result = SKDProcessorManager.SKDSyncronyseTime(device, UserName) };
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SyncronyseTime(deviceUID) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<string> SKDGetPassword(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<string>() { Result = ChinaSKDDriver.Processor.GetPassword(deviceUID) };
			}
			return new OperationResult<string>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetPassword(Guid deviceUID, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<bool>() { Result = ChinaSKDDriver.Processor.SetPassword(deviceUID, password) };
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.SKDWriteConfiguration(device, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.SKDUpdateFirmware(device, fileName, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDWriteAllIdentifiers(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return SKDProcessorManager.SKDWriteAllIdentifiers(device, UserName);
			}
			return new OperationResult<bool>("Устройство не найдено в конфигурации");
		}

		public OperationResult<bool> SKDSetRegimeOpen(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 1);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDSetRegimeClose(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 2);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDSetRegimeControl(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 3);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDSetRegimeConversation(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 4);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 5);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDCloseDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 6);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDAllowReader(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 7);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public OperationResult<bool> SKDDenyReader(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				SKDProcessorManager.SendControlCommand(device, 8);
				return new OperationResult<bool>() { Result = true };
			}
			else
			{
				return new OperationResult<bool>("Устройство не найдено в конфигурации");
			}
		}

		public void BeginGetSKDFilteredArchive(SKDArchiveFilter archiveFilter)
		{
			if (CurrentThread != null)
			{
				SKDDBHelper.IsAbort = true;
				CurrentThread.Join(TimeSpan.FromMinutes(1));
				CurrentThread = null;
			}
			SKDDBHelper.IsAbort = false;
			var thread = new Thread(new ThreadStart((new Action(() =>
			{
				SKDDBHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
				SKDDBHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
				SKDDBHelper.BeginGetSKDFilteredArchive(archiveFilter, false);

			}))));
			thread.Name = "SKD GetFilteredArchive";
			thread.IsBackground = true;
			CurrentThread = thread;
			thread.Start();
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems)
		{
			FiresecService.NotifySKDArchiveCompleted(journalItems);
		}
		#endregion
	}
}