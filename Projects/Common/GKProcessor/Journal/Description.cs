using FiresecAPI.Models;

namespace GKProcessor
{
	public class Description
	{
		public string Name { get; set; }
		public DescriptionType DescriptionType { get; set; }

		public Description(string name, DescriptionType descriptionType)
		{
			Name = name;
			DescriptionType = descriptionType;
		}
	}
}