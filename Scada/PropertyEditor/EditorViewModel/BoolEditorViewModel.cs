using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PropertyEditor
{
    class BoolEditorViewModel : BaseEditorViewModel
    {
        public BoolEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new BoolEditorView())
        {
            propertyValue = (bool)editingObject.GetType().GetProperty(PropertyName).GetValue(editingObject, null);
        }

        bool propertyValue;
        public bool PropertyValue
        {
            get { return propertyValue; }
            set
            {
                bool boolValue = value;
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, boolValue, null);
                propertyValue = (bool)EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null);
                OnPropertyChanged("PropertyValue");
            }
        }

        public override void Update()
        {
            PropertyValue = (bool)EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null);
        }
    }
}
