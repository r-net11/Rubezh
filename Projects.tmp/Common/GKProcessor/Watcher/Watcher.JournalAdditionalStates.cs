using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		void ParseAdditionalStates(JournalParser journalParser)
		{
			var journalItem = journalParser.JournalItem;
			var description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
			var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalParser.GKObjectNo);

			if (descriptor != null && descriptor.GKBase is GKDevice)
			{
				var device = descriptor.GKBase as GKDevice;
				var deviceState = device.InternalState;
				if (journalItem.JournalEventNameType == JournalEventNameType.Неисправность)
				{
					if (!string.IsNullOrEmpty(description))
					{
						AddAdditionalState(deviceState, description, XStateClass.Failure);
					}
				}
				if (journalItem.JournalEventNameType == JournalEventNameType.Неисправность_устранена)
				{
					if (string.IsNullOrEmpty(description))
					{
						deviceState.AdditionalStates.RemoveAll(x => x.StateClass == XStateClass.Failure);
					}
					else
					{
						deviceState.AdditionalStates.RemoveAll(x => x.Name == description);

						if (description == "Обрыв АЛС 1-2")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 1");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 2");
						}
						if (description == "Обрыв АЛС 3-4")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 3");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 4");
						}
						if (description == "Обрыв АЛС 5-6")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 5");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 6");
						}
						if (description == "Обрыв АЛС 7-8")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 7");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 8");
						}
					}
				}
				if (journalItem.JournalEventNameType == JournalEventNameType.Информация)
				{
					switch (description)
					{
						case "Низкий уровень":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Высокий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							break;
						case "Высокий уровень":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Высокий уровень", XStateClass.Info);
							break;
						case "Аварийный уровень":
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Высокий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Аварийный уровень", XStateClass.Failure);
							break;
						case "Уровень норма":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Низкий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Высокий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							break;
					}
				}
			}
		}

		void AddAdditionalState(GKBaseInternalState baseState, string description, XStateClass stateClass)
		{
			if (!baseState.AdditionalStates.Any(x => x.Name == description))
			{
				var additionalState = new GKAdditionalState()
				{
					StateClass = stateClass,
					Name = description
				};
				baseState.AdditionalStates.Add(additionalState);
			}
		}
	}
}