using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using System;
using XFiresecAPI;

namespace SKDDriver
{
	public class SKDDatabaseService
	{
		DataAccess.SKUDDataContext Context;
		
		public SKDDatabaseService()
		{
			Context = new DataAccess.SKUDDataContext();
			DocumentTranslator = new DocumentTranslator(Context.Document, Context);
			PositionTranslator = new PositionTranslator(Context.Position, Context);
			CardZoneTranslator = new CardZoneTranslator(Context.CardZoneLink, Context);
			CardTranslator = new CardTranslator(Context.Card, Context, CardZoneTranslator);
			GUDTranslator = new GUDTranslator(Context.GUD, Context, CardZoneTranslator);
			OrganizationTranslator = new OrganizationTranslator(Context.Organization, Context);
			JournalItemTranslator = new JournalItemTranslator(Context.Journal, Context);
			EmployeeTranslator = new EmployeeTranslator(Context.Employee, Context);
			DepartmentTranslator = new DepartmentTranslator(Context.Department, Context);
		}

		DocumentTranslator DocumentTranslator;
		PositionTranslator PositionTranslator;
		CardTranslator CardTranslator;
		CardZoneTranslator CardZoneTranslator;
		GUDTranslator GUDTranslator;
		OrganizationTranslator OrganizationTranslator;
		JournalItemTranslator JournalItemTranslator;
		EmployeeTranslator EmployeeTranslator;
		DepartmentTranslator DepartmentTranslator;
		
		#region Get
		public OperationResult<IEnumerable<Department>> GetDepartments(DepartmentFilter filter)
		{
			return DepartmentTranslator.Get(filter);
		}
		
		public OperationResult<IEnumerable<Employee>> GetEmployees(EmployeeFilter filter)
		{
			return EmployeeTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<SKDJournalItem>> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return JournalItemTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return OrganizationTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return PositionTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return CardTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return CardZoneTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return DocumentTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<GUD>> GetGUDs(GUDFilter filter)
		{
			return GUDTranslator.Get(filter);
		}
		#endregion

		#region Save
		public OperationResult SaveEmployees(IEnumerable<Employee> items)
		{
			return EmployeeTranslator.Save(items);
		}
		public OperationResult SaveDepartments(IEnumerable<Department> items)
		{
			return DepartmentTranslator.Save(items);
		}
		public OperationResult SavePositions(IEnumerable<Position> items)
		{
			return PositionTranslator.Save(items);
		}
		public OperationResult SaveSKDJournalItems(IEnumerable<SKDJournalItem> items)
		{
			return JournalItemTranslator.Save(items);
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return CardTranslator.Save(items);
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return CardZoneTranslator.Save(items);
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> items)
		{
			return OrganizationTranslator.Save(items);
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return DocumentTranslator.Save(items);
		}
		public OperationResult SaveGUDs(IEnumerable<GUD> items)
		{
			return GUDTranslator.Save(items);
		}
		#endregion

		#region MarkDeleted
		public OperationResult MarkDeletedEmployees(IEnumerable<Employee> items)
		{
			return EmployeeTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedDepartments(IEnumerable<Department> items)
		{
			return DepartmentTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> items)
		{
			return PositionTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedSKDJournalItems(IEnumerable<SKDJournalItem> items)
		{
			return JournalItemTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return CardTranslator.Save(items);
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return CardZoneTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			return OrganizationTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return DocumentTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedGUDs(IEnumerable<GUD> items)
		{
			return GUDTranslator.MarkDeleted(items);
		}
		#endregion
	}
}