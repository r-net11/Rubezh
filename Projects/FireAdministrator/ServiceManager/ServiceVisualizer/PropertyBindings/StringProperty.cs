using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace ServiceVisualizer
{
    public class StringProperty : BaseViewModel
    {
        public string PropertyName { get; set; }

        string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }
    }
}
