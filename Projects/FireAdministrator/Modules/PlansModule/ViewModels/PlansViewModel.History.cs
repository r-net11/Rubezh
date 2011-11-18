using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        List<HistoryItem> HistoryItems;
        int Offset;

        void InitializeHistory()
        {
            UndoCommand = new RelayCommand(OnUndo, CanUndo);
            RedoCommand = new RelayCommand(OnRedo, CanRedo);

            ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsAdded);
            ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsRemoved);
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);
        }

        void ResetHistory()
        {
            HistoryItems = new List<HistoryItem>();
            Offset = -1;
        }

        void AddHistoryItem(List<ElementBase> elements, ActionType actionType)
        {
            var historyItem = new HistoryItem()
            {
                ActionType = actionType
            };

            foreach (var elementBase in elements)
            {
                historyItem.Elements.Add(elementBase.Clone());
            }

            if (HistoryItems.Count > Offset + 1)
                HistoryItems.RemoveRange(Offset + 1, HistoryItems.Count - Offset - 1);
            HistoryItems.Add(historyItem);
            Offset = HistoryItems.Count - 1;
        }

        void OnElementsAdded(List<ElementBase> elements)
        {
            AddHistoryItem(elements, ActionType.Added);
        }

        void OnElementsRemoved(List<ElementBase> elements)
        {
            AddHistoryItem(elements, ActionType.Removed);
        }

        void OnElementsChanged(List<ElementBase> elements)
        {
            AddHistoryItem(elements, ActionType.Edited);

            foreach (var elementBase in elements)
            {
                ElementBasePolygon elementBasePolygon = elementBase as ElementBasePolygon;
                if (elementBasePolygon != null)
                {
                    Trace.WriteLine("");
                    foreach (var point in elementBasePolygon.PolygonPoints)
                    {
                        Trace.Write("(" + point.X.ToString() + ";" + point.Y.ToString() + ")");
                    }
                }
            }
        }

        void Reset(bool isUndo)
        {
            var historyItem = HistoryItems[Offset];
            foreach (var elementBase in historyItem.Elements)
            {
                if (historyItem.ActionType == ActionType.Edited)
                {
                    var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                    designerItem.ElementBase = elementBase.Clone();
                    designerItem.Redraw();
                }
                if (historyItem.ActionType == ActionType.Added)
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
                if (historyItem.ActionType == ActionType.Removed)
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
            if (HistoryItems.Count == Offset + 1)
            {
                var historyItem = HistoryItems[Offset];

                var beforeUndoHistoryItem = new HistoryItem()
                {
                    ActionType = historyItem.ActionType
                };

                foreach (var element in historyItem.Elements)
                {
                    var designerItem = DesignerCanvas.Items.FirstOrDefault(x=>x.ElementBase.UID == element.UID);
                    //var currentElement = DesignerCanvas.Elements.FirstOrDefault(x=>x.UID == element.UID);
                    var currentElement = designerItem.ElementBase;
                    currentElement.Left = DesignerCanvas.GetLeft(designerItem);
                    currentElement.Top = DesignerCanvas.GetTop(designerItem);
                    beforeUndoHistoryItem.Elements.Add(currentElement.Clone());
                }

                HistoryItems.Add(beforeUndoHistoryItem);
            }

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
            return HistoryItems.Count > Offset + 1;
        }
    }

    class HistoryItem
    {
        public HistoryItem()
        {
            Elements = new List<ElementBase>();
        }

        public ActionType ActionType { get; set; }
        public List<ElementBase> Elements { get; set; }
    }

    enum ActionType
    {
        Added,
        Removed,
        Edited
    }
}
