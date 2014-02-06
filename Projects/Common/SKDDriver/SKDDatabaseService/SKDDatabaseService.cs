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

		public IEnumerable<Card> GetCards(CardFilter filter)
		{
			try
			{
				var result = new List<Card>();
				foreach (var item in Context.Card)
				{
					if (FilterHelper.IsInFilter(item, filter))
						result.Add(Translator.Translate(item));
				}
				return result;
			}
			catch { return new List<Card>(); }
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

		public void SaveFrames(IEnumerable<Frame> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
						Context.Frame.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}

		public void SaveCards(IEnumerable<Card> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
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
					if (item != null)
						Context.CardZoneLink.InsertOnSubmit(Translator.TranslateBack(item));
				}
				Context.SubmitChanges();
			}
			catch { }
		}
		#endregion
	}
}