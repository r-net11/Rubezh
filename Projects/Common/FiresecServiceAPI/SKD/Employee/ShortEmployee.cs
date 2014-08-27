using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
    public class ShortEmployee : IWithName, IWithOrganisationUID, IWithUID
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public string PositionName { get; set; }

		[DataMember]
		public List<SKDCard> Cards { get; set; }

		[DataMember]
		public PersonType Type { get; set; }

		[DataMember]
		public List<TextColumn> TextColumns { get; set; }

		[DataMember]
		public string Appointed { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		public string Name
		{
            		get { return LastName + " " + FirstName + " " + SecondName; }
		        set { return; }
	        }

		public string FIO
		{
			get
			{
				return LastName + " " + FirstName + (SecondName != null ? " " + SecondName : "");
			}
		}
    }

	public class TextColumn
	{
		public Guid ColumnTypeUID { get; set; }
		public string Text { get; set; }
	}
}