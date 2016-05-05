using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.SKD;

namespace StrazhDAL
{
	public class AttachmentTranslator : TranslatorBase<DataAccess.Attachment, Attachment>
	{
		public AttachmentTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override void TranslateBack(DataAccess.Attachment tableItem, Attachment apiItem)
		{
			tableItem.UID = apiItem.UID;
			tableItem.Name = apiItem.FileName;
		}

		protected override Attachment Translate(DataAccess.Attachment tableItem)
		{
			var result = base.Translate(tableItem);
			result.FileName = tableItem.Name;
			return result;
		}
	}
}
