using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.Networks.Collections.ObjectModel
{
    public class CollectionChangedEventArgs: EventArgs
    {
        #region Fields And Properties

        private INetwrokController[] _Networks;
        private NotifyCollectionChangedAction _Action;

        public INetwrokController[] Networks
        {
            get { return _Networks; }
            set { _Networks = value; }
        }

        public NotifyCollectionChangedAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        #endregion
    }
}
