using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel
	{
		private List<HistoryItem> HistoryItems;
		private int Offset;
		private bool _historyAction = false;

		void InitializeHistory()
		{
			UndoCommand = new RelayCommand(OnUndo, CanUndo);
			RedoCommand = new RelayCommand(OnRedo, CanRedo);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementsAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementsRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementsChanged);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsAdded);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);

			ResetHistory();
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
				historyItem.ElementsBefore.Add(elementBase.Clone());

			foreach (var elementBase in elementsAfter)
				historyItem.ElementsAfter.Add(elementBase.Clone());

			if (HistoryItems.Count > Offset)
				HistoryItems.RemoveRange(Offset, HistoryItems.Count - Offset);
			HistoryItems.Add(historyItem);
			Offset = HistoryItems.Count;
		}

		void OnElementsAdded(List<ElementBase> elementsBefore)
		{
			if (!_historyAction)
				AddHistoryItem(new List<ElementBase>(), elementsBefore, ActionType.Added);
		}
		void OnElementsRemoved(List<ElementBase> elementsBefore)
		{
			if (!_historyAction)
			{
				PlanDesignerViewModel.Save();
				AddHistoryItem(elementsBefore, new List<ElementBase>(), ActionType.Removed);
			}
		}
		void OnElementsChanged(List<ElementBase> elementsBefore)
		{
		}
		public List<ElementBase> AddHistoryItem(List<ElementBase> elementsBefore)
		{
			var elementsAfter = DesignerCanvas.CloneElements(DesignerCanvas.SelectedItems);
			if (!_historyAction)
				AddHistoryItem(elementsBefore, elementsAfter, ActionType.Edited);
			return elementsAfter;
		}

		private void DoUndo()
		{
			var historyItem = HistoryItems[Offset];
			_historyAction = true;
			switch (historyItem.ActionType)
			{
				case ActionType.Edited:
					foreach (var elementBase in historyItem.ElementsBefore)
						DesignerCanvas.UpdateElement(elementBase);
					DesignerCanvas.UpdateZoom();
					ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(historyItem.ElementsBefore);
					break;
				case ActionType.Added:
					foreach (var elementBase in historyItem.ElementsAfter)
						DesignerCanvas.RemoveElement(elementBase);
					ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(historyItem.ElementsAfter);
					break;

				case ActionType.Removed:
					foreach (var elementBase in historyItem.ElementsBefore)
						DesignerCanvas.AddElement(elementBase);
					ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(historyItem.ElementsBefore);
					break;
			}
			DesignerCanvas.Refresh();
			_historyAction = false;
			DesignerCanvas.Toolbox.SetDefault();
		}
		private void DoRedo()
		{
			var historyItem = HistoryItems[Offset];
			_historyAction = true;
			switch (historyItem.ActionType)
			{
				case ActionType.Edited:
					foreach (var elementBase in historyItem.ElementsAfter)
						DesignerCanvas.UpdateElement(elementBase);
					DesignerCanvas.UpdateZoom();
					ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(historyItem.ElementsAfter);
					break;
				case ActionType.Added:
					foreach (var elementBase in historyItem.ElementsAfter)
						DesignerCanvas.AddElement(elementBase);
					ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(historyItem.ElementsAfter);
					break;
				case ActionType.Removed:
					foreach (var elementBase in historyItem.ElementsBefore)
						DesignerCanvas.RemoveElement(elementBase);
					ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(historyItem.ElementsBefore);
					break;
			}
			_historyAction = false;
			DesignerCanvas.Toolbox.SetDefault();
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
