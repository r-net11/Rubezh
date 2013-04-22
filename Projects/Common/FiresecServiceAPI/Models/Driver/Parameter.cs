using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Parameter
	{
		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Caption { get; set; }

		[DataMember]
		public bool Visible { get; set; }

		public bool IsIgnore
		{
			get
			{
				if (string.IsNullOrEmpty(Value) || Value == "<NULL>")
					return true;
				if (Name.StartsWith("Config$"))
					return true;
				return false;
			}
		}

		public Parameter Copy()
		{
			var parameter = new Parameter();
			parameter.Name = Name;
			parameter.Caption = Caption;
			parameter.Visible = Visible;
			return parameter;
		}
	}
}