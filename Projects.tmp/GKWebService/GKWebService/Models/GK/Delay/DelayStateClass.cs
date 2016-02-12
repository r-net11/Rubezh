using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class DelayStateClass
	{
		public string IconData { get; set; }

		public string Name { get; set; }

		public DelayStateClass()
		{

		}

		public DelayStateClass(XStateClass stateClass)
		{
			Name = stateClass.ToDescription();
			IconData = stateClass.ToString();
		}
	}
}