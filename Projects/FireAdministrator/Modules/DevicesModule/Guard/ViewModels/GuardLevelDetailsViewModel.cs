using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelDetailsViewModel : SaveCancelDialogContent
    {
        public GuardLevelDetailsViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ZoneLevelViewModel>(OnRemove);
            ZoneLevels = new ObservableCollection<ZoneLevelViewModel>();
        }

        public GuardLevel GuardLevel { get; private set; }
        bool _isNew;

        public void Initialize()
        {
            Title = "Создать уровень доступа";
            _isNew = true;
            GuardLevel = new GuardLevel();
            ZoneLevels = new ObservableCollection<ZoneLevelViewModel>();
        }

        public void Initialize(GuardLevel guardLevel)
        {
            Title = "Редактировать уровень доступа";
            _isNew = false;
            GuardLevel = guardLevel;
            Name = guardLevel.Name;
            ZoneLevels = new ObservableCollection<ZoneLevelViewModel>();

            if (guardLevel.ZoneLevels != null)
            {
                foreach (var zoneLevel in guardLevel.ZoneLevels)
                {
                    var ZoneLevelViewModel = new ZoneLevelViewModel(zoneLevel);
                    ZoneLevels.Add(ZoneLevelViewModel);
                }
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<ZoneLevelViewModel> ZoneLevels { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            ZoneLevels.Add(new ZoneLevelViewModel(new ZoneLevel()));
        }

        public RelayCommand<ZoneLevelViewModel> RemoveCommand { get; private set; }
        void OnRemove(ZoneLevelViewModel zoneLevelViewModel)
        {
            ZoneLevels.Remove(zoneLevelViewModel);
        }

        protected override void Save(ref bool cancel)
        {
            GuardLevel.Name = Name;
            GuardLevel.ZoneLevels = new List<ZoneLevel>();
            foreach (var zoneLevelViewModel in ZoneLevels)
            {
                GuardLevel.ZoneLevels.Add(zoneLevelViewModel.ZoneLevel);
            }
        }
    }
}
