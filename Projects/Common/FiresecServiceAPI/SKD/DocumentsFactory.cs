using System.Collections.Generic;
namespace FiresecAPI.SKD
{
	public class DocumentsFactory
	{
		public List<TimeTrackDocument> SystemDocuments { get; private set; }

		public DocumentsFactory()
		{
			SystemDocuments = GetAllSystemDocuments();
		}

		private List<TimeTrackDocument> GetAllSystemDocuments()
		{
			var resultCollection = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("Продолжительность работы в дневное время", "Я", 1, DocumentType.Presence),
				new TimeTrackDocument("Продолжительность работы в ночное время", "Н", 2, DocumentType.Presence),
				new TimeTrackDocument("Продолжительность работы в выходные и нерабочие праздничные дни", "РВ", 3, DocumentType.Overtime),
				new TimeTrackDocument("Продолжительность сверхурочной работы", "С", 4, DocumentType.Overtime),
				new TimeTrackDocument("Продолжительность работы вахтовым методом", "ВМ", 5, DocumentType.Presence),
				new TimeTrackDocument("Служебная командировка", "К", 6, DocumentType.Presence),
				new TimeTrackDocument("Повышение квалификации с отрывом от работы", "ПК", 7, DocumentType.Presence),
				new TimeTrackDocument("Повышение квалификации с отрывом от работы в другой местности", "ПМ", 8, DocumentType.Presence),
				new TimeTrackDocument("Ежегодный основной оплачиваемый отпуск", "ОТ", 9, DocumentType.AbsenceReasonable),
				new TimeTrackDocument( "Ежегодный дополнительный оплачиваемый отпуск", "ОД", 10, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением",
				"У", 11, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы",
					"УВ", 12, DocumentType.Presence),
				new TimeTrackDocument("Дополнительный отпуск в связи с обучением без сохранения заработной платы", "УД", 13, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)", "Р", 14, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Отпуск по уходу за ребенком до достижения им возраста трех лет", "ОЖ", 15, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя", "ДО", 16, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации",
					"ОЗ", 17, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Ежегодный дополнительный отпуск без сохранения заработной платы", "ДБ", 18, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Временная нетрудоспособность (кроме случаев, предусмотренных кодом 'Т') с назначением пособия согласно законодательству",
					"Б", 19, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством", "Т", 20, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством",
					"ЛЧ", 21, DocumentType.Presence),
				new TimeTrackDocument("Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе",
					"ПВ", 22, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству", "Г", 23, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)", "ПР", 24, DocumentType.Absence),
				new TimeTrackDocument("Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством",
					"НС", 25, DocumentType.Presence),
				new TimeTrackDocument("Выходные дни (еженедельный отпуск) и нерабочие праздничные дни", "В", 26, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Дополнительные выходные дни (оплачиваемые)", "ОВ", 27, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Дополнительные выходные дни (без сохранения заработной платы)", "НВ", 28, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Забастовка (при условиях и в порядке, предусмотренных законом)", "ЗБ", 29, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Неявки по невыясненным причинам (до выяснения обстоятельств)", "НН", 30, DocumentType.Absence),
				new TimeTrackDocument("Время простоя по вине работодателя", "РП", 31, DocumentType.Presence),
				new TimeTrackDocument("Время простоя по причинам, не зависящим от работодателя и работника", "НП", 32, DocumentType.Presence),
				new TimeTrackDocument("Время простоя по вине работника", "ВП", 33, DocumentType.Absence),
				new TimeTrackDocument("Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством", "НО", 34, DocumentType.AbsenceReasonable),
				new TimeTrackDocument("Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы",
					"НБ", 35, DocumentType.Absence),
				new TimeTrackDocument("Время приостановки работы в случае задержки выплаты заработной платы", "НЗ", 36, DocumentType.AbsenceReasonable)
			};

			return resultCollection;
		}
	}
}
