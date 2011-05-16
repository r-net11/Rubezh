using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    public abstract class RegionViewModel :
            BaseViewModel,
            IViewPart
    {
        #region Properties

        private string _title;
        private bool _visualizeErrors;

        public string Title
        {
            get { return _title; }
            set { SetValue(ref _title, value, "Title"); }
        }

        public bool VisualizeErrors
        {
            get { return _visualizeErrors; }
            set
            {
                _visualizeErrors = value;
                OnPropertyChanged("VisualizeErrors");
            }
        }

        #endregion

        protected RegionViewModel()
        {
            VisualizeErrors = false;
        }

        public abstract void Dispose();
    }
}
