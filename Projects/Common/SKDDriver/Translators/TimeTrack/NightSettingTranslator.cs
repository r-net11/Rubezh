using FiresecAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class NightSettingTranslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public NightSettingTranslator(DbService context)
		{
			DbService = context;
		}

		public OperationResult Save(API.NightSettings item)
		{
			try
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<API.NightSettings> GetByOrganisation(Guid organisationUID)
		{
			try
			{
				var tableItem = Context.NightSettings.FirstOrDefault(x => x.OrganisationUID == organisationUID);
				if (tableItem == null)
					return new OperationResult<API.NightSettings>();
				return new OperationResult<API.NightSettings>(Transalte(tableItem));
			}
			catch (Exception e)
			{
				return OperationResult<API.NightSettings>.FromError(e.Message);
			}
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