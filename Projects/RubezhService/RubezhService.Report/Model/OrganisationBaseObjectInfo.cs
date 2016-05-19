using System;

namespace RubezhService.Report.Model
{
	public class OrganisationBaseObjectInfo<T> : DeletableObjectInfo<T>
	{
		public Guid OrganisationUID { get; set; }
		public string Organisation { get; set; }
	}
}
