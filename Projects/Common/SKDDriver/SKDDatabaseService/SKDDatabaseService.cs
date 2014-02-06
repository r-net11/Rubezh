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
		}

		#region Get
		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			try
			{
				var result = new List<Employee>();
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
				foreach (var item in Context.CardZoneLink)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<CardZoneLink>(); }
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

					var databaseItem = Context.Employee.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
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

					var databaseItem = Context.Department.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
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

					var databaseItem = Context.Position.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
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
						databaseItem = Translator.TranslateBack(item);
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

					var databaseItem = Context.Frame.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
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

					var databaseItem = Context.Card.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
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

					var databaseItem = Context.CardZoneLink.FirstOrDefault(x => x.Uid == item.Uid);
					if (databaseItem != null)
					{
						databaseItem = Translator.TranslateBack(item);
					}
					else
						Context.CardZoneLink.InsertOnSubmit(Translator.TranslateBack(item));
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
						var databaseItem = Context.Employee.FirstOrDefault(x => x.Uid == item.Uid);
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
						var databaseItem = Context.Department.FirstOrDefault(x => x.Uid == item.Uid);
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
						var databaseItem = Context.Position.FirstOrDefault(x => x.Uid == item.Uid);
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
						var databaseItem = Context.Frame.FirstOrDefault(x => x.Uid == item.Uid);
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
						var databaseItem = Context.Card.FirstOrDefault(x => x.Uid == item.Uid);
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
						var databaseItem = Context.CardZoneLink.FirstOrDefault(x => x.Uid == item.Uid);
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