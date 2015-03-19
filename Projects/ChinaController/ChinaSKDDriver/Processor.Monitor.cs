﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ChinaSKDDriver
{
	public static partial class Processor
	{
		public static OperationResult<bool> OpenDoor(SKDDevice device)
		{
			if (device.Parent != null)
			{
				var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.Parent.UID);
				if (deviceProcessor != null)
				{
					if (!deviceProcessor.IsConnected)
						return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

					var result = deviceProcessor.Wrapper.OpenDoor(device.IntAddress);
					if (result)
						return new OperationResult<bool>() { Result = true };
					else
						return new OperationResult<bool>("Ошибка при выполнении операции в контроллере");
				}
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> CloseDoor(SKDDevice device)
		{
			if (device.Parent != null)
			{
				var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.Parent.UID);
				if (deviceProcessor != null)
				{
					if (!deviceProcessor.IsConnected)
						return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

					var result = deviceProcessor.Wrapper.CloseDoor(device.IntAddress);
					if (result)
					{
						return new OperationResult<bool>() { Result = true };
					}
					else
					{
						if (device.State != null && device.State.OpenAlwaysTimeIndex == 1)
						{
							return new OperationResult<bool>("Нельзя закрыть замок, находящийся в режиме ВСЕГДА ОТКРЫТО");
						}
						else
						{
							return new OperationResult<bool>("Ошибка при выполнении операции в контроллере");
						}
					}
				}
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> SKDRewriteAllCards(SKDDevice device, IEnumerable<SKDCard> cards, IEnumerable<AccessTemplate> accessTemplates)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером. " + deviceProcessor.LoginFailureReason);

				var result = deviceProcessor.Wrapper.RemoveAllCards();
				if (!result)
					return new OperationResult<bool>("Ошибка при удалении всех карт в контроллере");

				var cardWriter = new CardWriter();
				var error = cardWriter.RewriteAllCards(device, cards, accessTemplates);
				if (!string.IsNullOrEmpty(error))
				{
					return new OperationResult<bool>(error);
				}
				return new OperationResult<bool>() { Result = true };
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static CardWriter AddCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			var cardWriter = new CardWriter();
			cardWriter.AddCard(skdCard, accessTemplate);
			return cardWriter;
		}

		public static CardWriter EditCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate)
		{
			var cardWriter = new CardWriter();
			cardWriter.EditCard(oldCard, oldAccessTemplate, newCard, newAccessTemplate);
			return cardWriter;
		}

		public static CardWriter DeleteCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			var cardWriter = new CardWriter();
			cardWriter.DeleteCard(skdCard, accessTemplate);
			return cardWriter;
		}
	}
}