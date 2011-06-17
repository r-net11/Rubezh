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
            Title = "Создать направление";

            Direction = new Direction();
            Direction.Name = "Новое направление";
            Direction.Zones = new List<int>();

            int maxId = FiresecManager.Configuration.Directions.Max(x => x.Id);
            Id = maxId + 1;
        }

        public void Initialize(Direction direction)
        {
            _isNew = false;
            Title = "Редактировать направление";

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
            if (_isNew)
            {
                if (FiresecManager.Configuration.Directions.Any(x => x.Id == Id))
                {
                    Close(false);
                    return;
                }
                Save();
                FiresecManager.Configuration.Directions.Add(Direction);
            }
            else
            {
                if ((Id != Direction.Id) && (FiresecManager.Configuration.Directions.Any(x => x.Id == Id)))
                {
                    Close(false);
                    return;
                }
                Save();
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
