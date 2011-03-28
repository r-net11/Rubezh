using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace ServiceVisualizer
{
    public class TextBinding : BaseViewModel
    {
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
