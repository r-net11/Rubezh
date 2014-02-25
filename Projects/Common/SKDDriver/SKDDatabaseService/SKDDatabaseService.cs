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
			CardsTranslator = new CardsTranslator(Context.Card, Context);
		}

		DocumentsTranslator DocumentsTranslator;
		PositionsTranslator PositionsTranslator;
		CardsTranslator CardsTranslator;

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
		public OperationResult<IEnumerable<Position>> GetPositions(PositionFilter filter)
		{
			return PositionsTranslator.Get(filter);
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
		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			return CardsTranslator.Get(filter);
		}
		public IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneFilter filter)
		{
			try
			{
				var result = new List<CardZoneLink>();
				if (filter == null)
				{
					foreach (var item in Context.CardZoneLink)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.CardZoneLink)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<CardZoneLink>(); }
		}
		public IEnumerable<Organization> GetOrganizations(OrganizationFilter filter)
		{
			try
			{
				var result = new List<Organization>();
				if (filter == null)
				{
					foreach (var item in Context.Organization)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Organization)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Organization>(); }
		}
		public OperationResult<IEnumerable<Document>> GetDocuments(DocumentFilter filter)
		{
			return DocumentsTranslator.Get(filter);
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
		public void SaveCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.CardZoneLink.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.CardZoneLink.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void SaveOrganizations(IEnumerable<Organization> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Organization.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Organization.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult SaveDocuments(IEnumerable<Document> items)
		{
			return DocumentsTranslator.Save(items);
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
		public void MarkDeletedCardZoneLinks(IEnumerable<CardZoneLink> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.CardZoneLink.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public void MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Organization.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		public OperationResult MarkDeletedDocuments(IEnumerable<Document> items)
		{
			return DocumentsTranslator.MarkDeleted(items);
		}
		#endregion
	}
}