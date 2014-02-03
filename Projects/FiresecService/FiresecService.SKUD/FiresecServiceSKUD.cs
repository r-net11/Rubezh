using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using System;
using XFiresecAPI;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
        DataAccess.SKUDDataContext Context;
        
        public FiresecServiceSKUD()
        {
            Context = new DataAccess.SKUDDataContext();
			var items = GetSKDJournalItems(null);
        }


		#region IFiresecServiceSKUD Members

		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
            try
            {
                var employees = new List<Employee>();
                var databaseEmployees = Context.Employee.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databaseEmployees.ForEach(x => employees.Add(Translator.Translate(x)));
                return employees;
            }
            catch { return new List<Employee>(); }
        }

        public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
            try
            {
                var departments = new List<Department>();
                var databaseDepartments = Context.Department.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databaseDepartments.ForEach(x => departments.Add(Translator.Translate(x)));
                return departments;
            }
            catch { return new List<Department>(); }
        }
        
        public IEnumerable<Position> GetPositions(PositionFilter filter)
        {
            try
            {
                var positions = new List<Position>();
                var databasePositions = Context.Position.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databasePositions.ForEach(x => positions.Add(Translator.Translate(x)));
                return positions;
            }
            catch { return new List<Position>(); }
        }

		public IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			try
			{
				var journalItems = new List<SKDJournalItem>();
				var databaseJournalItems = Context.Journal.ToList().Where(x => IsInFilter(x, filter)).ToList();
				databaseJournalItems.ForEach(x => journalItems.Add(Translator.Translate(x)));
				return journalItems;
			}
			catch { return new List<SKDJournalItem>(); }
		}

		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			try
			{
				foreach (var item in journalItems)
				{
					if (item != null)
						Context.Journal.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}


        #endregion

        bool IsInFilter(FiresecService.SKUD.DataAccess.Employee employee, EmployeeFilter filter)
        {
            if (filter == null)
                return true;

			bool isInUids = IsInUidList(employee.Uid, filter.Uids);
			bool isInDepartments = IsInUidList(employee.DepartmentUid, filter.DepartmentUids);
			bool isInPositions = IsInUidList(employee.PositionUid, filter.PositionUids);
			bool isInAppointed = IsInDateTimePeriod(employee.Appointed, filter.Appointed);
			bool isInDismissed = IsInDateTimePeriod(employee.Dismissed, filter.Dismissed);	
            return isInUids && isInDepartments && isInPositions && isInAppointed && isInDepartments;
        }

		bool IsInFilter(FiresecService.SKUD.DataAccess.Journal item, SKDJournalFilter filter)
		{
			if (filter == null)
				return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);
			bool isInSystemDate = IsInDateTimePeriod(item.SysemDate, filter.SystemDateTime);
			bool isInDeviceDate = IsInDateTimePeriod(item.DeviceDate, filter.DeviceDateTime);
			return isInUids && isInSystemDate && isInDeviceDate;
		}

        bool IsInFilter(FiresecService.SKUD.DataAccess.Department item, DepartmentFilter filter)
        {
            if (filter == null)
                return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);

            return isInUids;
        }

        bool IsInFilter(FiresecService.SKUD.DataAccess.Position item, PositionFilter filter)
        {
            if (filter == null)
                return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);

            return isInUids;
        }

		bool IsInDateTimePeriod(DateTime? dateTime, DateTimePeriod dateTimePeriod)
		{
			if (dateTimePeriod == null)
				return true;
			return dateTime >= dateTimePeriod.StartDate && dateTime <= dateTimePeriod.EndDate;
		}

		bool IsInUidList(Guid? uid, List<Guid> uidList)
		{
			if (uidList == null || uidList.Count == 0)
				return true;
			return uidList.Any(x => x == uid);
		}
		#region Devices
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