using StrazhAPI;
using System;
using System.Data.Linq;
using System.Linq;
using StrazhAPI.Journal;

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

		public OperationResult<DateTime> GetLastJournalItemTimeProducedByController(Guid controllerUid)
		{
			try
			{
				var journalResult = Context.GetLastJournalItemProducedByController(controllerUid);
				var journalResultItems = journalResult.ToArray();
				
				if (journalResultItems.Length != 1)
                    return OperationResult<DateTime>.FromError(Resources.Language.Translators.JournalTranslator.GetLastJournalItemTimeProducedByController_Empty_Error);
				
				var deviceDate = journalResultItems[0].DeviceDate;
				
				if  (!deviceDate.HasValue)
                    return OperationResult<DateTime>.FromError(Resources.Language.Translators.JournalTranslator.GetLastJournalItemTimeProducedByController_DeviceDate_Error);
				
				return new OperationResult<DateTime>(journalResultItems[0].DeviceDate.Value);
			}
			catch (Exception e)
			{
				return OperationResult<DateTime>.FromError(e.Message);
			}

		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}