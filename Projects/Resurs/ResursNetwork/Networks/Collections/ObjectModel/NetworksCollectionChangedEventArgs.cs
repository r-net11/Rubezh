using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.Networks.Collections.ObjectModel
{
    public class NetworksCollectionChangedEventArgs: EventArgs
    {
        #region Fields And Properties

        private INetwrokController _Network;
        private NotifyCollectionChangedAction _Action;

        public INetwrokController Network
        {
            get { return _Network; }
            set { _Network = value; }
        }

        public NotifyCollectionChangedAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
        #endregion
    }
}
