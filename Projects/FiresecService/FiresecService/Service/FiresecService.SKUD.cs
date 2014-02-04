using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models.SKDDatabase;
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
        public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
			return _skdService.GetEmployees(filter);
        }
        public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
			return _skdService.GetDepartments(filter);
        }
        public IEnumerable<Position> GetPositions(PositionFilter filter)
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
		public IEnumerable<Card> GetCards(CardFilter filter)
		{
			return _skdService.GetCards(filter);
		}
		public IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter)
		{
			return _skdService.GetCardZoneLinks(filter);
		}
		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			_skdService.SaveSKDJournalItems(journalItems);
		}
		public void SaveFrames(IEnumerable<Frame> frames)
		{
			_skdService.SaveFrames(frames);
		}
		public void SaveCards(IEnumerable<Card> items)
		{
			_skdService.SaveCards(items);
		}
		public void SaveCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			_skdService.SaveCardZoneLinks(items);
		}

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

		public void SKDSetIgnoreRegime(Guid deviceUID)
		{

		}

		public void SKDResetIgnoreRegime(Guid deviceUID)
		{

		}

		public void SKDOpenDevice(Guid deviceUID)
		{

		}

		public void SKDCloseDevice(Guid deviceUID)
		{

		}

		public void SKDExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit)
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