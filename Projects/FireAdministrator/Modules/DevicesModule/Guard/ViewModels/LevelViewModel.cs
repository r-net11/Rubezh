using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelViewModel : BaseViewModel
    {
        public LevelViewModel(GuardLevel guardLevel)
        {
            GuardLevel = guardLevel;
        }

        GuardLevel _guardLevel;
        public GuardLevel GuardLevel
        {
            get { return _guardLevel; }
            set
            {
                _guardLevel = value;
                OnPropertyChanged("GuardLevel");
                OnPropertyChanged("Zones");
            }
        }

        public string Zones
        {
            get
            {
                var stringBuilder = new StringBuilder();
                foreach (var zoneLevel in GuardLevel.ZoneLevels)
                {
                    stringBuilder.Append(zoneLevel.ZoneActionType.ToDescription() + " ");
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneLevel.ZoneNo);
                    stringBuilder.Append(zone.PresentationName);
                    stringBuilder.Append(", ");
                }
                return stringBuilder.ToString().TrimEnd(',', ' ');
            }
        }
    }
}