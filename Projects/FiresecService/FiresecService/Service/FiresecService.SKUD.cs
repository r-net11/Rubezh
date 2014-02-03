using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models.SKDDatabase;
using SKDDriver;


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
		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			_skdService.SaveSKDJournalItems(journalItems);
		}
		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return _skdService.GetFrames(filter);
		}
		public void SaveFrames(IEnumerable<Frame> frames)
		{
			_skdService.SaveFrames(frames);
		}

        #endregion
    }
}