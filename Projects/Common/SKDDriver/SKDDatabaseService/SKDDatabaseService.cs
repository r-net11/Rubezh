using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using XFiresecAPI;

namespace SKDDriver
{
	public class SKDDatabaseService : IFiresecServiceSKUD
	{
		DataAccess.SKUDDataContext Context;
        
		public SKDDatabaseService()
		{
			Context = new DataAccess.SKUDDataContext();
		}


		#region IFiresecServiceSKUD Members

		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
            try
            {
                var employees = new List<Employee>();
				var databaseEmployees = Context.Employee.ToList().Where(x => FilterHelper.IsInFilter(x, filter)).ToList();
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
				var databaseDepartments = Context.Department.ToList().Where(x => FilterHelper.IsInFilter(x, filter)).ToList();
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
				var databasePositions = Context.Position.ToList().Where(x => FilterHelper.IsInFilter(x, filter)).ToList();
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
				var databaseJournalItems = Context.Journal.ToList().Where(x => FilterHelper.IsInFilter(x, filter)).ToList();
				databaseJournalItems.ForEach(x => journalItems.Add(Translator.Translate(x)));
				return journalItems;
			}
			catch { return new List<SKDJournalItem>(); }
		}

		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			try
			{
				var result = new List<Frame>();
				var databaseItems = Context.Frame.ToList().Where(x => FilterHelper.IsInFilter(x, filter)).ToList();
				databaseItems.ForEach(x => result.Add(Translator.Translate(x)));
				return result;
			}
			catch { return new List<Frame>(); }
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

		public void SaveFrames(IEnumerable<Frame> frames)
		{
			try
			{
				foreach (var item in frames)
				{
					if (item != null)
						Context.Frame.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { } 
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


        #endregion

	}
}