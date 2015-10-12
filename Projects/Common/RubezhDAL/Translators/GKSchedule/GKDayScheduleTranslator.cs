using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;

namespace RubezhDAL.DataClasses
{
	public class GKDayScheduleTranslator
	{
		DbService DbService;
		DatabaseContext Context;

		public GKDayScheduleTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
		}

		public OperationResult<List<RubezhAPI.GK.GKDaySchedule>> Get()
		{
			try
			{
				var result = GetTableItems().ToList().Select(x => Translate(x)).ToList();
				return new OperationResult<List<RubezhAPI.GK.GKDaySchedule>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<RubezhAPI.GK.GKDaySchedule>>.FromError(e.Message);
			}
		}

		public RubezhAPI.GK.GKDaySchedule Translate(GKDaySchedule tableItem)
		{
			var result = new RubezhAPI.GK.GKDaySchedule();
			result.UID = tableItem.UID;
			result.No = tableItem.No;
			result.Name = tableItem.Name;
			result.DayScheduleParts = tableItem.GKDayScheduleParts.Select(x => new RubezhAPI.GK.GKDaySchedulePart
			{
				StartMilliseconds = x.StartMilliseconds,
				EndMilliseconds = x.EndMilliseconds,
			}).OrderBy(x => x.StartMilliseconds).ToList();
			return result;
		}

		public OperationResult Save(RubezhAPI.GK.GKDaySchedule item)
		{
			try
			{
				bool isNew = false;
				var tableItem = Context.GKDaySchedules.Include(x => x.GKDayScheduleParts).FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					isNew = true;
					tableItem = new GKDaySchedule { UID = item.UID };
				}
				else
					Context.GKDayScheduleParts.RemoveRange(tableItem.GKDayScheduleParts);
				tableItem.No = item.No;
				tableItem.Name = item.Name;
				tableItem.Description = item.Description;
				tableItem.GKDayScheduleParts = item.DayScheduleParts.Select(x => new GKDaySchedulePart
				{
					UID = Guid.NewGuid(),
					StartMilliseconds = x.StartMilliseconds,
					EndMilliseconds = x.EndMilliseconds
				}).ToList();
				if (isNew)
					Context.GKDaySchedules.Add(tableItem);
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Delete(RubezhAPI.GK.GKDaySchedule item)
		{

			try
			{
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					Context.GKDaySchedules.Remove(tableItem);
				}
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		IQueryable<GKDaySchedule> GetTableItems()
		{
			return Context.GKDaySchedules.Include(x => x.GKDayScheduleParts).Include(x => x.ScheduleGKDaySchedules);
		}
	}
}