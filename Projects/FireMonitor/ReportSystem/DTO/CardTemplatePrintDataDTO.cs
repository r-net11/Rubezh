using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ReportSystem.DTO
{
	[DataContract]
	public class CardTemplatePrintDataDTO
	{
		[DataMember] public string LastName { get; set; }
		[DataMember] public string FirstName { get; set; }
		[DataMember] public string MiddleName { get; set; }
		[DataMember] public string OrganisationName { get; set; }
		[DataMember] public string DepartmentName { get; set; }
		[DataMember] public string PositionName { get; set; }
		[DataMember] public string ExcortName { get; set; }
		[DataMember] public string ScheduleName { get; set; }
		[DataMember] public string Description { get; set; }
		[DataMember] public string Phone { get; set; }
		[DataMember] public uint? CardNo { get; set; }
		[DataMember] public string TabelNo { get; set; }
		[DataMember] public byte[] Photo { get; set; }
		[DataMember] public byte[] OrganisationLogo { get; set; }
		[DataMember] public byte[] DepartmentLogo { get; set; }
		[DataMember] public byte[] PositionLogo { get; set; }
		[DataMember] public IEnumerable<AdditionalColumnDTO> AdditionalColumns { get; set; }

	}
}
