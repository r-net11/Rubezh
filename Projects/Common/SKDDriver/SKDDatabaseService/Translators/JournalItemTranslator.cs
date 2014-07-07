using System;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.Events;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using LinqKit;
using JournalItem = FiresecAPI.SKD.JournalItem;

namespace SKDDriver
{
	public class JournalItemTranslator : TranslatorBase<DataAccess.Journal, JournalItem, SKDJournalFilter>
	{
		public JournalItemTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{
			;
		}

		protected override JournalItem Translate(DataAccess.Journal tableItem)
		{
			return new JournalItem
			{
				Description = (EventDescription)tableItem.Description,
				DescriptionText = tableItem.DescriptionText,
				DeviceDateTime = tableItem.DeviceDate,
				Name = (GlobalEventNameEnum)tableItem.Name,
				NameText = tableItem.NameText,
				ObjectName = tableItem.ObjectName,
				ObjectType = (ObjectType)tableItem.ObjectType,
				ObjectUID = tableItem.ObjectUID,
				State = (XStateClass)tableItem.State,
				SubsystemType = (SubsystemType)tableItem.Subsystem,
				SystemDateTime = tableItem.SystemDate,
				UID = tableItem.UID,
				UserName = tableItem.UserName,
				CardNo = tableItem.CardNo
			};
		}

		protected override void TranslateBack(DataAccess.Journal tableItem, JournalItem apiItem)
		{
			tableItem.Description = (int)apiItem.Description;
			tableItem.DescriptionText = apiItem.DescriptionText;
			tableItem.DeviceDate = CheckDate(apiItem.DeviceDateTime);
			tableItem.Name = (int)apiItem.Name;
			tableItem.NameText = apiItem.NameText;
			tableItem.ObjectName = apiItem.ObjectName;
			tableItem.ObjectType = (int)apiItem.ObjectType;
			tableItem.ObjectUID = apiItem.ObjectUID;
			tableItem.State = (int)apiItem.State;
			tableItem.Subsystem = (int)apiItem.SubsystemType;
			tableItem.SystemDate = CheckDate(apiItem.SystemDateTime);
			tableItem.UserName = apiItem.UserName;
			tableItem.CardNo = tableItem.CardNo;
		}

		protected override Expression<Func<DataAccess.Journal, bool>> IsInFilter(SKDJournalFilter filter)
		{
			var result = base.IsInFilter(filter);

			var eventNames = filter.EventNames;
			if (eventNames != null && eventNames.Count != 0)
				result = result.And(e => eventNames.Contains((GlobalEventNameEnum)e.Name));

			var deviceDates = filter.DeviceDateTime;
			if (deviceDates != null)
				result = result.And(e => e.DeviceDate != null &&
					e.DeviceDate >= deviceDates.StartDate &&
					e.DeviceDate <= deviceDates.EndDate);

			var systemDates = filter.SystemDateTime;
			if (systemDates != null)
				result = result.And(e => e.SystemDate != null &&
					e.SystemDate >= systemDates.StartDate &&
					e.SystemDate <= systemDates.EndDate);

			return result;
		}
	}
}