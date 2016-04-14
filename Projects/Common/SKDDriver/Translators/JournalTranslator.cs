using Common;
using FiresecAPI;
using System;
using System.Data.Linq;
using System.Linq;
using FiresecAPI.Journal;

namespace SKDDriver.Translators
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
			Logger.Info(string.Format("Контроллер '{0}'. По журналу событий определяем время последнего зарегистрированного для данного контроллера события", controllerUid));
			try
			{
				var journalResult = Context.GetLastJournalItemProducedByController(controllerUid);
				var journalResultItems = journalResult.ToArray();

				if (journalResultItems.Length != 1)
				{
					Logger.Info(string.Format("Контроллер '{0}'. Нет зарегистрированных событий. Поэтому по журналу событий не можем определить время последнего зарегистрированного для данного контроллера события", controllerUid));
					return OperationResult<DateTime>.FromError("Нет зарегистрированных событий");
				}

				var deviceDate = journalResultItems[0].DeviceDate;

				if (!deviceDate.HasValue)
				{
					Logger.Info(string.Format("Контроллер '{0}'. Для зарегистрированного события не зафиксировано время на устройстве. Поэтому по журналу событий не можем определить время последнего зарегистрированного для данного контроллера события", controllerUid));
					return OperationResult<DateTime>.FromError("Для зарегистрированного события не зафиксировано время на устройстве");
				}

				return new OperationResult<DateTime>(journalResultItems[0].DeviceDate.Value);
			}
			catch (Exception e)
			{
				Logger.Warn(string.Format("Контроллер '{0}'. Ошибка при определении по журналу событий времени последнего зарегистрированного для данного контроллера события", controllerUid));
				return OperationResult<DateTime>.FromError(e.Message);
			}

		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}