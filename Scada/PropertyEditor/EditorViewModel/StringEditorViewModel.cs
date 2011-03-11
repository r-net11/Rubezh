using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Common;
using ControlBase;

namespace PropertyEditor
{
    public class StringEditorViewModel : BaseEditorViewModel
    {
        public StringEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new StringEditorView())
        {
            PropertyInfo propertyInfo = editingObject.GetType().GetProperty(PropertyName);
            propertyValue = propertyInfo.GetValue(editingObject, null) as string;
        }

        string propertyValue;
        public string PropertyValue
        {
            get { return propertyValue; }
            set
            {
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, value, null);
                propertyValue = EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null) as string;
                OnPropertyChanged("PropertyValue");
            }
        }

        public override void Update()
        {
            PropertyValue = EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null) as string;
        }
    }
}
