using System;
using StrazhAPI;
using StrazhAPI.SKD;
using System.Collections.Generic;
using System.Linq;

namespace StrazhDeviceSDK
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
                        return OperationResult<bool>.FromError(string.Format(Resources.Language.ProcessorMonitor.LoginFailure_Error, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

					var result = deviceProcessor.Wrapper.OpenDoor(device.IntAddress);
					if (result)
						return new OperationResult<bool>(true);
					else
                        return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.OperationOnController_Error);
				}
			}
            return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.LostController_Error);
		}

		public static OperationResult<bool> CloseDoor(SKDDevice device)
		{
			if (device.Parent != null)
			{
				var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.Parent.UID);
				if (deviceProcessor != null)
				{
					if (!deviceProcessor.IsConnected)
                        return OperationResult<bool>.FromError(string.Format(Resources.Language.ProcessorMonitor.LoginFailure_Error, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

					var result = deviceProcessor.Wrapper.CloseDoor(device.IntAddress);
					if (result)
					{
						return new OperationResult<bool>(true);
					}
					else
					{
						if (device.State != null && (device.State.AccessState == AccessState.OpenAlways))
						{
							return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.CloseDoor_OpenAlways_Error);
						}
						else
						{
                            return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.OperationOnController_Error);
						}
					}
				}
			}
            return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.LostController_Error);
		}

		/// <summary>
		/// Перезаписывает на контроллер все пропуска
		/// </summary>
		/// <param name="device">Контроллер, на котором необходимо перезаписать все пропуска</param>
		/// <param name="cards">Перезаписываемые пропуска</param>
		/// <param name="accessTemplates">Шаблоны доступа для перезаписываемых пропусков</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> SKDRewriteAllCards(SKDDevice device, IEnumerable<SKDCard> cards, IEnumerable<AccessTemplate> accessTemplates, bool doProgress = true)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.UID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return OperationResult<bool>.FromError(string.Format(Resources.Language.ProcessorMonitor.LoginFailure_Error, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

				var result = deviceProcessor.Wrapper.RemoveAllCards();
				if (!result)
                    return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.OperationOnController_Error);

				var cardWriter = new CardWriter();
				var error = cardWriter.RewriteAllCards(device, cards, accessTemplates, doProgress);
				if (error.Count > 0)
				{
					return OperationResult<bool>.FromError(error);
				}
				return new OperationResult<bool>(true);
			}
            return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.LostController_Error);
		}

		public static CardWriter AddCard(SKDCard skdCard, AccessTemplate accessTemplate)
		{
			var cardWriter = new CardWriter();
			cardWriter.AddCard(skdCard, accessTemplate);
			return cardWriter;
		}

		public static CardWriter ResetRepeatEnter(SKDCard card, List<Guid> doorGuids)
		{
			var cardWriter = new CardWriter();
			cardWriter.ResetRepeatEnter(card, doorGuids);
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

		public static OperationResult<bool> ClearPromptWarning(SKDDevice device)
		{
			if (device.Parent != null)
			{
				var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == device.Parent.UID);
				if (deviceProcessor != null)
				{
					if (!deviceProcessor.IsConnected)
                        return OperationResult<bool>.FromError(string.Format(Resources.Language.ProcessorMonitor.LoginFailure_Error, deviceProcessor.Device.Name, deviceProcessor.LoginFailureReason));

					var result = deviceProcessor.Wrapper.PromptWarning(device.IntAddress);
					if (result)
						return new OperationResult<bool>(true);
					else
                        return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.OperationOnController_Error);
				}
			}
            return OperationResult<bool>.FromError(Resources.Language.ProcessorMonitor.LostController_Error);
		}
	}
}