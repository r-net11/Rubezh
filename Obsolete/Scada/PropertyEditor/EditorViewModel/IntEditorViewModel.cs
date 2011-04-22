using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PropertyEditor
{
    public class IntEditorViewModel : BaseEditorViewModel
    {
        public IntEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new IntEditorView())
        {
            propertyValue = editingObject.GetType().GetProperty(PropertyName).GetValue(editingObject, null).ToString();
        }

        string propertyValue;
        public string PropertyValue
        {
            get { return propertyValue; }
            set
            {
                int intValue = Convert.ToInt32(value);
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, intValue, null);
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
