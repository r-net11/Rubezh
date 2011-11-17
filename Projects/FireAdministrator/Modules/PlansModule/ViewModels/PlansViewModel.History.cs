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
using System.Diagnostics;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        List<HistoryItem> History;
        public int Offset;

        void InitializeHistory()
        {
            UndoCommand = new RelayCommand(OnUndo, CanUndo);
            RedoCommand = new RelayCommand(OnRedo, CanRedo);

            ServiceFactory.Events.GetEvent<ElementPositionChangedEvent>().Subscribe(x => { OnElementPositionChanged(x); });
            ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsAdded);
            ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsRemoved);
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);
        }

        void ResetHistory()
        {
            History = new List<HistoryItem>();
            Offset = -1;
        }

        void OnElementsAdded(List<ElementBase> elements)
        {
            var historyItem = new HistoryItem();

            foreach (var element in elements)
            {
                var operationItem = new OperationItem()
                {
                    ActionType = ActionType.Added,
                    ElementBase = element.Clone()
                };
                historyItem.OperationItems.Add(operationItem);
            }

            if (History.Count > Offset + 1)
                History.RemoveRange(Offset + 1, History.Count - Offset - 1);
            History.Add(historyItem);
            Offset = History.Count - 1;
        }

        void OnElementsRemoved(List<ElementBase> elements)
        {
            var historyItem = new HistoryItem();

            foreach (var element in elements)
            {
                var operationItem = new OperationItem()
                {
                    ActionType = ActionType.Removed,
                    ElementBase = element.Clone()
                };
                historyItem.OperationItems.Add(operationItem);
            }

            if (History.Count > Offset + 1)
                History.RemoveRange(Offset + 1, History.Count - Offset - 1);
            History.Add(historyItem);
            Offset = History.Count - 1;
        }

        void OnElementsChanged(List<ElementBase> elements)
        {
            var historyItem = new HistoryItem();

            foreach (var element in elements)
            {
                var operationItem = new OperationItem()
                {
                    ActionType = ActionType.Edited,
                    ElementBase = element.Clone()
                };
                historyItem.OperationItems.Add(operationItem);
            }

            if (History.Count > Offset + 1)
                History.RemoveRange(Offset + 1, History.Count - Offset - 1);
            History.Add(historyItem);
            Offset = History.Count - 1;
        }

        void OnElementPositionChanged(object obj)
        {
            return;

            if (obj == null)
                return;

            DesignerItem designerItem = obj as DesignerItem;
            ElementBase elementBase = designerItem.ElementBase;

            var operationItem = new OperationItem()
            {
                ActionType = ActionType.Edited,
                ElementBase = elementBase.Clone()
            };
            var historyItem = new HistoryItem();
            historyItem.OperationItems.Add(operationItem);

            elementBase.Left = Canvas.GetLeft(designerItem);
            elementBase.Top = Canvas.GetTop(designerItem);

            if (History.Count > Offset + 1)
                History.RemoveRange(Offset + 1, History.Count - Offset - 1);
            History.Add(historyItem);
            Offset = History.Count - 1;
        }

        void Reset(bool isUndo)
        {
            var historyItem = History[Offset];
            foreach (var operationItem in historyItem.OperationItems)
            {
                ElementBase elementBase = operationItem.ElementBase;

                if (operationItem.ActionType == ActionType.Edited)
                {
                    var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                    designerItem.ElementBase.Left = elementBase.Left;
                    designerItem.ElementBase.Top = elementBase.Top;
                    Canvas.SetLeft(designerItem, designerItem.ElementBase.Left);
                    Canvas.SetTop(designerItem, designerItem.ElementBase.Top);
                    designerItem.Width = designerItem.ElementBase.Width;
                    designerItem.Height = designerItem.ElementBase.Height;
                    designerItem.Redraw();
                    PolygonResizeChrome.ResetActivePolygons();
                }
                if (operationItem.ActionType == ActionType.Added)
                {
                    if (isUndo)
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        DesignerCanvas.Children.Remove(designerItem);
                    }
                    else
                    {
                        DesignerCanvas.AddElement(elementBase);
                    }
                }
                if (operationItem.ActionType == ActionType.Removed)
                {
                    if (isUndo)
                    {
                        DesignerCanvas.AddElement(elementBase);
                    }
                    else
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        DesignerCanvas.Children.Remove(designerItem);
                    }
                }
            }
        }

        public RelayCommand UndoCommand { get; private set; }
        void OnUndo()
        {
            // сохранить текущее состояние

            Reset(true);
            Offset--;
        }

        bool CanUndo()
        {
            return Offset >= 0;
        }

        public RelayCommand RedoCommand { get; private set; }
        void OnRedo()
        {
            Offset++;
            Reset(false);
        }

        bool CanRedo()
        {
            Trace.WriteLine(History.Count.ToString() + " - " + Offset.ToString());
            return History.Count > Offset + 1;
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
        public ElementBase ElementBase { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Added,
        Removed,
        Edited
    }
}
