using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;

namespace StrazhDAL
{
	public class AccessTemplateDeactivatingReaderTranslator : TranslatorBase<DataAccess.AccessTemplateDeactivatingReader, AccessTemplateDeactivatingReader>
	{
		public AccessTemplateDeactivatingReaderTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override void TranslateBack(DataAccess.AccessTemplateDeactivatingReader tableItem, AccessTemplateDeactivatingReader apiItem)
		{
			tableItem.UID = apiItem.UID;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.DeactivatingReaderUID = apiItem.DeactivatingReaderUID;
		}

		protected override AccessTemplateDeactivatingReader Translate(DataAccess.AccessTemplateDeactivatingReader tableItem)
		{
			var result = base.Translate(tableItem);
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			result.DeactivatingReaderUID = tableItem.DeactivatingReaderUID;
			return result;
		}

		/// <summary>
		/// Из таблицы "AccessTemplateDeactivatingReader" БД "SKD" удаляет все записи, у которых поле "AccessTemplateUID" равно заданному аргументу "accessTemplateUID"
		/// </summary>
		/// <param name="accessTemplateUID">Идентификатор шаблона доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult RemoveFromAccessTemplate(Guid accessTemplateUID)
		{
			try
			{
				var databaseItems = Table.Where(x => x.AccessTemplateUID == accessTemplateUID);
				Extensions.ForEach(databaseItems, x => Delete(x.UID));
				return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("База данных. Возникла исключительная ситуация при удалении записей из таблицы 'AccessTemplateDeactivatingReader', у которых AccessTemplateUID='{0}'", accessTemplateUID));
				return new OperationResult(e.Message);
			}
		}

		public List<AccessTemplateDeactivatingReader> GetForAccessTemplate(Guid accessTemplateUID)
		{
			var result = new List<AccessTemplateDeactivatingReader>();
			foreach (var deactivatingReader in Table.Where(x => x != null && x.AccessTemplateUID == accessTemplateUID))
			{
				result.Add(Translate(deactivatingReader));
			}
			return result;
		}

		public bool HasReader(Guid readerUID)
		{
			return Table.Any(x => x.DeactivatingReaderUID == readerUID);
		}
	}
}