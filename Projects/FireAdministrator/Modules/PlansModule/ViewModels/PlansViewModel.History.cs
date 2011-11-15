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

            DesignerItem designerItem = obj as DesignerItem;
            ElementBase elementBase = designerItem.ElementBase;

            var operationItem = new OperationItem()
            {
                ActionType = ActionType.Edited,
                Item = elementBase.Clone()
            };
            var historyItem = new HistoryItem();
            historyItem.OperationItems.Add(operationItem);
            History.Add(historyItem);

            elementBase.Left = Canvas.GetLeft(designerItem);
            elementBase.Top = Canvas.GetTop(designerItem);

            Offset++;
        }

        public int Offset;

        void Reset()
        {
            var historyItem = History[Offset];
            foreach (var operationItem in historyItem.OperationItems)
            {
                ElementBase elementBase = operationItem.Item;
                var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                designerItem.ElementBase.Left = elementBase.Left;
                designerItem.ElementBase.Top = elementBase.Top;
                Canvas.SetLeft(designerItem, designerItem.ElementBase.Left);
                Canvas.SetTop(designerItem, designerItem.ElementBase.Top);
                designerItem.Redraw();
            }
        }

        public RelayCommand UndoCommand { get; private set; }
        void OnUndo()
        {
            if (Offset == 0)
                return;
            Offset--;
            Reset();
        }

        public RelayCommand RedoCommand { get; private set; }
        void OnRedo()
        {
            if (History.Count < Offset + 1)
                return;
            Reset();
            Offset++;
        }
    }

    public class HistoryItem
    {
        public HistoryItem()
        {
            OperationItems = new List<OperationItem>();
        }

        public List<OperationItem> OperationItems { get; set; }
    }

    public class OperationItem
    {
        public ElementBase Item { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Added,
        Removed,
        Edited
    }
}
