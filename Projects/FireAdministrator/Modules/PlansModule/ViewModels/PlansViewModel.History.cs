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
            Offset = 0;
        }

        void AddHistoryItem(List<ElementBase> elementsBefore, List<ElementBase> elementsAfter, ActionType actionType)
        {
            var historyItem = new HistoryItem()
            {
                ActionType = actionType
            };

            foreach (var elementBase in elementsBefore)
            {
                historyItem.ElementsBefore.Add(elementBase.Clone());
            }

            foreach (var elementBase in elementsAfter)
            {
                historyItem.ElementsAfter.Add(elementBase.Clone());
            }

            if (HistoryItems.Count > Offset)
                HistoryItems.RemoveRange(Offset, HistoryItems.Count - Offset);
            HistoryItems.Add(historyItem);
            Offset = HistoryItems.Count;
        }

        void OnElementsAdded(List<ElementBase> elementsBefore)
        {
            AddHistoryItem(new List<ElementBase>(), elementsBefore, ActionType.Added);
        }

        void OnElementsRemoved(List<ElementBase> elementsBefore)
        {
            AddHistoryItem(elementsBefore, new List<ElementBase>(), ActionType.Removed);
        }

        void OnElementsChanged(List<ElementBase> elementsBefore)
        {
            var elementsAfter = DesignerCanvas.CloneSelectedElements();
            AddHistoryItem(elementsBefore, elementsAfter, ActionType.Edited);
        }

        void DoUndo()
        {
            var historyItem = HistoryItems[Offset];

            switch (historyItem.ActionType)
            {
                case ActionType.Edited:
                    foreach (var elementBase in historyItem.ElementsBefore)
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        designerItem.ElementBase = elementBase.Clone();
                        designerItem.Redraw();
                    }
                    return;

                case ActionType.Added:
                    foreach (var elementBase in historyItem.ElementsAfter)
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        DesignerCanvas.Children.Remove(designerItem);
                    }
                    return;

                case ActionType.Removed:
                    foreach (var elementBase in historyItem.ElementsBefore)
                    {
                        DesignerCanvas.AddElement(elementBase);
                    }
                    return;
            }
        }

        void DoRedo()
        {
            var historyItem = HistoryItems[Offset];

            switch (historyItem.ActionType)
            {
                case ActionType.Edited:
                    foreach (var elementBase in historyItem.ElementsAfter)
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        designerItem.ElementBase = elementBase.Clone();
                        designerItem.Redraw();
                    }
                    return;

                case ActionType.Added:
                    foreach (var elementBase in historyItem.ElementsAfter)
                    {
                        DesignerCanvas.AddElement(elementBase);
                    }
                    return;

                case ActionType.Removed:
                    foreach (var elementBase in historyItem.ElementsBefore)
                    {
                        var designerItem = DesignerCanvas.Items.FirstOrDefault(x => x.ElementBase.UID == elementBase.UID);
                        DesignerCanvas.Children.Remove(designerItem);
                    }
                    return;
            }
        }

        public RelayCommand UndoCommand { get; private set; }
        void OnUndo()
        {
            Offset--;
            DoUndo();
        }

        bool CanUndo()
        {
            return Offset > 0;
        }

        public RelayCommand RedoCommand { get; private set; }
        void OnRedo()
        {
            DoRedo();
            Offset++;
        }

        bool CanRedo()
        {
            return HistoryItems.Count > Offset;
        }
    }

    class HistoryItem
    {
        public HistoryItem()
        {
            ElementsBefore = new List<ElementBase>();
            ElementsAfter = new List<ElementBase>();
        }

        public ActionType ActionType { get; set; }
        public List<ElementBase> ElementsBefore { get; set; }
        public List<ElementBase> ElementsAfter { get; set; }
    }

    enum ActionType
    {
        Added,
        Removed,
        Edited
    }
}
