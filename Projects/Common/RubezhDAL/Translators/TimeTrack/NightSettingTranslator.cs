using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class NightSettingTranslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public NightSettingTranslator(DbService context)
		{
			DbService = context;
		}

		public OperationResult<bool> Save(API.NightSettings item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				bool isNew = false;
				var tableItem = Context.NightSettings.FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					tableItem = new NightSetting { UID = item.UID };
					isNew = true;
				}
				tableItem.NightStartTime = (int)item.NightStartTime.TotalMinutes;
				tableItem.NightEndTime = (int)item.NightEndTime.TotalMinutes;
				tableItem.OrganisationUID = item.OrganisationUID.EmptyToNull();
				if (isNew)
					Context.NightSettings.Add(tableItem);
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<API.NightSettings> GetByOrganisation(Guid organisationUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.NightSettings.FirstOrDefault(x => x.OrganisationUID == organisationUID);
				if (tableItem == null)
					return null;
				return Transalte(tableItem);
			});
		}

		public API.NightSettings Transalte(NightSetting tableItem)
		{
			return new API.NightSettings
			{
				UID = tableItem.UID,
				NightEndTime = TimeSpan.FromMinutes(tableItem.NightEndTime),
				NightStartTime = TimeSpan.FromMinutes(tableItem.NightStartTime),
				OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
			};
		}
	}
}