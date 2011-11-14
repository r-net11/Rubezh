using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Designer;
using Infrastructure;
using PlansModule.Events;
using System.Windows.Controls;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        List<HistoryItem> History;

        void InitializeHistory()
        {
            History = new List<HistoryItem>();
            ServiceFactory.Events.GetEvent<ElementPositionChangedEvent>().Subscribe(x => { OnElementPositionChanged(x); });
        }

        void OnElementPositionChanged(object obj)
        {
            if (obj == null)
                return;

            ElementBase elementBase = (obj as DesignerItem).ElementBase;
            var historyItem = new HistoryItem();
            var operationItem = new OperationItem();
            operationItem.ActionType = ActionType.Edited;
            operationItem.Item = elementBase.Clone();
            historyItem.Items = new List<OperationItem>();
            historyItem.Items.Add(operationItem);
            History.Add(historyItem);
        }

        public RelayCommand UndoCommand { get; private set; }
        void OnUndo()
        {
            var item = History[History.Count - 1].Items[0].Item;
            ElementBase elementBase = item as ElementBase;
            var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
            designerItem.ElementBase.Left = elementBase.Left;
            designerItem.ElementBase.Top = elementBase.Top;
            Canvas.SetLeft(designerItem, designerItem.ElementBase.Left);
            Canvas.SetTop(designerItem, designerItem.ElementBase.Top);
            designerItem.Redraw();
        }

        public RelayCommand RedoCommand { get; private set; }
        void OnRedo()
        {

        }
    }

    public class HistoryItem
    {
        public List<OperationItem> Items { get; set; }
    }

    public class OperationItem
    {
        public object Item { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Added,
        Removed,
        Edited
    }
}
