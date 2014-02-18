﻿using System.Collections.Generic;
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
		}

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
		public IEnumerable<Position> GetPositions(PositionFilter filter)
		{
			try
			{
				var result = new List<Position>();
				if (filter == null)
				{
					foreach (var item in Context.Position)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Position)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Position>(); }
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
		public IEnumerable<SKDCard> GetCards(CardFilter filter)
		{
			try
			{
				var result = new List<SKDCard>();
				if (filter == null)
				{
					foreach (var item in Context.Card)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Card)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<SKDCard>(); }
		}
		public IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter)
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
		public IEnumerable<Document> GetDocuments(DocumentFilter filter)
		{
			try
			{
				var result = new List<Document>();
				if (filter == null)
				{
					foreach (var item in Context.Document)
						result.Add(Translator.Translate(item));
					return result;
				}
				foreach (var item in Context.Document)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Document>(); }
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
		public void SavePositions(IEnumerable<Position> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Position.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Position.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
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
		public void SaveCards(IEnumerable<SKDCard> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Card.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Card.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
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
		public void SaveDocuments(IEnumerable<Document> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;

					var databaseItem = Context.Document.FirstOrDefault(x => x.Uid == item.UID);
					if (databaseItem != null)
					{
						Translator.Update(databaseItem, item);
					}
					else
						Context.Document.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
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
		public void MarkDeletedPositions(IEnumerable<Position> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Position.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
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
		public void MarkDeletedCards(IEnumerable<SKDCard> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Card.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				} 
				Context.SubmitChanges();
			}
			catch { }
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
		public void MarkDeletedDocuments(IEnumerable<Document> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = Context.Document.FirstOrDefault(x => x.Uid == item.UID);
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		#endregion
	}
}