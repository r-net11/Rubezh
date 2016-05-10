using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class TimeTrackDocument
	{
		public TimeTrackDocument()
		{
			UID = Guid.NewGuid();
			DocumentCode = 0;
			StartDateTime = DateTime.Now.Date;
			EndDateTime = DateTime.Now.Date + new TimeSpan(23, 59, 59);
			DocumentDateTime = DateTime.Now;
		}

		public TimeTrackDocument(string name, string shortCode, int numCode, DocumentType type)
		{
			TimeTrackDocumentType = new TimeTrackDocumentType(name, shortCode, numCode, type);
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public bool IsOutside { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public DateTime StartDateTime { get; set; }

		[DataMember]
		public DateTime EndDateTime { get; set; }

		[DataMember]
		public int DocumentCode { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public DateTime DocumentDateTime { get; set; }

		[DataMember]
		public int DocumentNumber { get; set; }

		[DataMember]
		public TimeTrackDocumentType TimeTrackDocumentType { get; set; }

		/// <summary>
		/// Имя файла после помещения в хранилище
		/// </summary>
		[DataMember]
		public string FileName { get; set; }

		/// <summary>
		/// Оригинальное имя файла, до помещения в хранилище
		/// </summary>
		[DataMember]
		public string OriginalFileName { get; set; }

		public string JournalEventName
		{
			get { return string.Format(Resources.Language.SKD.TimeTrack.TimeTrackDocument.JournalEventName, TimeTrackDocumentType == null ? null : TimeTrackDocumentType.ShortName, TimeTrackDocumentType == null ? null : TimeTrackDocumentType.Code.ToString(), DocumentDateTime.ToString("d")); }
		}
	}
}