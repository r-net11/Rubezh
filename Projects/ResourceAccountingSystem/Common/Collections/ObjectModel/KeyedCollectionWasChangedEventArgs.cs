using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Collections.ObjectModel
{
    public class KeyedCollectionWasChangedEventArgs<T>: EventArgs
    {
        #region Fields And Properties

        private Action _Action;

        public Action Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        private T _Item;

        public T Item
        {
            get { return _Item; }
            set { _Item = value; }
        } 
   
        #endregion
        
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public KeyedCollectionWasChangedEventArgs()
        {
            _Action = Action.Notdefined;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">Действие изменившее коллекцию</param>
        /// <param name="device">Устройство вызвавшее изменение коллекции</param>
        public KeyedCollectionWasChangedEventArgs(
            Action action, T item)
        {
            _Action = action;
            _Item = item;
        }
        #endregion
    }
}
