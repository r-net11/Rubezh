using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKProcessor
{
	public class GKLifecycleManager : IDisposable
	{
		GKLifecycleInfo gkLifecycleInfo;

		public GKLifecycleManager(GKDevice device, string name)
		{
			gkLifecycleInfo = new GKLifecycleInfo()
			{
				Device = device,
				Name = name,
				Progress = "0 %"
			};
			OnGKLifecycleChanged(gkLifecycleInfo);
		}

		public void Dispose()
		{
			gkLifecycleInfo.Progress = gkLifecycleInfo.IsError ? "Ошибка" : "Готово";
			OnGKLifecycleChanged(gkLifecycleInfo);
		}

		public void Progress(int current, int total)
		{
			var doubleValue = (double)current / (double)total * 100;
			var stringValue = ((int)doubleValue).ToString() + " %";
			if (gkLifecycleInfo.Progress != stringValue)
			{
				gkLifecycleInfo.Progress = stringValue;
				OnGKLifecycleChanged(gkLifecycleInfo);
			}
		}

		public void AddItem(string name)
		{
			gkLifecycleInfo.Progress = name;
			gkLifecycleInfo.DetalisationItems.Add(name);
			OnGKLifecycleChanged(gkLifecycleInfo);
		}

		public void SetError(string name)
		{
			gkLifecycleInfo.IsError = true;
			gkLifecycleInfo.Progress = name;
			gkLifecycleInfo.DetalisationItems.Add(name);
			OnGKLifecycleChanged(gkLifecycleInfo);
		}

		public static void Add(GKDevice device, string name)
		{
			var gkLifecycleInfo = new GKLifecycleInfo()
			{
				Device = device,
				Name = name
			};
			OnGKLifecycleChanged(gkLifecycleInfo);
		}

		public static void OnGKLifecycleChanged(GKLifecycleInfo gkLifecycleInfo)
		{
			if (GKLifecycleChangedEvent != null)
				GKLifecycleChangedEvent(gkLifecycleInfo);
		}
		public static event Action<GKLifecycleInfo> GKLifecycleChangedEvent;
	}

	public class GKLifecycleInfo
	{
		public GKLifecycleInfo()
		{
			UID = Guid.NewGuid();
			DetalisationItems = new List<string>();
			IsError = false;
		}

		public Guid UID { get; set; }
		public GKDevice Device { get; set; }
		public string Name { get; set; }
		public string Progress { get; set; }
		public List<string> DetalisationItems { get; set; }
		public bool IsError { get; set; }
	}
}