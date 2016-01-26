using RubezhAPI;
using RubezhAPI.GK;
using System.Linq;

namespace GKWebService.Models
{
	public class DelayModel
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public string PresentationLogic { get; set; }
		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public string StateIcon { get; set; }
		GKState State { get; set; }
		GKDelay Delay { get; set; }
		public DelayModel(GKDelay delay)
		{
			Number = delay.No;
			Name = delay.Name;
			PresentationLogic = GKManager.GetPresentationLogic(delay.Logic);
			OnDelay = delay.State.OnDelay;
			HoldDelay = delay.State.HoldDelay;
			StateIcon = delay.State.StateClass.ToString();
			Delay = GKManager.Delays.FirstOrDefault(x => x.UID == delay.UID);
			Delay.State.StateChanged += OnStateChanged;
		}
		void OnStateChanged()
		{

		}
	}
}