using StrazhAPI;
using StrazhAPI.Journal;
using System;
using System.Data.Linq;
using System.Linq;

namespace StrazhDAL
{
	public class JournalTranslator : IDisposable
	{
		private Table<DataAccess.Journal> _Table;

		private string Name { get { return "Journal"; } }

		public string NameXml { get { return Name + ".xml"; } }

		public static string ConnectionString { get; set; }

		private DataAccess.JournalDataContext Context;

		public JournalTranslator()
		{
			Context = new DataAccess.JournalDataContext(ConnectionString);
			_Table = Context.Journals;
		}

		public OperationResult SaveVideoUID(Guid itemUID, Guid videoUID, Guid cameraUID)
		{
			try
			{
				var tableItem = _Table.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.VideoUID = videoUID;
					tableItem.CameraUID = cameraUID;
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveCameraUID(Guid itemUID, Guid CameraUID)
		{
			try
			{
				var tableItem = _Table.FirstOrDefault(x => x.UID == itemUID);
				if (tableItem != null)
				{
					tableItem.CameraUID = CameraUID;
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				var result = Context.Journals.Min(x => x.SystemDate);
				return new OperationResult<DateTime>(result);
			}
			catch (Exception e)
			{
				return OperationResult<DateTime>.FromError(e.Message);
			}
		}

		public OperationResult<int> GetLastJournalItemNoByController(Guid controllerUid)
		{
			var journals = Context.Journals.Where(x => x.ControllerUID == controllerUid)
				.Where(x => x.Name == (int)JournalEventNameType.Проход_запрещен || x.Name == (int)JournalEventNameType.Проход_разрешен).ToList();
			var result = 0;
			if (journals.Count > 0)
				result = journals.Max(x => x.No);
			return new OperationResult<int>(result);
		}

		public OperationResult<int> GetLastAlarmJournalItemNoByController(Guid controllerUid)
		{
			var journals = Context.Journals.Where(x => x.ControllerUID == controllerUid)
				.Where(x => x.Name == (int)JournalEventNameType.Принуждение
				|| x.Name == (int)JournalEventNameType.Взлом
				|| x.Name == (int)JournalEventNameType.Дверь_не_закрыта_начало
				|| x.Name == (int)JournalEventNameType.Дверь_не_закрыта_конец
				|| x.Name == (int)JournalEventNameType.Повторный_проход).ToList();
			var result = 0;
			if (journals.Count > 0)
				result = journals.Max(x => x.No);
			return new OperationResult<int>(result);
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}