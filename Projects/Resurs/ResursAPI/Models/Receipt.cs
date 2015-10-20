
using System.IO;
namespace ResursAPI
{
	public class Receipt : ModelBase
	{
		public byte[] Template { get; set; }
		public Receipt()
		{
			Name = "Новый шаблон";
		}
	}
}