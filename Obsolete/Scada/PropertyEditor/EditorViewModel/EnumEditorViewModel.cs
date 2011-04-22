using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PropertyEditor
{
    public class EnumEditorViewModel : BaseEditorViewModel
    {
        public EnumEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new EnumEditorView())
        {
            EnumEntries = new ObservableCollection<EnumEntry>();

            PropertyInfo propertyInfo = editingObject.GetType().GetProperty(PropertyName);
            foreach (FieldInfo fInfo in propertyInfo.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                EnumEntries.Add(new EnumEntry(fInfo.Name));
            }

            EnumEntry enumEntry = new EnumEntry(propertyInfo.GetValue(editingObject, null).ToString());
            SelectedEnumEntry = enumEntry;
            //SelectedEnumEntry = new EnumEntry(propertyInfo.GetValue(data, null).ToString());
            SelectedEnumEntry.Name = propertyInfo.GetValue(editingObject, null).ToString();
        }

        ObservableCollection<EnumEntry> enumEntries;
        public ObservableCollection<EnumEntry> EnumEntries
        {
            get { return enumEntries; }
            set
            {
                enumEntries = value;
                OnPropertyChanged("EnumEntries");
            }
        }

        EnumEntry selectedEnumEntry;
        public EnumEntry SelectedEnumEntry
        {
            get { return selectedEnumEntry; }
            set
            {
                selectedEnumEntry = value;
                object e = Enum.Parse(EditingObject.GetType().GetProperty(PropertyName).PropertyType, value.Name);
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, e, null);
                OnPropertyChanged("SelectedEnumEntry");
            }
        }
    }

    public class EnumEntry : INotifyPropertyChanged
    {
        public EnumEntry(string name)
        {
            Name = name;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
