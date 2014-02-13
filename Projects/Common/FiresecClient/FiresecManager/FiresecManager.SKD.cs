using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		#region Get
		public static IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			return FiresecService.GetEmployees(filter);
		}
		public static IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			return FiresecService.GetDepartments(filter);
		}
		public static IEnumerable<Position> GetPositions(PositionFilter filter)
		{
			return FiresecService.GetPositions(filter);
		}
		public static IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return FiresecService.GetSKDJournalItems(filter);
		}
		public static IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return FiresecService.GetFrames(filter);
		}
		public static IEnumerable<SKDCard> GetCards(CardFilter filter)
		{
			return FiresecService.GetCards(filter);
		}
		public static IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter)
		{
			return FiresecService.GetCardZoneLinks(filter);
		}
		#endregion

		#region Save
		public static void SaveEmployees(IEnumerable<Employee> Employees)
		{
			FiresecService.SaveEmployees(Employees);
			foreach (var employee in Employees)
			{
				var cards = GetCards(new CardFilter{ Uids = employee.CardUids });
				foreach (var card in cards)
				{
					card.EmployeeUid = employee.UID;
				}
				FiresecService.SaveCards(cards);
				 
			}
		}
		public static void SaveDepartments(IEnumerable<Department> Departments)
		{
			FiresecService.SaveDepartments(Departments);
		}
		public static void SavePositions(IEnumerable<Position> Positions)
		{
			FiresecService.SavePositions(Positions);
		}
		public static void SaveJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			FiresecService.SaveSKDJournalItems(journalItems);
		}
		public static void SaveFrames(IEnumerable<Frame> frames)
		{
			FiresecService.SaveFrames(frames);
		}
		public static void SaveCards(IEnumerable<SKDCard> items)
		{
			FiresecService.SaveCards(items);
		}
		public static void SaveCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			FiresecService.SaveCardZoneLinks(items);
		}
		#endregion

		#region MarkDeleted
		public static void MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			FiresecService.MarkDeletedEmployees(Employees);
		}
		public static void MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			FiresecService.MarkDeletedDepartments(Departments);
		}
		public static void MarkDeletedPositions(IEnumerable<Position> Positions)
		{
			FiresecService.MarkDeletedPositions(Positions);
		}
		public static void MarkDeletedJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			FiresecService.MarkDeletedSKDJournalItems(journalItems);
		}
		public static void MarkDeletedFrames(IEnumerable<Frame> frames)
		{
			FiresecService.MarkDeletedFrames(frames);
		}
		public static void MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			FiresecService.MarkDeletedCards(items);
		}
		public static void MarkDeletedCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			FiresecService.MarkDeletedCardZoneLinks(items);
		} 
		#endregion

		#region Get(Uid)
		public static Employee GetEmployee(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new EmployeeFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetEmployees(filter).ToList().FirstOrDefault();
		}
		public static Department GetDepartment(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DepartmentFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetDepartments(filter).ToList().FirstOrDefault();
		}
		public static Position GetPosition(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PositionFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetPositions(filter).ToList().FirstOrDefault();
		}
		public static SKDJournalItem GetJournalItem(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new SKDJournalFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetSKDJournalItems(filter).ToList().FirstOrDefault();
		}
		public static Frame GetFrame(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new FrameFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetFrames(filter).ToList().FirstOrDefault();
		}
		public static SKDCard GetCard(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new CardFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetCards(filter).ToList().FirstOrDefault();
		}
		public static CardZoneLink GetCardZoneLink(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new CardZoneLinkFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetCardZoneLinks(filter).ToList().FirstOrDefault();
		}
		#endregion 

	}
}