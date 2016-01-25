
namespace GKProcessor
{
	public partial class Watcher
	{
		int pingObjectNo = 0;

		void PingNextState()
		{
			return;
			var descriptor = GkDatabase.Descriptors[pingObjectNo];
			GetState(descriptor.GKBase);
			OnObjectStateChanged(descriptor.GKBase);

			pingObjectNo++;
			if (pingObjectNo >= GkDatabase.Descriptors.Count)
			{
				pingObjectNo = 0;
				CheckTechnologicalRegime();
			}
		}
	}
}