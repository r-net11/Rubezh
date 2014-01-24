using System.Collections.Generic;
using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Threading;
using System;

namespace GKProcessor
{
	public partial class Watcher
	{
		int pingObjectNo = 0;

		void PingNextState()
		{
            return;
			var descriptor = GkDatabase.Descriptors[pingObjectNo];
			GetState(descriptor.XBase);
			OnObjectStateChanged(descriptor.XBase);

			pingObjectNo++;
			if (pingObjectNo >= GkDatabase.Descriptors.Count)
			{
				pingObjectNo = 0;
				CheckTechnologicalRegime();
			}
		}
	}
}