using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.GK;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public class Direction
	{
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public string State { get; set; }
		public string StateIcon { get; set; }
		public string StateColor { get; set; }
		public List<DirectionStateClass> StateClasses { get; set; }
		public bool HasOnDelay { get; set; }
		public bool HasHoldDelay { get; set; }
		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public DeviceControlRegime ControlRegime { get; set; }
		public string ControlRegimeName { get; set; }
		public string ControlRegimeIcon { get; set; }
		public bool CanSetAutomaticState { get; set; }
		public bool CanSetManualState { get; set; }
		public bool CanSetIgnoreState { get; set; }
		public bool IsControlRegime { get; set; }
    }
}