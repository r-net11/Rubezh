using System;

namespace FiresecAPI.SKD
{
	public interface ITimeTrackDocument
	{
		Guid UID { get; set; }

		Guid EmployeeUID { get; set; }

		bool IsOutside { get; set; }

		DateTime StartDateTime { get; set; }

		DateTime EndDateTime { get; set; }

		int DocumentCode { get; set; }

		string Comment { get; set; }

		DateTime DocumentDateTime { get; set; }

		int DocumentNumber { get; set; }

		TimeTrackDocumentType TimeTrackDocumentType { get; set; }

		string FileName { get; set; }
	}
}
