using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.XModels;
using GKProcessor.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecClient;
using Infrastructure.Common.Services;
using System.Diagnostics;
using System.Threading;

namespace GKProcessor
{
	public partial class Watcher
	{
		void ParseAdditionalStates(JournalItem journalItem)
		{
			var binaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.GetNo() == journalItem.GKObjectNo);
			if (binaryObject!= null && binaryObject.Device != null)
			{
				if (journalItem.Name == "Неисправность")
				{
					var stateName = FailureToAdditionalState(journalItem.Description);
					if (stateName != null)
					{
						if (journalItem.YesNo != JournalYesNoType.No)
						{
							AddAdditionalState(binaryObject.Device, XStateClass.Failure, stateName);
						}
						else
						{
							RemoveAdditionalState(binaryObject.Device, stateName);
						}
					}
				}
			}
		}

		void AddAdditionalState(XDevice devce, XStateClass stateClass, string name)
		{
			if (!devce.DeviceState.AdditionalStates.Any(x => x.Name == name))
			{
				var additionalState = new XAdditionalState()
				{
					StateClass = stateClass,
					Name = name
				};
				devce.DeviceState.AdditionalStates.Add(additionalState);
			}
		}

		void RemoveAdditionalState(XDevice devce, string name)
		{
			devce.DeviceState.AdditionalStates.RemoveAll(x => x.Name == name);
		}

		string FailureToAdditionalState(string name)
		{
			switch (name)
			{
				case "Обр кнопка НОРМА    ": return "Обрыв в цепи включателя «НОРМА»";
				case "КЗ кнопка НОРМА     ": return "Замыкание в цепи включателя «НОРМА»";
				case "Обр кнопка ЗАЩИТА   ": return "Обрыв в цепи включателя «ЗАЩИТА";
				case "КЗ кнопка ЗАЩИТА    ": return "Замыкание в цепи включателя «ЗАЩИТА»";
				case "Обр конц ОТКРЫТО    ": return "Обрыв в цепи конечного выключателя «ОТКРЫТО»";
				case "Обр конц ЗАКРЫТО    ": return "Обрыв в цепи концевого выключателя «ЗАКРЫТО»";
				case "Обр цепи 1 ДВИГАТЕЛЯ": return "Обрыв цепи 1 двигателя";
				case "З/Р оба концевика   ": return "Запрещенное состояние концевых выключателей (Замкнуты/разомкнуты оба концевика)";
				case "Истекло время хода  ": return "Превышение времени хода";
			}
			return null;
		}

        void GetDeviceStateFromKAU(BinaryObjectBase binaryObjectBase)
        {
            return;
            var no = binaryObjectBase.BinaryBase.GetDatabaseNo(DatabaseType.Kau);
            if (no == 0)
                return;
            var sendResult = SendManager.Send(binaryObjectBase.BinaryBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(no));
            if (sendResult.HasError)
            {
                ConnectionChanged(false);
                return;
            }
            if (sendResult.Bytes.Count != 32)
            {
                IsAnyDBMissmatch = true;
                ApplicationService.Invoke(() => { binaryObjectBase.BinaryBase.GetXBaseState().IsGKMissmatch = true; });
                return;
            }
            ConnectionChanged(true);
            var binaryObjectStateHelper = new BinaryObjectStateHelper();
            binaryObjectStateHelper.ParseAdditionalParameters(sendResult.Bytes);
            ApplicationService.Invoke(() =>
            {
                var binaryState = binaryObjectBase.BinaryBase.GetXBaseState();
                binaryState.AdditionalStates = binaryObjectStateHelper.AdditionalStates;
                binaryState.LastDateTime = DateTime.Now;
            });
            Trace.WriteLine("binaryObjectStateHelper.AdditionalStates.Count = " + binaryObjectStateHelper.AdditionalStates.Count);
        }
	}
}