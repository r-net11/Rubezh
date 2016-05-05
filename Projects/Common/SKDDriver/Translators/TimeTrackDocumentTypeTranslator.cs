using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using OperationResult = FiresecAPI.OperationResult;
using Organisation = FiresecAPI.SKD.Organisation;
using TimeTrackDocumentType = FiresecAPI.SKD.TimeTrackDocumentType;

namespace StrazhDAL
{
	public class TimeTrackDocumentTypeTranslator
	{
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		protected List<TimeTrackDocumentType> GetAllSystemDocuments()
		{
			return new List<TimeTrackDocumentType>
			{
				new TimeTrackDocumentType("Продолжительность работы в дневное время", "Я", 1, DocumentType.Presence),
				new TimeTrackDocumentType("Продолжительность работы в ночное время", "Н", 2, DocumentType.Presence),
				new TimeTrackDocumentType("Продолжительность работы в выходные и нерабочие праздничные дни", "РВ", 3,
					DocumentType.Overtime),
				new TimeTrackDocumentType("Продолжительность сверхурочной работы", "С", 4, DocumentType.Overtime),
				new TimeTrackDocumentType("Продолжительность работы вахтовым методом", "ВМ", 5, DocumentType.Presence),
				new TimeTrackDocumentType("Служебная командировка", "К", 6, DocumentType.Presence),
				new TimeTrackDocumentType("Повышение квалификации с отрывом от работы", "ПК", 7, DocumentType.Presence),
				new TimeTrackDocumentType("Повышение квалификации с отрывом от работы в другой местности", "ПМ", 8,
					DocumentType.Presence),
				new TimeTrackDocumentType("Ежегодный основной оплачиваемый отпуск", "ОТ", 9, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Ежегодный дополнительный оплачиваемый отпуск", "ОД", 10, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением",
					"У", 11, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы",
					"УВ", 12, DocumentType.Presence),
				new TimeTrackDocumentType("Дополнительный отпуск в связи с обучением без сохранения заработной платы", "УД", 13,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)", "Р",
					14, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Отпуск по уходу за ребенком до достижения им возраста трех лет", "ОЖ", 15,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя", "ДО", 16,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации",
					"ОЗ", 17, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Ежегодный дополнительный отпуск без сохранения заработной платы", "ДБ", 18,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Временная нетрудоспособность (кроме случаев, предусмотренных кодом 'Т') с назначением пособия согласно законодательству",
					"Б", 19, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством", "Т", 20,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством",
					"ЛЧ", 21, DocumentType.Presence),
				new TimeTrackDocumentType(
					"Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе",
					"ПВ", 22, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству", "Г", 23,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)",
					"ПР", 24, DocumentType.Absence),
				new TimeTrackDocumentType(
					"Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством",
					"НС", 25, DocumentType.Presence),
				new TimeTrackDocumentType("Выходные дни (еженедельный отпуск) и нерабочие праздничные дни", "В", 26,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Дополнительные выходные дни (оплачиваемые)", "ОВ", 27, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Дополнительные выходные дни (без сохранения заработной платы)", "НВ", 28,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Забастовка (при условиях и в порядке, предусмотренных законом)", "ЗБ", 29,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType("Неявки по невыясненным причинам (до выяснения обстоятельств)", "НН", 30, DocumentType.Absence),
				new TimeTrackDocumentType("Время простоя по вине работодателя", "РП", 31, DocumentType.Presence),
				new TimeTrackDocumentType("Время простоя по причинам, не зависящим от работодателя и работника", "НП", 32,
					DocumentType.Presence),
				new TimeTrackDocumentType("Время простоя по вине работника", "ВП", 33, DocumentType.Absence),
				new TimeTrackDocumentType(
					"Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством", "НО", 34,
					DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(
					"Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы",
					"НБ", 35, DocumentType.Absence),
				new TimeTrackDocumentType("Время приостановки работы в случае задержки выплаты заработной платы", "НЗ", 36,
					DocumentType.AbsenceReasonable)
			};
		}

		public TimeTrackDocumentTypeTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		public OperationResult<List<TimeTrackDocumentType>> Get(Guid organisationUID)
		{
			return Get(organisationUID, Context.TimeTrackDocumentTypes);
		}

		public OperationResult<List<TimeTrackDocumentType>> Get(Guid organisationUID, IEnumerable<DataAccess.TimeTrackDocumentType> tableItems)
		{
			try
			{
				var timeTrackDocumentTypes = tableItems
					.Where(x => x.OrganisationUID == organisationUID)
					.Select(tableTimeTrackDocumentType => new TimeTrackDocumentType
					{
						UID = tableTimeTrackDocumentType.UID,
						OrganisationUID = tableTimeTrackDocumentType.OrganisationUID,
						Name = tableTimeTrackDocumentType.Name,
						ShortName = tableTimeTrackDocumentType.ShortName,
						Code = tableTimeTrackDocumentType.DocumentCode,
						DocumentType = (DocumentType) tableTimeTrackDocumentType.DocumentType,
						IsSystem = tableTimeTrackDocumentType.IsSystem
					})
					.ToList();

				return new OperationResult<List<TimeTrackDocumentType>>(timeTrackDocumentTypes);
			}
			catch (Exception e)
			{
				return OperationResult<List<TimeTrackDocumentType>>.FromError(e.Message);
			}
		}

		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			try
			{
				var tableItem = new DataAccess.TimeTrackDocumentType
				{
					UID = timeTrackDocumentType.UID,
					OrganisationUID = timeTrackDocumentType.OrganisationUID,
					Name = timeTrackDocumentType.Name,
					ShortName = timeTrackDocumentType.ShortName,
					DocumentCode = timeTrackDocumentType.Code,
					DocumentType = (int) timeTrackDocumentType.DocumentType
				};
				Context.TimeTrackDocumentTypes.InsertOnSubmit(tableItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult AddSystemDocumentTypesForOrganisation(Guid organisationGuid)
		{
			try
			{
				var systemDocuments = GetAllSystemDocuments().Select(x => new DataAccess.TimeTrackDocumentType
				{
					UID = Guid.NewGuid(),
					DocumentCode = x.Code,
					DocumentType = (int) x.DocumentType,
					Name = x.Name,
					ShortName = x.ShortName,
					OrganisationUID = organisationGuid,
					IsSystem = true
				});
				Context.TimeTrackDocumentTypes.InsertAllOnSubmit(systemDocuments);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			try
			{
				var tableItem = Context.TimeTrackDocumentTypes.FirstOrDefault(x => x.UID.Equals(timeTrackDocumentType.UID));
				if (tableItem != null)
				{
					tableItem.Name = timeTrackDocumentType.Name;
					tableItem.ShortName = timeTrackDocumentType.ShortName;
					tableItem.DocumentCode = timeTrackDocumentType.Code;
					tableItem.DocumentType = (int)timeTrackDocumentType.DocumentType;
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult RemoveTimeTrackDocumentType(Guid uid)
		{
			try
			{
				var tableItem = Context.TimeTrackDocumentTypes.Single(x => x.UID.Equals(uid));
				Context.TimeTrackDocumentTypes.DeleteOnSubmit(tableItem);
				Context.TimeTrackDocumentTypes.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public TimeTrackDocumentType Get(int code)
		{
			var tableItem = Context.TimeTrackDocumentTypes.FirstOrDefault(x => x.DocumentCode == code);
			return tableItem == null ? null : Translate(tableItem);
		}

		private TimeTrackDocumentType Translate(DataAccess.TimeTrackDocumentType tableItem)
		{
			return new TimeTrackDocumentType
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				ShortName = tableItem.ShortName,
				Code = tableItem.DocumentCode,
				DocumentType = (DocumentType)tableItem.DocumentType,
				OrganisationUID = tableItem.OrganisationUID,
				IsSystem = tableItem.IsSystem
			};
		}
	}
}