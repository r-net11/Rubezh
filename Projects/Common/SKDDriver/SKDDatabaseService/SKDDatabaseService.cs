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
			DocumentsTranslator = new DocumentsTranslator(Context.Document, Context);
			PositionsTranslator = new PositionsTranslator(Context.Position, Context);
			CardZonesTranslator = new CardZonesTranslator(Context.CardZoneLink, Context);
			CardsTranslator = new CardsTranslator(Context.Card, Context, CardZonesTranslator);
			GUDTranslator = new GUDTranslator(Context.GUD, Context, CardZonesTranslator);
			OrganizationTranslator = new OrganizationTranslator(Context.Organization, Context);
		}

		DocumentsTranslator DocumentsTranslator;
		PositionsTranslator PositionsTranslator;
		CardsTranslator CardsTranslator;
		CardZonesTranslator CardZonesTranslator;
		GUDTranslator GUDTranslator;
		OrganizationTranslator OrganizationTranslator;

		#region Get
		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			
			try
			{
				var result = new List<Employee>();
				if (filter == null)
				{
					foreach (var item in Context.Employee)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Employee)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Employee>(); }
		}
		public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			try
			{
				var result = new List<Department>();
				if (filter == null)
				{
					foreach (var item in Context.Department)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Department)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Department>(); }
		}
		public IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			try
			{
				var result = new List<SKDJournalItem>();
				if (filter == null)
				{
					foreach (var item in Context.Journal)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Journal)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<SKDJournalItem>(); }
		}
		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			try
			{
				var result = new List<Frame>();
				if (filter == null)
				{
					foreach (var item in Context.Frame)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Frame)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Frame>(); }
		}
		public OperationResult<IEnumerable<Organization>> GetOrganizations(OrganizationFilter filter)
		{
			return OrganizationTranslator.Get(filter);
		}
		
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return PositionsTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return CardsTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<CardZone>> GetCardZones(CardZoneFilter filter)
		{
			return CardZonesTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return DocumentsTranslator.Get(filter);
		}
		public OperationResult<IEnumerable<GUD>> GetGUDs(GUDFilter filter)
		{
			return GUDTranslator.Get(filter);
		}
		#endregion

		#region Save
		public void SaveEmployees(IEnumerable<Employee> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
					    continue;

					var databaseItem = Context.Employee.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Employee.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void SaveDepartments(IEnumerable<Department> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Department.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Department.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult SavePositions(IEnumerable<Position> items)
		{
			return PositionsTranslator.Save(items);
		}
		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			try
			{
				foreach (var item in journalItems)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Journal.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Journal.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void SaveFrames(IEnumerable<Frame> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Frame.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Frame.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult SaveCards(IEnumerable<SKDCard> items)
		{
			return CardsTranslator.Save(items);
		}
		public OperationResult SaveCardZones(IEnumerable<CardZone> items)
		{
			return CardZonesTranslator.Save(items);
		}
		public OperationResult SaveOrganizations(IEnumerable<Organization> items)
		{
			return OrganizationTranslator.Save(items);
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return DocumentsTranslator.Save(items);
		}
		public OperationResult SaveGUDs(IEnumerable<GUD> items)
		{
			return GUDTranslator.Save(items);
		}
		#endregion

		#region MarkDeleted
		public void MarkDeletedEmployees(IEnumerable<Employee> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Employee.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void MarkDeletedDepartments(IEnumerable<Department> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Department.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult MarkDeletedPositions(IEnumerable<Position> items)
		{
			return PositionsTranslator.MarkDeleted(items);
		}
		public void MarkDeletedSKDJournalItems(IEnumerable<SKDJournalItem> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Journal.FirstOrDefault(x => x.Uid == item.Uid);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void MarkDeletedFrames(IEnumerable<Frame> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Frame.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			return CardsTranslator.Save(items);
		}
		public OperationResult MarkDeletedCardZones(IEnumerable<CardZone> items)
		{
			return CardZonesTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			return OrganizationTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return DocumentsTranslator.MarkDeleted(items);
		}
		public OperationResult MarkDeletedGUDs(IEnumerable<GUD> items)
		{
			return GUDTranslator.MarkDeleted(items);
		}
		#endregion
	}
}