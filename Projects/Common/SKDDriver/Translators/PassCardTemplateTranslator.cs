using System;
using Common;
using DevExpress.XtraReports.UI;
using StrazhAPI;
using StrazhAPI.SKD;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhDAL
{
	public class PassCardTemplateTranslator : WithShortTranslator<DataAccess.PassCardTemplate, PassCardTemplate, PassCardTemplateFilter, ShortPassCardTemplate>
	{
		private readonly DataContractSerializer _serializer;

		public PassCardTemplateTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			_serializer = new DataContractSerializer(typeof(PassCardTemplate));
		}

		protected override OperationResult CanSave(PassCardTemplate item)
		{
			var result = base.CanSave(item);

			if (result.HasError)
				return result;

			var hasSameName = Table.Any(x => x.Name == item.Caption &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);

			return hasSameName ? new OperationResult("Попытка добавления шаблон пропуска с совпадающим наименованием") : new OperationResult();
		}

		protected override PassCardTemplate Translate(DataAccess.PassCardTemplate tableItem)
		{
			try
			{
				using (var ms = new MemoryStream(tableItem.Data.ToArray()))
				{
					return (PassCardTemplate)_serializer.ReadObject(ms);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return null;
			}
		}

		protected override void TranslateBack(DataAccess.PassCardTemplate tableItem, PassCardTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Caption;
			tableItem.Description = apiItem.Description;
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, apiItem);
				tableItem.Data = ms.ToArray();
			}
		}

		protected override ShortPassCardTemplate TranslateToShort(DataAccess.PassCardTemplate tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}
	}
}