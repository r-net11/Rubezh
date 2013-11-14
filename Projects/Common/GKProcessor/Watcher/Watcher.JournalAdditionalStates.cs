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
						if (!deviceState.AdditionalStates.Any(x => x.Name == journalItem.Description))
						{
							var additionalState = new XAdditionalState()
							{
								StateClass = XStateClass.Failure,
								Name = journalItem.Description
							};
							deviceState.AdditionalStates.Add(additionalState);
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
					}
				}
				deviceState.OnStateChanged();
			}
		}
	}
}