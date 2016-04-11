using System;
using System.Linq;
using System.ServiceModel;
using GKImitator.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKImitator
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class ImitatorService : IImitatorService
	{
		public OperationResult<string> TestImitator()
		{
			return new OperationResult<string>("Hello from Imitator");
		}

		public OperationResult<bool> ConrtolGKBase(Guid uid, GKStateBit command)
		{
			var descriptor = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.UID == uid);
			if (descriptor == null)
				return OperationResult<bool>.FromError("Не найден элемент " + uid + " в конфигурации");
			switch (command)
			{
				case GKStateBit.Fire1:
					descriptor.SetFire1Command.Execute();
					break;
				case GKStateBit.Fire2:
					descriptor.SetFire2Command.Execute();
					break;
				case GKStateBit.Reset:
					descriptor.ResetFireCommand.Execute();
					break;
				default:
					return OperationResult<bool>.FromError("Такая команда ещё не реализована");
			}
			return new OperationResult<bool>(true);
		}
	}
}
