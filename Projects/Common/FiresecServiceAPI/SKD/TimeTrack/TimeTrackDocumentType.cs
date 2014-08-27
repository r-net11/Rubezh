using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackDocumentType
	{
		public TimeTrackDocumentType(string name, string shortName, int code, DocumentType documentType = DocumentType.Presence)
		{
			Name = name;
			ShortName = shortName;
			Code = code;
			DocumentType = documentType;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public int Code { get; set; }

		[DataMember]
		public DocumentType DocumentType { get; set; }
	}

	public enum DocumentType
	{
		Overtime = 0,
		Presence = 1,
		Absence = 2,
	}

	public static class TimeTrackDocumentTypesCollection
	{
		public static List<TimeTrackDocumentType> TimeTrackDocumentTypes { get; private set; }

		static TimeTrackDocumentTypesCollection()
		{
			TimeTrackDocumentTypes = new List<TimeTrackDocumentType>();
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность работы в дневное время", "Я", 1));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность работы в ночное время", "Н", 2));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность работы в выходные и нерабочие праздничные дни", "РВ", 3, DocumentType.Overtime));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность сверхурочной работы", "С", 4, DocumentType.Overtime));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность работы вахтовым методом", "ВМ", 5));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Служебная командировка", "К", 6));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Повышение квалификации с отрывом от работы", "ПК", 7));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Повышение квалификации с отрывом от работы в другой местности", "ПМ", 8));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Ежегодный основной оплачиваемый отпуск", "ОТ", 9));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Ежегодный дополнительный оплачиваемый отпуск", "ОД", 10));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением", "У", 11));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы", "УВ", 12));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Дополнительный отпуск в связи с обучением без сохранения заработной платы", "УД", 13));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)", "Р", 14));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отпуск по уходу за ребенком до достижения им возраста трех лет", "ОЖ", 15));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя", "ДО", 16));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации", "ОЗ", 17));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Ежегодный дополнительный отпуск без сохранения заработной платы", "ДБ", 18));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Временная нетрудоспособность (кроме случаев, предусмотренных кодом 'Т') с назначением пособия согласно законодательству", "Б", 19));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством", "Т", 20));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством", "ЛЧ", 21));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе", "ПВ", 22));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству", "Г", 23));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)", "ПР", 24, DocumentType.Absence));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством", "НС", 25));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Выходные дни (еженедельный отпуск) и нерабочие праздничные дни", "В", 26));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Дополнительные выходные дни (оплачиваемые)", "ОВ", 27));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Дополнительные выходные дни (без сохранения заработной платы)", "НВ", 28));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Забастовка (при условиях и в порядке, предусмотренных законом)", "ЗБ", 29));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Неявки по невыясненным причинам (до выяснения обстоятельств)", "НН", 30, DocumentType.Absence));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Время простоя по вине работодателя", "РП", 31));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Время простоя по причинам, не зависящим от работодателя и работника", "НП", 32));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Время простоя по вине работника", "ВП", 33, DocumentType.Absence));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством", "НО", 34));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы", "НБ", 35));
			TimeTrackDocumentTypes.Add(new TimeTrackDocumentType("Время приостановки работы в случае задержки выплаты заработной платы", "НЗ", 36));
		}
	}
}