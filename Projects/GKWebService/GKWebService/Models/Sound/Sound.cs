using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models.Sound
{
	public class Sound
	{
		public XStateClass StateClass { get; set; }

		public string SoundName { get; set; }

		//public BeeperType BeeperType { get; set; }

		public bool IsContinious { get; set; }
	}
}