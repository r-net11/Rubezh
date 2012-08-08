using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
    public class CameraViewModel : BaseViewModel
    {
        public Camera Camera { get; set; }

        public CameraViewModel(Camera camera)
        {
            Camera = camera;
        }

        public string PresenrationZones
        {
            get
            {
                var presenrationZones = new StringBuilder();
                if (Camera.Zones == null)
					Camera.Zones = new List<int>();
                for (int i = 0; i < Camera.Zones.Count; i++)
                {
                    if (i > 0)
                        presenrationZones.Append(", ");
                    var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == Camera.Zones[i]);
                    if (zone != null)
                        presenrationZones.Append(zone.PresentationName);
                }

                return presenrationZones.ToString();
            }
        }

        public void Update()
        {
            OnPropertyChanged("Camera");
            OnPropertyChanged("PresenrationZones");
        }
    }
}