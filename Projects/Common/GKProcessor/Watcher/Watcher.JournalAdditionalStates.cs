using System.Collections.Generic;
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
				var deviceState = descriptor.Device.InternalState;
				if (journalItem.Name == "Неисправность")
				{
					if (!string.IsNullOrEmpty(journalItem.Description))
					{
						AddAdditionalState(deviceState, journalItem.Description, XStateClass.Failure);
						if (descriptor.Device.DriverType == XDriverType.Battery)
						{
							var batteryNamesGroup = BatteryJournalHelper.BatteryNamesGroups.FirstOrDefault(x => x.Names.Contains(journalItem.Description));
							if (batteryNamesGroup != null)
							{
								foreach (var name in batteryNamesGroup.Names)
								{
									if (name != journalItem.Description)
									{
										deviceState.AdditionalStates.RemoveAll(x => x.Name == name);
									}
								}
							}
						}
					}
				}
				if (journalItem.Name == "Неисправность устранена")
				{
					if (string.IsNullOrEmpty(journalItem.Description))
					{
						deviceState.AdditionalStates.RemoveAll(x => x.StateClass == XStateClass.Failure);
					}
					else
					{
						deviceState.AdditionalStates.RemoveAll(x => x.Name == journalItem.Description);
						if (descriptor.Device.DriverType == XDriverType.Battery)
						{
							var batteryNamesGroup = BatteryJournalHelper.BatteryNamesGroups.FirstOrDefault(x => x.ResetName == journalItem.Description);
							if (batteryNamesGroup != null)
							{
								foreach (var name in batteryNamesGroup.Names)
								{
									deviceState.AdditionalStates.RemoveAll(x => x.Name == name);
								}
							}
						}
					}
				}
				if (journalItem.Name == "Информация")
				{
					switch (journalItem.Description)
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

		void AddAdditionalState(XBaseInternalState baseState, string description, XStateClass stateClass)
		{
			if (!baseState.AdditionalStates.Any(x => x.Name == description))
			{
				var additionalState = new XAdditionalState()
				{
					StateClass = stateClass,
					Name = description
				};
				baseState.AdditionalStates.Add(additionalState);
			}
		}
	}

	public static class BatteryJournalHelper
	{
		static BatteryJournalHelper()
		{
			BatteryNamesGroups = new List<BatteryNamesGroup>();

			var batteryNamesGroup1 = new BatteryNamesGroup("Выход 1");
			batteryNamesGroup1.Names.Add("КЗ Выхода 1");
			batteryNamesGroup1.Names.Add("Перегрузка Выхода 1");
			batteryNamesGroup1.Names.Add("Напряжение Выхода 1 выше нормы");
			BatteryNamesGroups.Add(batteryNamesGroup1);

			var batteryNamesGroup2 = new BatteryNamesGroup("Выход 2");
			batteryNamesGroup2.Names.Add("КЗ Выхода 2");
			batteryNamesGroup2.Names.Add("Перегрузка Выхода 2");
			batteryNamesGroup2.Names.Add("Напряжение Выхода 2 выше нормы");
			BatteryNamesGroups.Add(batteryNamesGroup2);

			var batteryNamesGroup3 = new BatteryNamesGroup("АКБ 1");
			batteryNamesGroup3.Names.Add("АКБ 1 Разряд");
			batteryNamesGroup3.Names.Add("АКБ 1 Глубокий Разряд");
			batteryNamesGroup3.Names.Add("АКБ 1 Отсутствие");
			BatteryNamesGroups.Add(batteryNamesGroup3);

			var batteryNamesGroup4 = new BatteryNamesGroup("АКБ 2");
			batteryNamesGroup4.Names.Add("АКБ 2 Разряд");
			batteryNamesGroup4.Names.Add("АКБ 2 Глубокий Разряд");
			batteryNamesGroup4.Names.Add("АКБ 2 Отсутствие");
			BatteryNamesGroups.Add(batteryNamesGroup4);
		}

		public static List<BatteryNamesGroup> BatteryNamesGroups { get; private set; }

		public class BatteryNamesGroup
		{
			public BatteryNamesGroup(string resetName)
			{
				ResetName = resetName;
				Names = new List<string>();
			}

			public string ResetName { get; set; }
			public List<string> Names { get; set; }
		}
	}
}