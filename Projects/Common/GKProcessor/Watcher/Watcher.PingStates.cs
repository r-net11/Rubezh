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
			var binaryObject = GkDatabase.BinaryObjects[pingObjectNo];
			bool result = GetState(binaryObject.BinaryBase);

			pingObjectNo++;
			if (pingObjectNo >= GkDatabase.BinaryObjects.Count)
			{
				pingObjectNo = 0;
				CheckTechnologicalRegime();
			}
		}
	}
}