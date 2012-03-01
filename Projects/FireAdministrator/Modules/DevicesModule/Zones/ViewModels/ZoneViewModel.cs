using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public Zone Zone { get; set; }

        public ZoneViewModel(Zone zone)
        {
            Zone = zone;
        }

        public string Name
        {
            get { return Zone.Name; }
        }

        public ulong? No
        {
            get { return Zone.No; }
        }

        public string Description
        {
            get { return Zone.Description; }
        }

        public string DetectorCount
        {
            get
            {
                if (Zone.ZoneType == ZoneType.Fire)
                    return Zone.DetectorCount.ToString();
                return null;
            }
        }

        public string EvacuationTime
        {
            get { return Zone.EvacuationTime; }
        }

        public string PresentationName
        {
            get { return Zone.PresentationName; }
        }

        public void Update()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("No");
            OnPropertyChanged("Description");
            OnPropertyChanged("DetectorCount");
            OnPropertyChanged("EvacuationTime");
            OnPropertyChanged("PresentationName");
        }
    }
}