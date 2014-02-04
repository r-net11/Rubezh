using System.Collections.Generic;
using System;
using System.Linq;
using FiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			return FiresecService.GetEmployees(filter);
		}
		public static IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			return FiresecService.GetDepartments(filter);
		}
		public static Department GetDepartment(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DepartmentFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetDepartments(filter).ToList().FirstOrDefault();
		}
		public static IEnumerable<Position> GetPositions(PositionFilter filter)
		{
			return FiresecService.GetPositions(filter);
		}
		public static Position GetPosition(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PositionFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetPositions(filter).ToList().FirstOrDefault();
		}

		public static IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return FiresecService.GetSKDJournalItems(filter);
		}
		public static SKDJournalItem GetJournalItem(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new SKDJournalFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetSKDJournalItems(filter).ToList().FirstOrDefault();
		}
		public static void SaveJournalItem(IEnumerable<SKDJournalItem> journalItems)
		{
			FiresecService.SaveSKDJournalItems(journalItems);
		}
		public static IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return FiresecService.GetFrames(filter);
		}
		public static IEnumerable<Card> GetCards(CardFilter filter)
		{
			return FiresecService.GetCards(filter);
		}
		public static IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter)
		{
			return FiresecService.GetCardZoneLinks(filter);
		} 
		public static void SaveFrames(IEnumerable<Frame> frames)
		{
			FiresecService.SaveFrames(frames);
		}
		public static void SaveCards(IEnumerable<Card> items)
		{
			FiresecService.SaveCards(items);
		}
		public static void SaveCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			FiresecService.SaveCardZoneLinks(items);
		}


	}
}