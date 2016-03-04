using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.GK;
using RubezhAPI.GK;

namespace GKWebService.Models
{
	public abstract class GKBaseModel
	{
		public Guid UID { get; set; }

		/// <summary>
		/// Имя 
		/// </summary>
		public string Name { get; set; }

		public GKBaseObjectType ObjectType { get; set; }

		/// <summary>
		/// Изображение-логотип 
		/// </summary>
		public string ImageSource { get; set; }

		public string StateClass { get; set; }

		public GKStateModel State { get; set; }

		public GKBaseModel(GKBase gkObject)
		{
			UID = gkObject.UID;
			Name = gkObject.Name;
			ImageSource = gkObject.ImageSource.Replace("/Controls;component/", "");
			ObjectType = gkObject.ObjectType;
			StateClass = gkObject.State.StateClass.ToString();
			State = new GKStateModel(gkObject.State);
		}
	}
}