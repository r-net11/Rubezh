using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Firesec.CoreConfig;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class DirectionDetailsViewModel : DialogContent
    {
        public DirectionDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        bool _isNew;
        public Direction Direction { get; private set; }

        public void Initialize()
        {
            _isNew = true;

            Direction = new Direction();
            Direction.Zones = new List<int>();

            int maxId = FiresecManager.CurrentConfiguration.Directions.Max(x => x.Id);
            Id = maxId + 1;
        }

        public void Initialize(Direction direction)
        {
            _isNew = false;
            Direction = direction;

            Id = direction.Id;
            Name = direction.Name;
            Description = direction.Description;
        }

        int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
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

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        void Save()
        {
            Direction.Id = Id;
            Direction.Name = Name;
            Direction.Description = Description;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            if (_isNew)
            {
                FiresecManager.CurrentConfiguration.Directions.Add(Direction);
            }

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
