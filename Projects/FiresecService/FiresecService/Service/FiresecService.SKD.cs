using Common;
using FiresecService.Report.Helpers;
using Infrastructure.Common;
using Localization.StrazhService.Core.Common;
using Localization.StrazhService.Core.Errors;
using StrazhAPI;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhDAL;
using StrazhDeviceSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		private string UserName
		{
			get
			{
				return CurrentClientCredentials != null ? CurrentClientCredentials.FriendlyUserName : "<Нет>";
				if (CurrentClientCredentials != null)
					return CurrentClientCredentials.FriendlyUserName;
				return CommonResources.None;
			}
		}

		private readonly IList<Guid> _criticalOperationRunningControllerGuids = new List<Guid>();
		private readonly object _criticalOperationRunningControllerGuidsLock = new object();

		#region Employee

		public OperationResult<IEnumerable<Employee>> GetFullEmployeeData(EmployeeFilter filter)
		{
			using (var ds = new SKDDatabaseService())
			{
				return ds.EmployeeTranslator.GetFullList(filter);
			}
		}

		public OperationResult<IEnumerable<ShortEmployee>> GetEmployeeList(EmployeeFilter filter)
		{
			OperationResult<IEnumerable<ShortEmployee>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.EmployeeTranslator.GetList(filter);
			}
			return result;
		}

		public OperationResult<Employee> GetEmployeeDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.GetSingle(uid);
			}
		}

		public OperationResult SaveEmployee(Employee item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_сотрудника, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_сотрудника, name, JournalEventDescriptionType.Удаление, uid: uid);
			var errors = new List<string>();
			using (var databaseService = new SKDDatabaseService())
			{
				var getEmployeeOperationResult = databaseService.EmployeeTranslator.GetSingle(uid);

				if (!getEmployeeOperationResult.HasError)
				{
					foreach (var card in getEmployeeOperationResult.Result.Cards)
					{
						var operationResult = DeleteCardFromEmployee(card, CommonResources.EmployeeDeleted);
						if (operationResult.HasError)
						{
							errors.AddRange(operationResult.Errors);
						}
					}
				}

				var interval = databaseService.PassJournalTranslator.TryGetOpenedInterval(uid);
				if (interval != null)
					databaseService.PassJournalTranslator.ManualClosingInterval(interval, DateTime.Now);

				var markdDletedOperationResult = databaseService.EmployeeTranslator.MarkDeleted(uid);
				if (markdDletedOperationResult.HasError)
				{
					errors.Add(string.Format(CommonErrors.DB_Error, markdDletedOperationResult.Error));
				}
			}

			return errors.Any() ? new OperationResult(errors) : new OperationResult();
		}

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracks(filter, startDate, endDate);
			}
		}

		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackTranslator.GetTimeTracksStream(filter, startDate, endDate);
			}
		}

		public OperationResult SaveEmployeeDepartment(Guid uid, Guid departmentUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SaveDepartment(uid, departmentUid);
			}
		}

		public OperationResult SaveEmployeePosition(Guid uid, Guid PositionUid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.SavePosition(uid, PositionUid);
			}
		}

		public OperationResult RestoreEmployee(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_сотрудника, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.EmployeeTranslator.Restore(uid);
			}
		}

		#endregion Employee

		#region Department

		public OperationResult<IEnumerable<ShortDepartment>> GetDepartmentList(DepartmentFilter filter)
		{
			OperationResult<IEnumerable<ShortDepartment>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.DepartmentTranslator.GetList(filter);
			}
			return result;
		}

		public OperationResult<Department> GetDepartmentDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetSingle(uid);
			}
		}

		public OperationResult SaveDepartment(Department item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_отдела, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_отдела, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedDepartment(ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Удаление_отдела, department.Name, uid: department.UID);
			foreach (var childDepartment in department.ChildDepartments)
			{
				AddJournalMessage(JournalEventNameType.Удаление_отдела, childDepartment.Value, uid: childDepartment.Key);
			}
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.MarkDeleted(department.UID);
			}
		}

		public OperationResult SaveDepartmentChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_отдела, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreDepartment(ShortDepartment department)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_отдела, department.Name, uid: department.UID);
			foreach (var parent in department.ParentDepartments)
			{
				AddJournalMessage(JournalEventNameType.Восстановление_отдела, parent.Value, uid: parent.Key);
			}
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.Restore(department.UID);
			}
		}

		public OperationResult<IEnumerable<Guid>> GetChildEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetChildEmployeeUIDs(uid);
			}
		}

		public OperationResult<IEnumerable<Guid>> GetParentEmployeeUIDs(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DepartmentTranslator.GetParentEmployeeUIDs(uid);
			}
		}

		#endregion Department

		#region Position

		public OperationResult<IEnumerable<ShortPosition>> GetPositionList(PositionFilter filter)
		{
			OperationResult<IEnumerable<ShortPosition>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.PositionTranslator.GetList(filter);
			}
			return result;
		}

		public OperationResult<Position> GetPositionDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.GetSingle(uid);
			}
		}

		public OperationResult SavePosition(Position item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_должности, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_должности, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedPosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_должности, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestorePosition(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_должности, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PositionTranslator.Restore(uid);
			}
		}

		#endregion Position

		#region Card

		public OperationResult<IEnumerable<SKDCard>> GetCards(CardFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.Get(filter);
			}
		}

		public OperationResult<IEnumerable<SKDCard>> GetEmployeeCards(Guid employeeUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.GetEmployeeCards(employeeUID);
			}
		}

		/// <summary>
		/// Метод сброса флага повторного прохода для коллекции карт
		/// </summary>
		/// <param name="cardsToReset">Словарь карт для сброса. Ключ - карта, Значение - список UID точек доступа, подлежащих сбросу</param>
		/// <param name="cardNo">Номер карты</param>
		/// <param name="doorName">Название ТД</param>
		/// <param name="organisationName">Название организации</param>
		/// <returns>Если во время операции не возникло ошибок, то true</returns>
		public OperationResult<bool> ResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo, string doorName, string organisationName)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				SetJournalMessageForResetRepeatEnter(cardsToReset, cardNo, doorName, organisationName);
				var errors = new List<string>();

				foreach (KeyValuePair<SKDCard, List<Guid>> cardToReset in cardsToReset)
				{
					var cardWriter = Processor.ResetRepeatEnter(cardToReset.Key, cardToReset.Value);
					var cardWriterError = cardWriter.GetError();
					if (!String.IsNullOrEmpty(cardWriterError))
						errors.Add(cardWriterError);

					var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);
					var pendingResult = databaseService.CardTranslator.ResetRepeatEnterPendingList(cardToReset.Key.UID, failedControllerUIDs);
					if (pendingResult.HasError)
						errors.Add(pendingResult.Error);
				}

				return OperationResult<bool>.FromError(errors, true);
			}
		}

		private void SetJournalMessageForResetRepeatEnter(Dictionary<SKDCard, List<Guid>> cardsToReset, int? cardNo, string doorName, string organisationName)
		{
			var employeeName =
					cardsToReset.Keys.FirstOrDefault() != null
					? cardsToReset.Keys.FirstOrDefault().EmployeeName
					: string.Empty;

			if (cardNo != null && !string.IsNullOrEmpty(doorName))
			{
				AddJournalMessage(JournalEventNameType.Сброс_антипессбэка_для_выбранной_ТД, employeeName,
					descriptionText: string.Format("{0}, {1}", cardNo, doorName));
			}
			else if (cardNo != null)
			{
				AddJournalMessage(JournalEventNameType.Сброс_антипессбэка_для_всех_ТД, employeeName, descriptionText: cardNo.ToString());
			}
			else
			{
				AddJournalMessage(JournalEventNameType.Сброс_антипессбэка_для_всех_пропусков, organisationName);
			}
		}

		public OperationResult<bool> AddCard(SKDCard card, string employeeName)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				if (!_licenseManager.CanAddCard(databaseService.CardTranslator.GetCardsCount()))
					return OperationResult<bool>.FromError(string.Format(CommonErrors.LicenseAddCard_Error, _licenseManager.CurrentLicense.TotalUsers));

				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				errors.AddRange(AddStrazhCard(card, getAccessTemplateOperationResult.Result, databaseService));

				AddJournalMessage(JournalEventNameType.Добавление_карты, employeeName, uid: card.EmployeeUID);
				return OperationResult<bool>.FromError(errors, true);
			}
		}

		private IEnumerable<string> AddStrazhCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = StrazhDeviceSDK.Processor.AddCard(card, accessTemplate);
			var cardWriterError = cardWriter.GetError();
			if (!String.IsNullOrEmpty(cardWriterError))
				yield return cardWriterError;

			var failedControllerUIDs = GetFailedControllerUIDs(cardWriter);
			var pendingResult = databaseService.CardTranslator.AddPendingList(card.UID, failedControllerUIDs);
			if (pendingResult.HasError)
				yield return pendingResult.Error;
		}

		public OperationResult<bool> EditCard(SKDCard card, string employeeName)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_карты, employeeName, uid: card.EmployeeUID);

			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				var operationResult = databaseService.CardTranslator.GetSingle(card.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					errors.AddRange(EditStrazhCard(oldCard, oldGetAccessTemplateOperationResult.Result, card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add(CommonErrors.PreviousPasscardNotFound_Error);
				}

				var saveResult = databaseService.CardTranslator.Save(card);
				if (saveResult.HasError)
					return OperationResult<bool>.FromError(saveResult.Error);

				return OperationResult<bool>.FromError(errors, true);
			}
		}

		private IEnumerable<string> EditStrazhCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = StrazhDeviceSDK.Processor.EditCard(oldCard, oldAccessTemplate, card, accessTemplate);
			var cardWriterError = cardWriter.GetError();
			if (!String.IsNullOrEmpty(cardWriterError))
				yield return cardWriterError;

			var pendingResult = databaseService.CardTranslator.EditPendingList(card.UID, GetFailedControllerUIDs(cardWriter));
			if (pendingResult.HasError)
				yield return pendingResult.Error;
		}

		public OperationResult<bool> DeleteCardFromEmployee(SKDCard card, string employeeName, string reason = null)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddJournalMessage(JournalEventNameType.Удаление_карты, employeeName, uid: card.EmployeeUID);

				var errors = new List<string>();

				var getAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(card.AccessTemplateUID);
				if (getAccessTemplateOperationResult.HasError)
					errors.AddRange(getAccessTemplateOperationResult.Errors);

				var operationResult = databaseService.CardTranslator.GetSingle(card.UID);
				if (!operationResult.HasError && operationResult.Result != null)
				{
					var oldCard = operationResult.Result;
					var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(oldCard.AccessTemplateUID);

					errors.AddRange(DeleteStrazhCard(card, getAccessTemplateOperationResult.Result, databaseService));
				}
				else
				{
					errors.Add(CommonErrors.PreviousPasscardNotFound_Error);
				}

				var cardToDelete = new SKDCard
				{
					UID = card.UID,
					Number = card.Number,
					HolderUID = null,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now,
					UserTime = 0,
					DeactivationControllerUID = Guid.Empty,
					CardDoors = new List<CardDoor>(),
					PassCardTemplateUID = null,
					AccessTemplateUID = null,
					CardType = card.CardType,
					Password = null,
					IsInStopList = true,
					StopReason = reason,
					EmployeeName = null,
					EmployeeUID = Guid.Empty,
					OrganisationUID = Guid.Empty,
				};

				var saveResult = databaseService.CardTranslator.Save(cardToDelete);
				if (saveResult.HasError)
				{
					return OperationResult<bool>.FromError(saveResult.Error);
				}

				// Уведомляем подключенных Клиентов о деактивации карты
				card.IsInStopList = true;
				card.StopReason = reason;
				NotifyCardDeactivated(card);

				return OperationResult<bool>.FromError(errors, true);
			}
		}

		private IEnumerable<string> DeleteStrazhCard(SKDCard card, AccessTemplate accessTemplate, SKDDatabaseService databaseService)
		{
			var cardWriter = StrazhDeviceSDK.Processor.DeleteCard(card, accessTemplate);
			var cardWriterError = cardWriter.GetError();
			if (!String.IsNullOrEmpty(cardWriterError))
				yield return cardWriterError;

			var pendingResult = databaseService.CardTranslator.DeletePendingList(card.UID, GetFailedControllerUIDs(cardWriter));
			if (pendingResult.HasError)
				yield return pendingResult.Error;
		}

		public OperationResult DeletedCard(SKDCard card)
		{
			AddJournalMessage(JournalEventNameType.Удаление_карты, card.Number.ToString(), JournalEventDescriptionType.Удаление,
				string.Format(CommonResources.PasscardWithNumber, card.Number), uid: card.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.Delete(card.UID);
			}
		}

		public OperationResult SaveCardTemplate(SKDCard card)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.SavePassTemplate(card);
			}
		}

		private IEnumerable<Guid> GetFailedControllerUIDs(CardWriter cardWriter)
		{
			return cardWriter.ControllerCardItems.Where(x => x.HasError).Select(x => x.ControllerDevice.UID);
		}

		#endregion Card

		#region AccessTemplate

		public OperationResult<IEnumerable<AccessTemplate>> GetAccessTemplates(AccessTemplateFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.Get(filter);
			}
		}

		public OperationResult<bool> SaveAccessTemplate(AccessTemplate item, bool isNew)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				// В журнал событий дабавляем запись о факте добавления/редактирования шаблона доступа
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_доступа, item.Name, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_доступа, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);

				var oldGetAccessTemplateOperationResult = databaseService.AccessTemplateTranslator.GetSingle(item.UID);
				var saveResult = databaseService.AccessTemplateTranslator.Save(item);

				var errors = new List<string>();
				if (saveResult.HasError)
					errors.Add(saveResult.Error);

				var operationResult = databaseService.CardTranslator.GetByAccessTemplateUID(item.UID);
				if (operationResult.Result != null)
				{
					foreach (var card in operationResult.Result)
					{
						var cardWriter = StrazhDeviceSDK.Processor.EditCard(card, oldGetAccessTemplateOperationResult.Result, card, item);
						var cardWriterError = cardWriter.GetError();
						var pendingResult = databaseService.CardTranslator.EditPendingList(item.UID, GetFailedControllerUIDs(cardWriter));

						if (!String.IsNullOrEmpty(cardWriterError))
							errors.Add(cardWriterError);
						if (pendingResult.HasError)
							errors.Add(pendingResult.Error);
					}
				}

				return OperationResult<bool>.FromError(errors, !saveResult.HasError);
			}
		}

		public OperationResult MarkDeletedAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreAccessTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_доступа, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AccessTemplateTranslator.Restore(uid);
			}
		}

		#endregion AccessTemplate

		#region Organisation

		public OperationResult<IEnumerable<Organisation>> GetOrganisations(OrganisationFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Get(filter);
			}
		}

		public OperationResult SaveOrganisation(OrganisationDetails item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_организации, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_организации, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				var errors = new List<string>();
				var cards = databaseService.CardTranslator.Get(new CardFilter { EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new List<Guid> { uid } }, DeactivationType = LogicalDeletationType.Active });
				if (cards.HasError) return new OperationResult(cards.Error);

				foreach (var card in cards.Result)
				{
					var cardResult = DeleteCardFromEmployee(card, name);
					if (cardResult.HasError)
						errors.Add(cardResult.Error);
				}

				var markDeleledResult = databaseService.OrganisationTranslator.MarkDeleted(uid);
				if (markDeleledResult.HasError)
				{
					errors.Add(string.Format(CommonErrors.DB_Error, markDeleledResult.Error));
				}

				return errors.Count > 0 ? new OperationResult(String.Join("\n", errors)) : new OperationResult();
			}
		}

		public OperationResult SaveOrganisationDoors(Organisation item)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveDoors(item);
			}
		}

		public OperationResult SaveOrganisationUsers(Organisation item)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveUsers(item);
			}
		}

		public OperationResult<OrganisationDetails> GetOrganisationDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.GetDetails(uid);
			}
		}

		public OperationResult SaveOrganisationChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveChief(uid, chiefUID);
			}
		}

		public OperationResult SaveOrganisationHRChief(Guid uid, Guid chiefUID, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_организации, name, JournalEventDescriptionType.Редактирование, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.SaveHRChief(uid, chiefUID);
			}
		}

		public OperationResult RestoreOrganisation(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_организации, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Restore(uid);
			}
		}

		public OperationResult<bool> IsAnyOrganisationItems(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.IsAnyItems(uid);
			}
		}

		#endregion Organisation

		#region AdditionalColumnType

		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetAdditionalColumnTypeList(AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetList(filter);
			}
		}

		public OperationResult<IEnumerable<AdditionalColumnType>> GetAdditionalColumnTypes(AdditionalColumnTypeFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Get(filter);
			}
		}

		public OperationResult<AdditionalColumnType> GetAdditionalColumnTypeDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.GetSingle(uid);
			}
		}

		public OperationResult SaveAdditionalColumnType(AdditionalColumnType item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_новой_дополнительной_колонки, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дополнительной_колонки, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreAdditionalColumnType(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дополнительной_колонки, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.AdditionalColumnTypeTranslator.Restore(uid);
			}
		}

		#endregion AdditionalColumnType

		#region NightSettings

		public OperationResult<NightSettings> GetNightSettingsByOrganisation(Guid organisationUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.NightSettingsTranslator.GetByOrganisation(organisationUID);
			}
		}

		public OperationResult SaveNightSettings(NightSettings nightSettings)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.NightSettingsTranslator.Save(nightSettings);
			}
		}

		#endregion NightSettings

		#region Devices

		public OperationResult<SKDStates> SKDGetStates()
		{
			return new OperationResult<SKDStates>(SKDProcessor.SKDGetStates());
		}

		/// <summary>
		/// Получает информацию о контроллере.
		/// Такую как версия прошивки, сетевые настройки, дата и время.
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<SKDDeviceInfo> SKDGetDeviceInfo(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_контроллера, device);
				return Processor.GetDeviceInfo(deviceUID);
			}
			return OperationResult<SKDDeviceInfo>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Устанавливает на контроллере текущее системное время.
		/// В журнал событий заносится соответствующая запись.
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатов выполнения операции</returns>
		public OperationResult<bool> SKDSynchronizeTime(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Синхронизация_времени_контроллера, device);
				return Processor.SynchronizeTime(deviceUID);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDResetController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var isDisallowedHere = default(bool);
				try
				{
					// Проверяем признак выполнения на данном устройстве опасной операции
					isDisallowedHere = DisallowRunCriticalOperationOnDevice(deviceUID);
					if (!isDisallowedHere)
						return OperationResult<bool>.FromError(CommonResources.AnotherOperation);

					// Выполняем критическую операцию
					AddSKDJournalMessage(JournalEventNameType.Сброс_Контроллера, device);
					return Processor.ResetController(deviceUID);
				}
				finally
				{
					// Убираем признак выполнения на данном устройстве опасной операции
					if (isDisallowedHere)
						AllowRunCriticalOperationOnDevice(deviceUID);
				}
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDRebootController(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var isDisallowedHere = default(bool);
				try
				{
					// Проверяем признак выполнения на данном устройстве опасной операции
					isDisallowedHere = DisallowRunCriticalOperationOnDevice(deviceUID);
					if (!isDisallowedHere)
						return OperationResult<bool>.FromError(CommonResources.AnotherOperation);

					// Выполняем критическую операцию
					AddSKDJournalMessage(JournalEventNameType.Перезагрузка_Контроллера, device);
					return Processor.RebootController(deviceUID);
				}
				finally
				{
					// Убираем признак выполнения на данном устройстве опасной операции
					if (isDisallowedHere)
						AllowRunCriticalOperationOnDevice(deviceUID);
				}
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Записывает графики доступа на контроллер
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера, на который производится запись</param>
		/// <returns>Объект OperationResult, описывающий результат выполнения процедуры записи</returns>
		public OperationResult<bool> SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_графиков_доступа, device);
				return Processor.SKDWriteTimeSheduleConfiguration(deviceUID);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Записывает графики доступа на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> SKDWriteAllTimeSheduleConfiguration()
		{
			var errors = new List<string>();
			var failedDeviceUIDs = new List<Guid>();

			var devicesCount = SKDManager.Devices.Count(device => device.Driver.IsController);

			// Показываем прогресс хода выполнения операции
			var progressCallback = Processor.StartProgress(CommonResources.WriteAccessScheduleOnAllControllers, null, devicesCount, true, SKDProgressClientType.Administrator);

			foreach (var device in SKDManager.Devices.Where(device => device.Driver.IsController))
			{
				// Пользователь прервал операцию
				if (progressCallback.IsCanceled)
				{
#if DEBUG
					Logger.Info("Запись графиков доступа на все контроллеры отменена");
#endif
					Processor.StopProgress(progressCallback);
					return OperationResult<List<Guid>>.FromCancel(CommonErrors.WritingAccessScheduleCanceled_Error);
				}

				AddSKDJournalMessage(JournalEventNameType.Запись_графиков_доступа, device);
#if DEBUG
				Logger.Info(String.Format("Запись графиков доступа на контроллер \"{0}\"", device.Name));
#endif
				var result = Processor.SKDWriteTimeSheduleConfiguration(device.UID, false);
				if (result.HasError)
				{
					failedDeviceUIDs.Add(device.UID);
					errors.AddRange(result.Errors);
				}

				// Обновляем прогресс хода выполнения операции
				Processor.DoProgress(null, progressCallback);
			}

			// Останавливаем прогресс хода выполнения операции
			Processor.StopProgress(progressCallback);

			return OperationResult<List<Guid>>.FromError(errors, failedDeviceUIDs);
		}

		/// <summary>
		/// Запрещает выполнение на контроллере опасной операции
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>true - выполнение на контроллере опасной операции запрещено,
		/// false - выполнение на контроллере опасной операции было запрещено ранее</returns>
		private bool DisallowRunCriticalOperationOnDevice(Guid deviceUID)
		{
			lock (_criticalOperationRunningControllerGuidsLock)
			{
				if (_criticalOperationRunningControllerGuids.Contains(deviceUID))
					return false;
				_criticalOperationRunningControllerGuids.Add(deviceUID);
				return true;
			}
		}

		/// <summary>
		/// Разрешает выполнение на контроллере опасной операции
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		private void AllowRunCriticalOperationOnDevice(Guid deviceUID)
		{
			lock (_criticalOperationRunningControllerGuidsLock)
			{
				_criticalOperationRunningControllerGuids.Remove(deviceUID);
			}
		}

		/// <summary>
		/// Перезаписывает на контроллер все пропуска, связанные с ним через точку доступа и организацию
		/// </summary>
		/// <param name="deviceUID">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDRewriteAllCards(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var isDisallowedHere = default(bool);
				try
				{
					// Проверяем признак выполнения на данном устройстве опасной операции
					isDisallowedHere = DisallowRunCriticalOperationOnDevice(deviceUID);
					if (!isDisallowedHere)
						return OperationResult<bool>.FromError(CommonResources.AnotherOperation);

					// Выполняем критическую операцию
					using (var databaseService = new SKDDatabaseService())
					{
						AddSKDJournalMessage(JournalEventNameType.Перезапись_всех_карт, device);
						var cardsResult = databaseService.CardTranslator.Get(new CardFilter());
						var accessTemplatesResult = databaseService.AccessTemplateTranslator.Get(new AccessTemplateFilter());
						if (!cardsResult.HasError && !accessTemplatesResult.HasError)
						{
							return Processor.SKDRewriteAllCards(device, cardsResult.Result, accessTemplatesResult.Result);
						}
						else
						{
							return OperationResult<bool>.FromError(CommonErrors.RecivePasscardsError);
						}
					}
				}
				finally
				{
					// Убираем признак выполнения на данном устройстве опасной операции
					if (isDisallowedHere)
						AllowRunCriticalOperationOnDevice(deviceUID);
				}
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Перезаписывает пропуска на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteCardsOnAllControllers()
		{
			var errors = new List<string>();
			var failedDeviceUIDs = new List<Guid>();

			var devicesCount = SKDManager.Devices.Count(device => device.Driver.IsController);

			// Показываем прогресс хода выполнения операции
			var progressCallback = Processor.StartProgress(CommonResources.WritePasscardsOnAllControllers, null, devicesCount, true, SKDProgressClientType.Administrator);

			foreach (var device in SKDManager.Devices.Where(device => device.Driver.IsController))
			{
				// Пользователь прервал операцию
				if (progressCallback.IsCanceled)
				{
#if DEBUG
					Logger.Info("Запись пропусков на все контроллеры отменена");
#endif
					Processor.StopProgress(progressCallback);
					return OperationResult<List<Guid>>.FromCancel(CommonErrors.WritingPasscardsCanceled_Error);
				}

				OperationResult<bool> result = null;
				using (var databaseService = new SKDDatabaseService())
				{
					AddSKDJournalMessage(JournalEventNameType.Перезапись_всех_карт, device);
#if DEBUG
					Logger.Info(String.Format("Запись пропусков на контроллер \"{0}\"", device.Name));
#endif
					var cardsResult = databaseService.CardTranslator.Get(new CardFilter());
					var accessTemplatesResult = databaseService.AccessTemplateTranslator.Get(new AccessTemplateFilter());
					if (!cardsResult.HasError && !accessTemplatesResult.HasError)
					{
						result = Processor.SKDRewriteAllCards(device, cardsResult.Result, accessTemplatesResult.Result, false);
					}
					else
					{
						result = OperationResult<bool>.FromError(CommonErrors.RecivePasscardsError);
					}
				}
				if (result.HasError)
				{
					failedDeviceUIDs.Add(device.UID);
					errors.AddRange(result.Errors);
				}

				// Обновляем прогресс хода выполнения операции
				Processor.DoProgress(null, progressCallback);
			}

			// Останавливаем прогресс хода выполнения операции
			Processor.StopProgress(progressCallback);

			return OperationResult<List<Guid>>.FromError(errors, failedDeviceUIDs);
		}

		public OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				if (Processor.DeviceProcessors.Single(x => x.Device.UID == device.UID).UpdateFirmware(fileName))
				{
					AddSKDJournalMessage(JournalEventNameType.Обновление_ПО_Контроллера, device);
					return new OperationResult<bool>(true);
				}
				return OperationResult<bool>.FromError(CommonErrors.UpdateFirmware_Error);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<SKDDoorConfiguration> SKDGetDoorConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_конфигурации_двери, device);
				return StrazhDeviceSDK.Processor.GetDoorConfiguration(deviceUID);
			}
			return OperationResult<SKDDoorConfiguration>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDSetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_конфигурации_двери, device);
				return StrazhDeviceSDK.Processor.SetDoorConfiguration(deviceUID, doorConfiguration);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<DoorType> GetControllerDoorType(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_направления_контроллера, device);
				return StrazhDeviceSDK.Processor.GetControllerDoorType(deviceUID);
			}
			return OperationResult<DoorType>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SetControllerDoorType(Guid deviceUID, DoorType doorType)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_направления_контроллера, device);
				return StrazhDeviceSDK.Processor.SetControllerDoorType(deviceUID, doorType);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SetControllerPassword(Guid deviceUID, string name, string oldPassword, string password)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_пароля_контроллера, device);
				return StrazhDeviceSDK.Processor.SetControllerPassword(deviceUID, name, oldPassword, password);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<SKDControllerTimeSettings> GetControllerTimeSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_временных_настроек_контроллера, device);
				return StrazhDeviceSDK.Processor.GetControllerTimeSettings(deviceUID);
			}
			return OperationResult<SKDControllerTimeSettings>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SetControllerTimeSettings(Guid deviceUID, SKDControllerTimeSettings controllerTimeSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_временных_настроек_контроллера, device);
				return StrazhDeviceSDK.Processor.SetControllerTimeSettings(deviceUID, controllerTimeSettings);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<SKDControllerNetworkSettings> GetControllerNetworkSettings(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_сетевых_настроек_контроллера, device);
				return StrazhDeviceSDK.Processor.GetControllerNetworkSettings(deviceUID);
			}
			return OperationResult<SKDControllerNetworkSettings>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SetControllerNetworkSettings(Guid deviceUID, SKDControllerNetworkSettings controllerNetworkSettings)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_сетевых_настроек_контроллера, device);
				return StrazhDeviceSDK.Processor.SetControllerNetworkSettings(deviceUID, controllerNetworkSettings);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SetControllerTimeSchedulesAndLocksPasswords(Guid deviceUID, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			var isDisallowedHere = default(bool);
			try
			{
				// Проверяем признак выполнения на данном устройстве опасной операции
				isDisallowedHere = DisallowRunCriticalOperationOnDevice(deviceUID);
				if (!isDisallowedHere)
					return OperationResult<bool>.FromError(CommonResources.AnotherOperation);

				// Выполняем критическую операцию
				// 1. Записываем графики доступа на контроллер
				var result = SKDWriteTimeSheduleConfiguration(deviceUID);
				// 2. Записываем пароли замков на контроллер
				if (!result.IsCanceled)
					result = SetControllerLocksPasswords(deviceUID, locksPasswords);
				return result;
			}
			finally
			{
				// Убираем признак выполнения на данном устройстве опасной операции
				if (isDisallowedHere)
					AllowRunCriticalOperationOnDevice(deviceUID);
			}
		}

		#region <Пароли замков>

		/// <summary>
		/// Получить список паролей замков на контроллере
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<IEnumerable<SKDLocksPassword>> GetControllerLocksPasswords(Guid deviceUid)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.GetControllerLocksPasswords, device);
				return Processor.GetControllerLocksPasswords(deviceUid);
			}
			return OperationResult<IEnumerable<SKDLocksPassword>>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Записать пароли замков на контроллер
		/// </summary>
		/// <param name="deviceUid">Идентификатор контроллера</param>
		/// <param name="locksPasswords">Список паролей замков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SetControllerLocksPasswords(Guid deviceUid, IEnumerable<SKDLocksPassword> locksPasswords)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.SetControllerLocksPasswords, device);
				return Processor.SetControllerLocksPasswords(deviceUid, locksPasswords);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		/// <summary>
		/// Перезаписывает пароли замков на все контроллеры
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<List<Guid>> RewriteControllerLocksPasswordsOnAllControllers()
		{
			var errors = new List<string>();
			var failedDeviceUIDs = new List<Guid>();

			var devicesCount = SKDManager.Devices.Count(device => device.Driver.IsController);

			// Показываем прогресс хода выполнения операции
			var progressCallback = Processor.StartProgress(CommonResources.WritePasswordsOnAllControllers, null, devicesCount, true, SKDProgressClientType.Administrator);

			foreach (var device in SKDManager.Devices.Where(device => device.Driver.IsController))
			{
				// Пользователь прервал операцию
				if (progressCallback.IsCanceled)
				{
#if DEBUG
					Logger.Info("Запись паролей замков на все контроллеры отменена");
#endif
					Processor.StopProgress(progressCallback);
					return OperationResult<List<Guid>>.FromCancel(CommonErrors.WritingLockPasswordsCanceled_Error);
				}

				AddSKDJournalMessage(JournalEventNameType.SetControllerLocksPasswords, device);
#if DEBUG
				Logger.Info(String.Format("Запись паролей замков на контроллер \"{0}\"", device.Name));
#endif
				var result = Processor.SetControllerLocksPasswords(device.UID, device.ControllerPasswords != null ? device.ControllerPasswords.LocksPasswords : new List<SKDLocksPassword>(), false);
				if (result.HasError)
				{
					failedDeviceUIDs.Add(device.UID);
					errors.AddRange(result.Errors);
				}

				// Обновляем прогресс хода выполнения операции
				Processor.DoProgress(null, progressCallback);
			}

			// Останавливаем прогресс хода выполнения операции
			Processor.StopProgress(progressCallback);

			return OperationResult<List<Guid>>.FromError(errors, failedDeviceUIDs);
		}

		#endregion </Пароли замков>

		#region <Замок>

		public OperationResult<bool> SKDOpenDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_двери, device);
				return StrazhDeviceSDK.Processor.OpenDoor(device);
			}
			else
			{
				return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
			}
		}

		public OperationResult<bool> SKDCloseDevice(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_закрытие_двери, device);
				return StrazhDeviceSDK.Processor.CloseDoor(device);
			}
			else
			{
				return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
			}
		}

		private OperationResult<bool> ChangeLockAccessState(SKDDevice device, AccessState accessState)
		{
			var prevAccessState = device.SKDDoorConfiguration.AccessState;

			// Пытаемся перевести замок в требуемый режим
			device.SKDDoorConfiguration.AccessState = accessState;
			var result = Processor.SetDoorConfiguration(device.UID, device.SKDDoorConfiguration);
			if (result.HasError)
			{
				device.SKDDoorConfiguration.AccessState = prevAccessState;
				return result;
			}

			// Замок может не перейти в требуемый режим, поэтому запрашиваем результатирующую конфигурацию замка
			var checkResult = Processor.GetDoorConfiguration(device.UID);
			if (checkResult.HasError)
				return OperationResult<bool>.FromError(String.Format(CommonErrors.ChangeLockModeNotConfirmedByController_Error, device.Name));
			//return new OperationResult<bool>(false);

			// Замок не перешел в требуемый режим
			if (checkResult.Result.AccessState != accessState)
			{
				device.SKDDoorConfiguration.AccessState = prevAccessState;
				return OperationResult<bool>.FromError(String.Format(CommonErrors.CouldNotChangeLockMode_Error, device.Name));
				//return new OperationResult<bool>(false);
			}

			// Обновляем состояние доменной модели замка
			if (device.State != null)
			{
				device.State.AccessState = accessState;
				var skdStates = new SKDStates();
				skdStates.DeviceStates.Add(device.State);

				Processor.OnStatesChanged(skdStates);
			}
			return new OperationResult<bool>(true);
		}

		/// <summary>
		/// Переводит замок в режим "Открыто/Закрыто/Норма"
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка</param>
		/// <param name="eventNameType">Тип события для вставки в журнал событий</param>
		/// <param name="accessState">Режим, в который переводится замок</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private OperationResult<bool> SKDSetDeviceAccessState(Guid deviceUID, JournalEventNameType eventNameType, AccessState accessState)
		{
			// Ищем замок в конфигурации
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);

			// Если замок не найден в конфигурации, возвращаем ошибку
			if (device == null)
				return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);

			// Фиксируем в журнале событий намерение на преревод замка в требуемый режим
			AddSKDJournalMessage(eventNameType, device);

			return ChangeLockAccessState(device, accessState);
		}

		/// <summary>
		/// Посылает команду на перевод замка в режим "Норма"
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDeviceAccessStateNormal(Guid deviceUID)
		{
			// Пытаемся перевести замок в режим "Номра"
			var result = SKDSetDeviceAccessState(deviceUID, JournalEventNameType.Команда_на_перевод_замка_в_режим_Норма, AccessState.Normal);

			// Фиксируем в журнале событий факт преревода замка в режим "Норма"
			if (!result.HasError && result.Result)
				AddSKDJournalMessage(JournalEventNameType.Перевод_замка_в_режим_Норма, SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID));

			return result;
		}

		/// <summary>
		/// Посылает команду на перевод замка в режим "Закрыто"
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDeviceAccessStateCloseAlways(Guid deviceUID)
		{
			// Пытаемся перевести замок в режим "Открыто"
			var result = SKDSetDeviceAccessState(deviceUID, JournalEventNameType.Команда_на_перевод_замка_в_режим_Закрыто, AccessState.CloseAlways);

			// Фиксируем в журнале событий факт преревода замка в режим "Закрыто"
			if (!result.HasError && result.Result)
				AddSKDJournalMessage(JournalEventNameType.Перевод_замка_в_режим_Закрыто, SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID));

			return result;
		}

		/// <summary>
		/// Посылает команду на перевод замка в режим "Открыто"
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDeviceAccessStateOpenAlways(Guid deviceUID)
		{
			// Пытаемся перевести замок в режим "Открыто"
			var result = SKDSetDeviceAccessState(deviceUID, JournalEventNameType.Команда_на_перевод_замка_в_режим_Открыто, AccessState.OpenAlways);

			// Фиксируем в журнале событий факт преревода замка в режим "Открыто"
			if (!result.HasError && result.Result)
				AddSKDJournalMessage(JournalEventNameType.Перевод_замка_в_режим_Открыто, SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID));

			return result;
		}

		/// <summary>
		/// Сбрасывает состояние "Взлом" замка
		/// </summary>
		/// <param name="deviceUID">Идентификатор замка</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDClearDevicePromptWarning(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);

			if (device == null)
				return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);

			// Фиксируем в журнале событий факт отправки команды на сброс состояния "Взлом" замка
			AddSKDJournalMessage(JournalEventNameType.Команда_на_сброс_состояния_взлом_замка, device);

			// Сбрасываем состояние замка "Взлом"
			var result = Processor.ClearPromptWarning(device);

			if (!result.HasError)
			{
				// Смотрим в каком режиме работает замок после сброса состояние "Взлом"
				var getDoorConfigurationResult = Processor.GetDoorConfiguration(deviceUID);

				// Синхронизируем состояние виртуальной модели замка с ее физической реализацией на контроллере
				if (!getDoorConfigurationResult.HasError)
				{
					device.State.AccessState = getDoorConfigurationResult.Result.AccessState;
					var skdStates = new SKDStates();
					skdStates.DeviceStates.Add(device.State);
					Processor.OnStatesChanged(skdStates);
				}

				// Фиксируем в журнале событий подтверждение на выполнение команды на сброс состояния "Взлом" замка
				AddSKDJournalMessage(JournalEventNameType.Сброс_состояния_взлом_замка, device);
			}
			return result;
		}

		#endregion </Замок>

		#region <Зона>

		public OperationResult<bool> SKDOpenZone(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_зоны, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						var result = Processor.OpenDoor(lockDevice);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.ZoneWithoutLock_Error);
					}
				}
				//if (errors.Count > 0)
				//{
				//	return OperationResult<bool>.FromError(errors);
				//}
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonErrors.ZoneNotFoundInConfig_Error);
		}

		public OperationResult<bool> SKDCloseZone(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_закрытие_зоны, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						var result = Processor.CloseDoor(lockDevice);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.ZoneWithoutLock_Error);
					}
				}
				//if (errors.Count > 0)
				//{
				//	return OperationResult<bool>.FromError(errors);
				//}
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonErrors.ZoneNotFoundInConfig_Error);
		}

		/// <summary>
		/// Переводит зону в режим "Открыто/Закрыто/Норма"
		/// </summary>
		/// <param name="zoneUID">Идентификатор зоны</param>
		/// <param name="eventNameType">Тип события для вставки в журнал событий</param>
		/// <param name="accessState">Режим, в который переводится зона</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private OperationResult<bool> SKDSetZoneAccessState(Guid zoneUID, JournalEventNameType eventNameType, AccessState accessState)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				AddSKDJournalMessage(eventNameType, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						// Пытаемся перевести замок в требуемый режим
						var result = ChangeLockAccessState(lockDevice, accessState);
						if (result.HasError)
						{
							errors.AddRange(result.Errors);
						}
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.ZoneWithoutLock_Error);
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonErrors.ZoneNotFoundInConfig_Error);
		}

		/// <summary>
		/// Посылает команду на перевод зоны в режим "Норма"
		/// </summary>
		/// <param name="zoneUID">Идентификатор зоны</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDZoneAccessStateNormal(Guid zoneUID)
		{
			// Пытаемся перевести зону в режим "Норма"
			var result = SKDSetZoneAccessState(zoneUID, JournalEventNameType.Команда_на_перевод_зоны_в_режим_Норма, AccessState.Normal);

			// Фиксируем в журнале событий факт преревода зоны в режим "Норма"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Норма, SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID));

			return result;
		}

		/// <summary>
		/// Посылает команду на перевод зоны в режим "Закрыто"
		/// </summary>
		/// <param name="zoneUID">Идентификатор зоны</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDZoneAccessStateCloseAlways(Guid zoneUID)
		{
			// Пытаемся перевести зону в режим "Закрыто"
			var result = SKDSetZoneAccessState(zoneUID, JournalEventNameType.Команда_на_перевод_зоны_в_режим_Закрыто, AccessState.CloseAlways);

			// Фиксируем в журнале событий факт преревода зоны в режим "Закрыто"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Закрыто, SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID));

			return result;
		}

		/// <summary>
		/// Посылает команду на перевод зоны в режим "Открыто"
		/// </summary>
		/// <param name="zoneUID">Идентификатор зоны</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDZoneAccessStateOpenAlways(Guid zoneUID)
		{
			// Пытаемся перевести зону в режим "Открыто"
			var result = SKDSetZoneAccessState(zoneUID, JournalEventNameType.Команда_на_перевод_зоны_в_режим_Открыто, AccessState.OpenAlways);

			// Фиксируем в журнале событий факт преревода зоны в режим "Открыто"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Открыто, SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID));

			return result;
		}

		/// <summary>
		/// Сбрасывает состояние "Взлом" зоны
		/// </summary>
		/// <param name="zoneUID">Идентификатор зоны</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDClearZonePromptWarning(Guid zoneUID)
		{
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				// Фиксируем в журнале событий факт отправки команды на сброс состояния "Взлом" зоны
				AddSKDJournalMessage(JournalEventNameType.Команда_на_сброс_состояния_взлом_зоны, zone);
				var errors = new List<string>();
				foreach (var device in zone.Devices)
				{
					var lockAddress = device.IntAddress;
					if (device.Parent != null && device.Parent.DoorType == DoorType.TwoWay)
					{
						lockAddress = device.IntAddress / 2;
					}
					var lockDevice = device.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						var result = Processor.ClearPromptWarning(lockDevice);
						if (!result.HasError)
						{
							// Смотрим в каком режиме работает замок после сброса состояние "Взлом"
							var getDoorConfigurationResult = Processor.GetDoorConfiguration(lockDevice.UID);

							// Синхронизируем состояние виртуальной модели замка с ее физической реализацией на контроллере
							if (!getDoorConfigurationResult.HasError)
							{
								lockDevice.State.AccessState = getDoorConfigurationResult.Result.AccessState;
								var skdStates = new SKDStates();
								skdStates.DeviceStates.Add(device.State);
								Processor.OnStatesChanged(skdStates);
							}
						}
						else
							errors.AddRange(result.Errors);
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.ZoneWithoutLock_Error);
					}
				}
				if (errors.Count > 0)
				{
					return OperationResult<bool>.FromError(errors);
				}

				// Фиксируем в журнале событий подтверждение на выполнение команды на сброс состояния "Взлом" зоны
				AddSKDJournalMessage(JournalEventNameType.Сброс_состояния_взлом_зоны, zone);

				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError(CommonErrors.ZoneNotFoundInConfig_Error);
		}

		#endregion </Зона>

		#region <Точка доступа>

		public OperationResult<bool> SKDOpenDoor(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_открытие_точки_доступа, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						return StrazhDeviceSDK.Processor.OpenDoor(lockDevice);
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.DoorWithoutLock_Error);
					}
				}
				else
				{
					return OperationResult<bool>.FromError(CommonErrors.DoorWithoutEntranceDevice_Error);
				}
			}
			else
			{
				return OperationResult<bool>.FromError(CommonErrors.DoorNotFound_Error);
			}
		}

		public OperationResult<bool> SKDCloseDoor(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Команда_на_закрытие_точки_доступа, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						return StrazhDeviceSDK.Processor.CloseDoor(lockDevice);
					}
					else
					{
						return OperationResult<bool>.FromError(CommonErrors.DoorWithoutLock_Error);
					}
				}
				else
				{
					return OperationResult<bool>.FromError(CommonErrors.DoorWithoutEntranceDevice_Error);
				}
			}
			else
			{
				return OperationResult<bool>.FromError(CommonErrors.DoorNotFound_Error);
			}
		}

		/// <summary>
		/// Переводит точку доступа в режим "Открыто/Закрыто/Норма"
		/// </summary>
		/// <param name="doorUID">Идентификатор точки доступа</param>
		/// <param name="eventNameType">Тип события для вставки в журнал событий</param>
		/// <param name="accessState">Режим, в который переводится точка доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private OperationResult<bool> SKDSetDoorAccessState(Guid doorUID, JournalEventNameType eventNameType, AccessState accessState)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				AddSKDJournalMessage(eventNameType, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
						return ChangeLockAccessState(lockDevice, accessState);
					return OperationResult<bool>.FromError(CommonErrors.DoorWithoutLock_Error);
				}
				return OperationResult<bool>.FromError(CommonErrors.DoorWithoutEntranceDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonErrors.DoorNotFound_Error);
		}

		/// <summary>
		/// Посылает команду на перевод точки доступа в режим "Норма"
		/// </summary>
		/// <param name="doorUID">Идентификатор точки доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDoorAccessStateNormal(Guid doorUID)
		{
			// Пытаемся перевести точку доступа в режим "Норма"
			var result = SKDSetDoorAccessState(doorUID, JournalEventNameType.Команда_на_перевод_точки_доступа_в_режим_Норма, AccessState.Normal);

			// Фиксируем в журнале событий факт преревода точки доступа в режим "Норма"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Норма, SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID));

			return result;
		}

		/// <summary>
		///Посылает команду на перевод точки доступа в режим "Закрыто"
		/// </summary>
		/// <param name="doorUID">Идентификатор точки доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDoorAccessStateCloseAlways(Guid doorUID)
		{
			// Пытаемся перевести точку доступа в режим "Закрыто"
			var result = SKDSetDoorAccessState(doorUID, JournalEventNameType.Команда_на_перевод_точки_доступа_в_режим_Закрыто, AccessState.CloseAlways);

			// Фиксируем в журнале событий факт преревода точки доступа в режим "Закрыто"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Закрыто, SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID));

			return result;
		}

		/// <summary>
		///Посылает команду на перевод точки доступа в режим "Открыто"
		/// </summary>
		/// <param name="doorUID">Идентификатор точки доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDDoorAccessStateOpenAlways(Guid doorUID)
		{
			// Пытаемся перевести точку доступа в режим "Открыто"
			var result = SKDSetDoorAccessState(doorUID, JournalEventNameType.Команда_на_перевод_точки_доступа_в_режим_Открыто, AccessState.OpenAlways);

			// Фиксируем в журнале событий факт преревода точки доступа в режим "Открыто"
			if (!result.HasError)
				AddSKDJournalMessage(JournalEventNameType.Перевод_точки_доступа_в_режим_Открыто, SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID));

			return result;
		}

		/// <summary>
		/// Сбрасывает состояние "Взлом" для точки доступа
		/// </summary>
		/// <param name="doorUID">Идентификатор точки доступа</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> SKDClearDoorPromptWarning(Guid doorUID)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
			if (door != null)
			{
				// Фиксируем в журнале событий факт отправки команды на сброс состояния "Взлом" точки доступа
				AddSKDJournalMessage(JournalEventNameType.Команда_на_сброс_состояния_взлом_точки_доступа, door);
				if (door.InDevice != null)
				{
					var lockAddress = door.InDevice.IntAddress;
					if (door.DoorType == DoorType.TwoWay)
					{
						lockAddress = door.InDevice.IntAddress / 2;
					}
					var lockDevice = door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
					if (lockDevice != null)
					{
						// Сбрасываем состояние замка "Взлом"
						var result = Processor.ClearPromptWarning(lockDevice);

						if (!result.HasError)
						{
							// Смотрим в каком режиме работает замок после сброса состояние "Взлом"
							var getDoorConfigurationResult = Processor.GetDoorConfiguration(lockDevice.UID);

							// Синхронизируем состояние виртуальной модели замка с ее физической реализацией на контроллере
							if (!getDoorConfigurationResult.HasError)
							{
								lockDevice.State.AccessState = getDoorConfigurationResult.Result.AccessState;
								var skdStates = new SKDStates();
								skdStates.DeviceStates.Add(lockDevice.State);
								Processor.OnStatesChanged(skdStates);
							}

							// Фиксируем в журнале событий подтверждение на выполнение команды на сброс состояния "Взлом" точки доступа
							AddSKDJournalMessage(JournalEventNameType.Сброс_состояния_взлом_точки_доступа, door);
						}
						return result;
					}
					return OperationResult<bool>.FromError(CommonErrors.DoorWithoutLock_Error);
				}
				return OperationResult<bool>.FromError(CommonErrors.DoorWithoutEntranceDevice_Error);
			}
			return OperationResult<bool>.FromError(CommonErrors.DoorNotFound_Error);
		}

		#endregion </Точка доступа>

		public OperationResult<SKDAntiPassBackConfiguration> SKDGetAntiPassBackConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_antipassback_настройки_контроллера, device);
				return StrazhDeviceSDK.Processor.GetAntiPassBackConfiguration(deviceUID);
			}
			return OperationResult<SKDAntiPassBackConfiguration>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDSetAntiPassBackConfiguration(Guid deviceUID, SKDAntiPassBackConfiguration antiPassBackConfiguration)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_antipassback_настройки_контроллера, device);
				return StrazhDeviceSDK.Processor.SetAntiPassBackConfiguration(deviceUID, antiPassBackConfiguration);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<SKDInterlockConfiguration> SKDGetInterlockConfiguration(Guid deviceUID)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запрос_interlock_настройки_контроллера, device);
				return StrazhDeviceSDK.Processor.GetInterlockConfiguration(deviceUID);
			}
			return OperationResult<SKDInterlockConfiguration>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDSetInterlockConfiguration(Guid deviceUID, SKDInterlockConfiguration interlockConfiguration)
		{
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				AddSKDJournalMessage(JournalEventNameType.Запись_interlock_настройки_контроллера, device);
				return Processor.SetInterlockConfiguration(deviceUID, interlockConfiguration);
			}
			return OperationResult<bool>.FromError(CommonErrors.DeviceNotFound_Error);
		}

		public OperationResult<bool> SKDStartSearchDevices()
		{
			return StrazhDeviceSDK.Processor.StartSearchDevices();
		}
		public OperationResult<bool> SKDStopSearchDevices()
		{
			return StrazhDeviceSDK.Processor.StopSearchDevices();
		}


		#endregion

		#region PassCardTemplate

		public OperationResult<IEnumerable<ShortPassCardTemplate>> GetPassCardTemplateList(PassCardTemplateFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.GetList(filter);
			}
		}

		public OperationResult<IEnumerable<PassCardTemplate>> GetFullPassCardTemplateList(PassCardTemplateFilter filter)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.PassCardTemplateTranslator.GetFullList(filter);
			}
		}

		public OperationResult<IEnumerable<Tuple<Guid, string>>> GetTemplateNames(Guid organisationId)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.PassCardTemplateTranslator.GetTemplateNames(organisationId);
			}
		}

		public OperationResult<PassCardTemplate> GetPassCardTemplateDetails(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.GetSingle(uid);
			}
		}

		public OperationResult SavePassCardTemplate(PassCardTemplate item, bool isNew)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				var result = databaseService.PassCardTemplateTranslator.Save(item);

				if (result.HasError)
					return result;

				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_шаблона_пропуска, item.Caption, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_шаблона_пропуска, item.Caption, JournalEventDescriptionType.Редактирование, uid: item.UID);

				return result;
			}
		}

		public OperationResult MarkDeletedPassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_шаблона_пропуска, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestorePassCardTemplate(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_шаблона_пропуска, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassCardTemplateTranslator.Restore(uid);
			}
		}

		#endregion PassCardTemplate

		public OperationResult ResetSKDDatabase()
		{
			return PatchManager.Reset_SKD();
		}

		public OperationResult SaveJournalVideoUID(Guid journalItemUID, Guid videoUID, Guid cameraUID)
		{
			using (var journalTranslator = new JournalTranslator())
			{
				return journalTranslator.SaveVideoUID(journalItemUID, videoUID, cameraUID);
			}
		}

		public OperationResult SaveJournalCameraUID(Guid journalItemUID, Guid CameraUID)
		{
			using (var journalTranslator = new JournalTranslator())
			{
				return journalTranslator.SaveCameraUID(journalItemUID, CameraUID);
			}
		}

		#region Export

		public OperationResult ExportOrganisation(ExportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Export(filter);
			}
		}

		public OperationResult ImportOrganisation(ImportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.Import(filter);
			}
		}

		public OperationResult ExportOrganisationList(ExportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.ListSynchroniser.Export(filter);
			}
		}

		public OperationResult ImportOrganisationList(ImportFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.OrganisationTranslator.Synchroniser.ListSynchroniser.Import(filter);
			}
		}

		public OperationResult ExportJournal(JournalExportFilter filter)
		{
			var journalResult = new OperationResult();
			var passJournalResult = new OperationResult();
			if (filter.IsExportJournal)
			{
				using (var journalSynchroniser = new JounalSynchroniser())
				{
					journalResult = journalSynchroniser.Export(filter);
				}
			}
			if (filter.IsExportPassJournal)
			{
				using (var databaseService = new SKDDatabaseService())
				{
					passJournalResult = databaseService.PassJournalTranslator.Synchroniser.Export(filter);
				}
			}
			return TranslatiorHelper.ConcatOperationResults(journalResult, passJournalResult);
		}

		public OperationResult ExportConfiguration(ConfigurationExportFilter filter)
		{
			return ConfigurationSynchroniser.Export(filter);
		}

		public OperationResult ExportReport(ReportExportFilter filter)
		{
			return ReportingHelpers.ExportReport(filter);
		}

		#endregion Export

		public OperationResult<bool> UpdatePassCardOriginalImage(Guid? idToRemove, Attachment attachment)
		{
			if (idToRemove.HasValue)
			{
				var removeResult = RemoveFile(idToRemove.Value);
				if (removeResult.HasError)
					return new OperationResult<bool>(false);
			}

			if (attachment == null)
				return new OperationResult<bool>(true);

			var uploadResult = UploadFile(attachment);

			return uploadResult.HasError
				? new OperationResult<bool>(false)
				: new OperationResult<bool>(true);
		}

		public OperationResult<List<Attachment>> UploadPassCardImages()
		{
			using (var db = new SKDDatabaseService())
			{
				var templates = db.PassCardTemplateTranslator.Get(new PassCardTemplateFilter()).Result;

				var result = new List<Attachment>();

				foreach (var template in templates)
				{
					var frontAttachment = GetAttachmentForTemplateSide(template.Front);
					var backAttachment = GetAttachmentForTemplateSide(template.Back);

					if (frontAttachment != null)
						result.Add(frontAttachment);
					if (backAttachment != null)
						result.Add(backAttachment);
				}

				return new OperationResult<List<Attachment>>(result);
			}
		}

		private Attachment GetAttachmentForTemplateSide(PassCardTemplateSide side)
		{
			if (side.WatermarkImage == null || !side.WatermarkImage.OriginalImageGuid.HasValue) return null;

			var result = DownloadFile(side.WatermarkImage.OriginalImageGuid.Value);

			return result.HasError ? null : result.Result;
		}
		#region <Attachment>

		/// <summary>
		/// Выгружает файл на Сервер приложений
		/// </summary>
		/// <param name="attachment">Метаданные выгружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<Guid> UploadFile(Attachment attachment)
		{
			var dir = AppServerSettingsHelper.AppServerSettings.AttachmentsFolder;
			try
			{
				// Сохраняем файл в хранилище
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				var filePath = Path.Combine(dir, attachment.UID.ToString());
				File.WriteAllBytes(filePath, attachment.Data);

				// Сохраняем метаданные сохраненного файла
				OperationResult operationResult;
				using (var databaseService = new SKDDatabaseService())
				{
					operationResult = databaseService.AttachmentTranslator.Save(attachment);
				}
				if (operationResult.HasError)
					return OperationResult<Guid>.FromError(String.Format(CommonErrors.Transfer_Error, attachment.FileName));
			}
			catch (Exception e)
			{
				return OperationResult<Guid>.FromError(String.Format(CommonErrors.TransferFile_Error, attachment.FileName, e.Message));
			}
			return new OperationResult<Guid>(attachment.UID);
		}

		/// <summary>
		/// Загружает файл с Сервера приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных загружаемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<Attachment> DownloadFile(Guid attachmentUID)
		{
			var fileName = Path.Combine(AppServerSettingsHelper.AppServerSettings.AttachmentsFolder, attachmentUID.ToString());
			try
			{
				Attachment attachment;
				using (var databaseService = new SKDDatabaseService())
				{
					var operationResult = databaseService.AttachmentTranslator.GetSingle(attachmentUID);
					if (operationResult.HasError)
						return OperationResult<Attachment>.FromError(CommonErrors.ReciveFile_Error);
					attachment = operationResult.Result;
				}
				attachment.Data = File.ReadAllBytes(fileName);
				return new OperationResult<Attachment>(attachment);
			}
			catch (Exception e)
			{
				return OperationResult<Attachment>.FromError(e.Message);
			}
		}

		/// <summary>
		/// Удаляет файл из хранилища на Сервере приложений
		/// </summary>
		/// <param name="attachmentUID">Идентификатор метаданных удаляемого файла</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> RemoveFile(Guid attachmentUID)
		{
			var fileName = Path.Combine(AppServerSettingsHelper.AppServerSettings.AttachmentsFolder, attachmentUID.ToString());
			try
			{
				// Удаляем файл из хранилища
				File.Delete(fileName);

				// Удаляем метаинформацию удаленного файла
				using (var databaseService = new SKDDatabaseService())
				{
					var getSingleOperationResult = databaseService.AttachmentTranslator.GetSingle(attachmentUID);
					if (getSingleOperationResult.HasError)
						return OperationResult<bool>.FromError(CommonErrors.Delete_Error);

					var attachmentFileName = getSingleOperationResult.Result.FileName;

					var deleteOperationResult = databaseService.AttachmentTranslator.Delete(attachmentUID);
					if (deleteOperationResult.HasError)
						return OperationResult<bool>.FromError(String.Format(CommonErrors.Delete_Error, attachmentFileName));
				}
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(String.Format(CommonErrors.DeleteFile_Error, e.Message));
			}
			return new OperationResult<bool>();
		}

		#endregion </Attachment>
	}
}