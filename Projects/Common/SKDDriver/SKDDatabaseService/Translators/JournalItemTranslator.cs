using System;
using System.Linq.Expressions;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using LinqKit;

namespace SKDDriver
{
	public class JournalItemTranslator : TranslatorBase<DataAccess.Journal, JournalItem, JournalFilter>
	{
		public JournalItemTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{
			var j = new JournalItem();
			j.JournalDetalisationItems.Add(new JournalDetalisationItem("Name", "Value"));
			var tableJ = new DataAccess.Journal();
			tableJ.UID = j.UID;
			TranslateBack(tableJ, j);
			var jOut = Translate(tableJ);
		}
		
		protected override JournalItem Translate(DataAccess.Journal tableItem)
		{
			var journalItem = new JournalItem
			{
				JournalEventDescriptionType = (JournalEventDescriptionType)tableItem.Description,
				DescriptionText = tableItem.DescriptionText,
				DeviceDateTime = tableItem.DeviceDate,
				JournalEventNameType = (JournalEventNameType)tableItem.Name,
				NameText = tableItem.NameText,
				ObjectName = tableItem.ObjectName,
				JournalObjectType = (JournalObjectType)tableItem.ObjectType,
				ObjectUID = tableItem.ObjectUID,
				StateClass = (XStateClass)tableItem.State,
				JournalSubsystemType = (JournalSubsystemType)tableItem.Subsystem,
				SystemDateTime = tableItem.SystemDate,
				UID = tableItem.UID,
				UserName = tableItem.UserName,
				EmployeeUID = tableItem.EmployeeUID.HasValue ? tableItem.EmployeeUID.Value : Guid.Empty,
				JournalDetalisationItems = JournalDetalisationItem.StringToList(tableItem.Detalisation)
			};
			return journalItem;
		}

		protected override void TranslateBack(DataAccess.Journal tableItem, JournalItem apiItem)
		{
			tableItem.Description = (int)apiItem.JournalEventDescriptionType;
			tableItem.DescriptionText = apiItem.DescriptionText;
			tableItem.DeviceDate = CheckDate(apiItem.DeviceDateTime);
			tableItem.Name = (int)apiItem.JournalEventNameType;
			tableItem.NameText = apiItem.NameText;
			tableItem.ObjectName = apiItem.ObjectName;
			tableItem.ObjectType = (int)apiItem.JournalObjectType;
			tableItem.ObjectUID = apiItem.ObjectUID;
			tableItem.State = (int)apiItem.StateClass;
			tableItem.Subsystem = (int)apiItem.JournalSubsystemType;
			tableItem.SystemDate = CheckDate(apiItem.SystemDateTime);
			tableItem.UserName = apiItem.UserName;
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.Detalisation = JournalDetalisationItem.ListToString(apiItem.JournalDetalisationItems);
		}

		protected override Expression<Func<DataAccess.Journal, bool>> IsInFilter(JournalFilter filter)
		{
			var result = base.IsInFilter(filter);

			var journalEventNameTypes = filter.JournalEventNameTypes;
			if (journalEventNameTypes != null && journalEventNameTypes.Count != 0)
				result = result.And(e => journalEventNameTypes.Contains((JournalEventNameType)e.Name));

			return result;
		}
	}
}