using FiresecAPI.Models;
using Infrastructure.Common;

namespace VideoModule.ViewModels
{
    public class CameraViewModel : BaseViewModel
    {
        public Camera Camera { get; set; }

        public CameraViewModel(Camera camera)
        {
            Camera = camera;
        }

        public void Update()
        {
            OnPropertyChanged("Camera");
        }
    }
}