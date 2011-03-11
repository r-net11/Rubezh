using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PropertyEditor
{
    class DoubleEditorViewModel : BaseEditorViewModel
    {
        public DoubleEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new DoubleEditorView())
        {
            propertyValue = editingObject.GetType().GetProperty(PropertyName).GetValue(editingObject, null).ToString();
        }

        string propertyValue;
        public string PropertyValue
        {
            get { return propertyValue; }
            set
            {
                double doubleValue = Convert.ToDouble(value);
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, doubleValue, null);
                propertyValue = EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null).ToString();
                OnPropertyChanged("PropertyValue");
            }
        }

        public override void Update()
        {
            PropertyValue = EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null).ToString();
        }
    }
}
