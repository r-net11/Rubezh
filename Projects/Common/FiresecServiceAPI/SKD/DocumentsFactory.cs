using System.Collections.Generic;
namespace FiresecAPI.SKD
{
	public class DocumentsFactory
	{
		public List<ITimeTrackDocument> SystemDocuments { get; private set; }

		public DocumentsFactory()
		{
			SystemDocuments = GetAllSystemDocuments();
		}

		private List<ITimeTrackDocument> GetAllSystemDocuments()
		{
			var resultCollection = new List<ITimeTrackDocument>
			{
				new PresentDocument("Продолжительность работы в дневное время", "Я", 1),
				new PresentDocument("Продолжительность работы в ночное время", "Н", 2),
				new OvertimeDocument("Продолжительность работы в выходные и нерабочие праздничные дни", "РВ", 3),
				new OvertimeDocument("Продолжительность сверхурочной работы", "С", 4),
				new PresentDocument("Продолжительность работы вахтовым методом", "ВМ", 5),
				new PresentDocument("Служебная командировка", "К", 6),
				new PresentDocument("Повышение квалификации с отрывом от работы", "ПК", 7),
				new PresentDocument("Повышение квалификации с отрывом от работы в другой местности", "ПМ", 8),
				new AbsenceReasonableDocument("Ежегодный основной оплачиваемый отпуск", "ОТ", 9),
				new AbsenceReasonableDocument( "Ежегодный дополнительный оплачиваемый отпуск", "ОД", 10),
				new AbsenceReasonableDocument("Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением",
				"У", 11),
				new PresentDocument("Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы",
					"УВ", 12),
				new AbsenceReasonableDocument("Дополнительный отпуск в связи с обучением без сохранения заработной платы", "УД", 13),
				new AbsenceReasonableDocument("Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)", "Р", 14),
				new AbsenceReasonableDocument("Отпуск по уходу за ребенком до достижения им возраста трех лет", "ОЖ", 15),
				new AbsenceReasonableDocument("Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя", "ДО", 16),
				new AbsenceReasonableDocument("Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации",
					"ОЗ", 17),
				new AbsenceReasonableDocument("Ежегодный дополнительный отпуск без сохранения заработной платы", "ДБ", 18),
				new AbsenceReasonableDocument("Временная нетрудоспособность (кроме случаев, предусмотренных кодом 'Т') с назначением пособия согласно законодательству",
					"Б", 19),
				new AbsenceReasonableDocument("Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством", "Т", 20),
				new PresentDocument("Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством",
					"ЛЧ", 21),
				new AbsenceReasonableDocument("Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе",
					"ПВ", 22),
				new AbsenceReasonableDocument("Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству", "Г", 23),
				new AbsenceDocument("Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)", "ПР", 24),
				new PresentDocument("Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством",
					"НС", 25),
				new AbsenceReasonableDocument("Выходные дни (еженедельный отпуск) и нерабочие праздничные дни", "В", 26),
				new AbsenceReasonableDocument("Дополнительные выходные дни (оплачиваемые)", "ОВ", 27), //Отсутствие по уважительной причине
				new AbsenceReasonableDocument("Дополнительные выходные дни (без сохранения заработной платы)", "НВ", 28), //Отсутствие по уважительной причине
				new AbsenceReasonableDocument("Забастовка (при условиях и в порядке, предусмотренных законом)", "ЗБ", 29), //Отсутствие по уважительной причине
				new AbsenceDocument("Неявки по невыясненным причинам (до выяснения обстоятельств)", "НН", 30), //Отсутствие по не уважительной причине
				new PresentDocument("Время простоя по вине работодателя", "РП", 31),
				new PresentDocument("Время простоя по причинам, не зависящим от работодателя и работника", "НП", 32),
				new AbsenceDocument("Время простоя по вине работника", "ВП", 33), //Отсутствие по не уважительной причине
				new AbsenceReasonableDocument("Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством", "НО", 34), //Отсутствие по уважительной причине
				new AbsenceDocument("Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы",
					"НБ", 35), //Отсутствие по не уважительной причине
				new AbsenceReasonableDocument("Время приостановки работы в случае задержки выплаты заработной платы", "НЗ", 36) //Отсутствие по уважительной причине
			};

			return resultCollection;
		}

		//public ITimeTrackDocument GetDocument(DocumentType type, string name, string shortName, int code)
		//{
		//	ITimeTrackDocument document = null;

		//	switch (type)
		//	{
		//		case DocumentType.Absence: document = new AbsenceDocument(name, shortName, code);
		//			break;
		//		case DocumentType.Presence: document = new PresentDocument(name, shortName, code);
		//			break;
		//		case DocumentType.Overtime: document = new OvertimeDocument(name, shortName, code);
		//			break;
		//	}

		//	return document;
		//}

		public ITimeTrackDocument GetDocument(DocumentType type)
		{
			ITimeTrackDocument document = null;

			switch (type)
			{
				case DocumentType.Absence: document = new AbsenceDocument();
					break;
				case DocumentType.AbsenceReasonable: document = new AbsenceReasonableDocument();
					break;
				case DocumentType.Presence: document = new PresentDocument();
					break;
				case DocumentType.Overtime: document = new OvertimeDocument();
					break;
			}

			return document;
		}

		public ITimeTrackDocument GetDocument(TimeTrackDocumentType documentType)
		{
			ITimeTrackDocument document = null;

			switch (documentType.DocumentType)
			{
				case DocumentType.Absence: document = new AbsenceDocument(documentType.Name, documentType.ShortName, documentType.Code);
					break;
				case DocumentType.AbsenceReasonable: document = new AbsenceReasonableDocument(documentType.Name, documentType.ShortName, documentType.Code);
					break;
				case DocumentType.Presence: document = new PresentDocument(documentType.Name, documentType.ShortName, documentType.Code);
					break;
				case DocumentType.Overtime: document = new OvertimeDocument(documentType.Name, documentType.ShortName, documentType.Code);
					break;
			}

			return document;
		}

	}
}
