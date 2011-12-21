using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelDetailsViewModel : SaveCancelDialogContent
    {
        public GuardLevel GuardLevel { get; private set; }

        public LevelDetailsViewModel(GuardLevel guardLevel = null)
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand<ZoneLevelViewModel>(OnRemove);
            ZoneLevels = new ObservableCollection<ZoneLevelViewModel>();

            if (guardLevel == null)
            {
                Title = "Создать уровень доступа";
                GuardLevel = new GuardLevel();
            }
            else
            {
                Title = "Редактировать уровень доступа";
                GuardLevel = guardLevel;
                Name = guardLevel.Name;

                if (guardLevel.ZoneLevels != null)
                {
                    foreach (var zoneLevel in guardLevel.ZoneLevels)
                    {
                        ZoneLevels.Add(new ZoneLevelViewModel(zoneLevel));
                    }
                }
            }

            if (ZoneLevels.Count == 0)
            {
                ZoneLevels.Add(new ZoneLevelViewModel(new ZoneLevel()));
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

        protected override bool CanSave()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override void Save(ref bool cancel)
        {
            GuardLevel.Name = Name;
            GuardLevel.ZoneLevels = new List<ZoneLevel>();
            foreach (var zoneLevelViewModel in ZoneLevels)
            {
                if (zoneLevelViewModel.ZoneLevel.ZoneNo.HasValue)
                    GuardLevel.ZoneLevels.Add(zoneLevelViewModel.ZoneLevel);
            }
        }
    }
}