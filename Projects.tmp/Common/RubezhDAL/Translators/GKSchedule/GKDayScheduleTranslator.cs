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
			return DbServiceHelper.InTryCatch(() =>
			{
				return GetTableItems().ToList().Select(x => Translate(x)).ToList();
			});
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

		public OperationResult<bool> Save(RubezhAPI.GK.GKDaySchedule item)
		{
			return DbServiceHelper.InTryCatch(() => 
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
				return true;
			});
		}

		public OperationResult<bool> Delete(RubezhAPI.GK.GKDaySchedule item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					Context.GKDaySchedules.Remove(tableItem);
				}
				Context.SaveChanges();
				return true;
			});
		}

		IQueryable<GKDaySchedule> GetTableItems()
		{
			return Context.GKDaySchedules.Include(x => x.GKDayScheduleParts).Include(x => x.ScheduleGKDaySchedules);
		}
	}
}