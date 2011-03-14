using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using WpfApplication13.Common;
using System.Windows;

namespace WpfApplication13
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Name = "Hello";
            Persons = new ObservableCollection<Person>();
            Persons.Add(new Person() { Name = "Mike", Id = "1", Enabled=true, Description = "sadfsdfsdfsdf" });
            Persons.Add(new Person() { Name = "John", Id = "2", Enabled = false, Description = "11111111111111111" });
            Persons.Add(new Person() { Name = "Arnold", Id = "3", Enabled = true, Description = "00000000000000000000000" });
            SelectCommand = new RelayCommand(OnSelect, can);
        }

        public RelayCommand SelectCommand { get; private set; }

        void OnSelect(object o)
        {
            MessageBox.Show(SelectedPerson.Name);
            SelectedPerson = Persons[1];
            flag = false;
        }

        bool flag = true;
        bool can(object o)
        {
            return flag;
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        ObservableCollection<Person> persons;
        public ObservableCollection<Person> Persons
        {
            get { return persons; }
            set
            {
                persons = value;
                OnPropertyChanged("Persons");
            }
        }

        Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged("SelectedPerson");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
    }
}
