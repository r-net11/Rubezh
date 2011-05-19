using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common
{
    public class DialogContent : BaseViewModel, IDialogContent
    {
        public bool Result { get; set; }
        public string Title { get; set; }

        public object InternalViewModel
        {
            get { return this; }
        }

        public Window Surface { get; set; }

        public void Close(bool result)
        {
            if (Surface != null)
            {
                Result = result;
                //Surface.DialogResult = result;
                Surface.Close();
            }
        }
    }
}
