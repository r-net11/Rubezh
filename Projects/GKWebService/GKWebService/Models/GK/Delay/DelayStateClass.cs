using Controls.Converters;
using GKWebService.Utils;
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

			var iconResourceName = new XStateClassToIconConverter().Convert(stateClass, null, null, null);
			if (iconResourceName != null)
			{
				IconData = "data:image/gif;base64," + InternalConverter.GetImageResource((string)iconResourceName).Item1;
			}
			else
			{
				IconData = string.Empty;
			}
		}
	}
}