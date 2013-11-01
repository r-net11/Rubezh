using System.Collections.Generic;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using GKProcessor.Events;
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
			bool result = GetState(descriptor.XBase);

			pingObjectNo++;
			if (pingObjectNo >= GkDatabase.Descriptors.Count)
			{
				pingObjectNo = 0;
				CheckTechnologicalRegime();
			}
		}
	}
}