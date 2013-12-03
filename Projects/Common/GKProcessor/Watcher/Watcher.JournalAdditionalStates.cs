using System.Linq;
using XFiresecAPI;

namespace GKProcessor
{
	public partial class Watcher
	{
		void ParseAdditionalStates(JournalItem journalItem)
		{
			var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalItem.GKObjectNo);

			if (descriptor != null && descriptor.Device != null)
			{
				var deviceState = descriptor.Device.DeviceState;
				if (journalItem.Name == "Неисправность")
				{
					if (!string.IsNullOrEmpty(journalItem.Description))
					{
						AddAdditionalState(deviceState, journalItem.Description);
						if (descriptor.Device.DriverType == XDriverType.Battery)
						{
						}
					}
				}
				if (journalItem.Name == "Неисправность устранена")
				{
					if (string.IsNullOrEmpty(journalItem.Description))
					{
						deviceState.AdditionalStates.Clear();
					}
					else
					{
						deviceState.AdditionalStates.RemoveAll(x => x.Name == journalItem.Description);
						if (descriptor.Device.DriverType == XDriverType.Battery)
						{
							switch(journalItem.Description)
							{
								case "Выход 1":
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "КЗ Выхода 1");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "Перегрузка Выхода 1");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "Напряжение Выхода 1 выше нормы");
									break;

								case "Выход 2":
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "КЗ Выхода 2");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "Перегрузка Выхода 2");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "Напряжение Выхода 2 выше нормы");
									break;

								case "АКБ 1":
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 1 Разряд");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 1 Глубокий Разряд");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 1 Отсутствие");
									break;

								case "АКБ 2":
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 2 Разряд");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 2 Глубокий Разряд");
									deviceState.AdditionalStates.RemoveAll(x => x.Name == "АКБ 2 Отсутствие");
									break;
							}
						}
					}
				}
				if (journalItem.Name == "Информация")
				{
					switch(journalItem.Description)
					{
						case "Низкий уровень":
						case "Высокий уровень":
						case "Аварийный уровень":
							AddAdditionalState(deviceState, journalItem.Description);
							break;
						case "Уровень норма":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Низкий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Высокий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							break;
					}
				}
				deviceState.OnInternalStateChanged();
			}
		}

		void AddAdditionalState(XDeviceState deviceState, string description)
		{
			if (!deviceState.AdditionalStates.Any(x => x.Name == description))
			{
				var additionalState = new XAdditionalState()
				{
					StateClass = XStateClass.Failure,
					Name = description
				};
				deviceState.AdditionalStates.Add(additionalState);
			}
		}
	}
}