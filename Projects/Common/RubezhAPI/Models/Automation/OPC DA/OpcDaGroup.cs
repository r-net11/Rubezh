using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OpcDaGroup: OpcDaElement
	{
		//public OpcDaGroup()
		//{
		//	IsTag = false;
		//}

		public override bool IsTag
		{
			get { return false; }
		}
	}
}