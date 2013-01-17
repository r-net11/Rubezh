using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
    public class ZoneTestViewModel : SaveCancelDialogViewModel
    {
        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        public ZoneTestViewModel()
        {
            Zones = new ObservableCollection<ZoneViewModel>();
            foreach (var zone in from zone in FiresecManager.Zones orderby zone.No select zone)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
        }
    }
}