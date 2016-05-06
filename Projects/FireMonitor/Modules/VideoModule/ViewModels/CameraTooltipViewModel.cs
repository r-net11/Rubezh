using StrazhAPI.GK;
using StrazhAPI.Models;
using Infrastructure.Client.Plans.Presenter;

namespace VideoModule.ViewModels
{
	public class CameraTooltipViewModel : StateTooltipViewModel<Camera>
	{
		public XStateClass StateClass { get; private set; }
		public Camera Camera
		{
			get { return Item; }
		}

		public CameraTooltipViewModel(Camera camera)
			: base(camera)
		{
			StateClass = camera.CameraState.StateClass;
		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();
			OnPropertyChanged(() => StateClass);
		}
	}
}