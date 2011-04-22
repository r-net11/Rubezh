using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace PropertyEditor
{
    class ColorEditorViewModel : BaseEditorViewModel
    {
        public ColorEditorViewModel(object editingObject, string propertyName)
            : base(editingObject, propertyName, new ColorEditorView())
        {
            StringColors = new ObservableCollection<string>(GetAllKnownColors());
            propertyValue = (Color)editingObject.GetType().GetProperty(PropertyName).GetValue(editingObject, null);
            StringSelectedColor = GetKnownColorName(propertyValue);
        }

        Color propertyValue;
        public Color PropertyValue
        {
            get { return propertyValue; }
            set
            {
                Color colorValue = value;
                EditingObject.GetType().GetProperty(PropertyName).SetValue(EditingObject, colorValue, null);
                propertyValue = (Color)EditingObject.GetType().GetProperty(PropertyName).GetValue(EditingObject, null);
                OnPropertyChanged("PropertyValue");
            }
        }

        string stringSelectedColor;
        public string StringSelectedColor
        {
            get { return stringSelectedColor; }
            set
            {
                stringSelectedColor = value;
                try
                {
                    PropertyValue = (Color)ColorConverter.ConvertFromString(stringSelectedColor);
                }
                catch
                {
                    ;
                }
                OnPropertyChanged("StringSelectedColor");
            }
        }

        ObservableCollection<string> stringColors;
        public ObservableCollection<string> StringColors
        {
            get { return stringColors; }
            set
            {
                stringColors = value;
                OnPropertyChanged("StringColors");
            }
        }

        #region Helpers
        public static List<string> GetAllKnownColors()
        {
            List<string> colorList = new List<string>();
            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propInfos)
            {
                colorList.Add(propertyInfo.Name);
            }
            return colorList;
        }

        public static string GetKnownColorName(Color clr)
        {
            Color clrKnownColor;

            Type ColorType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] arrPiColors = ColorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo pi in arrPiColors)
            {
                clrKnownColor = (Color)pi.GetValue(null, null);
                if (clrKnownColor == clr) return pi.Name;
            }

            return string.Empty;
        }
        #endregion
    }
}
