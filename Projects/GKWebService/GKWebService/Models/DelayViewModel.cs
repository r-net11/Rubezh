using GKWebService.DataProviders;
using RubezhAPI;
using RubezhAPI.GK;
using System.Linq;

namespace GKWebService.Models
{
	public class DelayViewModel
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public string PresentationLogic { get; set; }
		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public string StateIcon { get; set; }
		public string Uid { get; set; }
		GKState State { get; set; }
		GKDelay Delay { get; set; }
		public DelayViewModel(GKDelay delay)
		{
			Number = delay.No;
			Name = delay.Name;
			PresentationLogic = GKManager.GetPresentationLogic(delay.Logic);
			OnDelay = delay.State.OnDelay;
			HoldDelay = delay.State.HoldDelay;
			StateIcon = delay.State.StateClass.ToString();
			Uid = delay.UID.ToString();
			Delay = GKManager.Delays.FirstOrDefault(x => x.UID == delay.UID);
			Delay.State.StateChanged += OnStateChanged;
		}
		void OnStateChanged()
		{
			StateIcon = Delay.State.StateClass.ToString();
			if (DelaysHub.Instance != null)
				DelaysHub.Instance.DelayStateIconUpdate(Uid, StateIcon);
		}
	}
}