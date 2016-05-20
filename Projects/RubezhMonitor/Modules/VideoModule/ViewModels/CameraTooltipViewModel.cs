using Infrastructure.Plans.Presenter;
using RubezhAPI.Models;

namespace VideoModule.ViewModels
{
	public class CameraTooltipViewModel : StateTooltipViewModel<Camera>
	{

		public Camera Camera
		{
			get { return Item; }
		}

		public string OnGuardString { get { return Item.IsOnGuard ? "На охране" : string.Empty; } }
		public string OnRecordString { get { return Item.IsRecordOnline ? "Идет запись" : string.Empty; } }

		public CameraTooltipViewModel(Camera camera)
			: base(camera)
		{

		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();

		}
	}
}